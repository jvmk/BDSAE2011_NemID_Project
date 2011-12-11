// -----------------------------------------------------------------------
// <copyright file="MessageProcessingUtility.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Miscellaneoues
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MessageProcessingUtility
    {
        /// <summary>
        /// Helper method for ReadMessage.
        /// Reads the bytes from the specified stream and
        /// encodes them in UTF8.
        /// </summary>
        /// <param name="stream">
        /// The stream to be read from.
        /// </param>
        /// <returns>
        /// The string encoded in UTF8 from the bytes read
        /// in the specified stream.
        /// </returns>
        public static string ReadFrom(Stream stream)
        {
            // Read the message sent by the client.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;

            do
            {
                // Read the client's test message.
                bytes = stream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
            }
            while (bytes != 0);

            return messageData.ToString();
        }

        /// <summary>
        /// Determines if he url contains a request parameter.
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        [Pure]
        public static bool DoesUrlContainRequest(string rawUrl)
        {
            return rawUrl.Contains("request=");
        }

        /// <summary>
        /// Gets the requested operation specified in the given URL.
        /// </summary>
        /// <param name="url">
        /// The url containing the string representation of an operation.
        /// </param>
        /// <returns>
        /// String representation of the requested operation.
        /// </returns>
        public static string GetRequesterOperation(string url)
        {
            Contract.Requires(string.IsNullOrEmpty(url));
            Contract.Requires(DoesUrlContainRequest(url));

            int start = url.IndexOf("request=") + "request=".Length;
            int end = url.IndexOf('/', start);

            return url.Substring(start, end - start);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <returns>
        /// Is encrypted
        /// </returns>
        public static string GetRequesterDomain(string rawMessageBody, byte[] serverPrivateKey)
        {
            Contract.Requires(IsRawMessageBodyWellFormed(rawMessageBody));

            int start = rawMessageBody.IndexOf("origin=") + "origin=".Length;

            // Get the index of the last character i in the encrypted domain
            // string:
            int end = rawMessageBody.IndexOf('&') - 1;

            // This string is encrypted in the authenticator's public key.
            string encRequesterDomain = rawMessageBody.Substring(start, end - start);
            return Cryptograph.Decrypt(encRequesterDomain, serverPrivateKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <param name="clientDomain">
        /// The domain of the requester, who sent the original HTTP message.
        /// Used for verification of the signed message body part.
        /// </param>
        /// <returns>
        /// String representations of the parameters in the specified raw
        /// message body.
        /// </returns>
        public static string[] GetRequesterParameters(string rawMessageBody, string clientDomain, byte[] serverPrivateKey)
        {
            Contract.Requires(IsRawMessageBodyWellFormed(rawMessageBody));

            // Get string representation of the parameters to the requested operation
            // invocation sent in the message.

            Console.WriteLine("MessageBody: " + rawMessageBody);
            string decryptedMessage = ProcessRequesterMessageData(rawMessageBody, clientDomain, serverPrivateKey);

            // Process the decrypted messagebody to obtain parameters sent from
            // the client.
            int numberOfParameters = decryptedMessage.Count(c => c.Equals('&')) + 1;
            string[] parameters = new string[numberOfParameters];

            int currentIndex = 0;

            for (int i = 1; i < numberOfParameters; i++)
            {
                int s = decryptedMessage.IndexOf('=', currentIndex) + 1;
                Console.WriteLine(s);

                int e = -1;

                // If true, no more '&'-signs are present and the
                // last parameter of the requester message has been
                // reached.
                if (i + 1 == numberOfParameters)
                {
                    e = decryptedMessage.Length;
                }
                else
                {
                    e = decryptedMessage.IndexOf('&', currentIndex);
                }

                parameters[i] = decryptedMessage.Substring(s, e - s);

                currentIndex = e;
            }

            return parameters;
        }


        /// <summary>
        /// Helper method for GetParameters.
        /// Get the data sent in the specified raw message body by verify
        /// the signing and decrypting the message.
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <param name="clientDomain"></param>
        /// <returns>
        /// The decrypted messagebody.
        /// </returns>
        private static string ProcessRequesterMessageData(string rawMessageBody, string clientDomain, byte[] serverPrivateKey) // TODO client unique identifier.
        {
            Contract.Requires(IsRawMessageBodyWellFormed(rawMessageBody));

            string[] parts = rawMessageBody.Split('&');

            // encMessageBody is encrypted in the authenticator's public
            // key. This text represents the text that is signed by the
            // client.
            string encMessageBody = parts[1];

            // signedEncMessageBody is the signed text of the encMessageBody.
            // It has been signed by the client's private key.
            string signedEncMessageBody = parts[2];

            bool verified = Cryptograph.VerifyData(
                encMessageBody, signedEncMessageBody, Cryptograph.GetPublicKey(clientDomain));

            if (verified)
            {
                return Cryptograph.Decrypt(
                    encMessageBody, serverPrivateKey);
            }

            // TODO way to end?
            // The message has been tangled with:
            throw new Exception();
        }

        /// <summary>
        /// Determines if the specified raw message body of an HTTP message
        /// is well formed as defined in the http message structure text. // TODO done?
        /// </summary>
        /// <param name="rawMessageBody">
        /// The string representing a raw HTTP message body.
        /// </param>
        /// <returns>
        /// True if the raw message body is well formed, false otherwise.
        /// </returns>
        [Pure]
        public static bool IsRawMessageBodyWellFormed(string rawMessageBody)
        {
            if (string.IsNullOrEmpty(rawMessageBody))
            {
                return false;
            }

            bool isWellFormed = true;

            isWellFormed = rawMessageBody.StartsWith("origin=") && rawMessageBody.Contains('&');

            if (!isWellFormed)
            {
                return false;
            }

            // Encrypted origin well formed?
            int start = "origin=".Length;
            int end = rawMessageBody.IndexOf('&');

            string encOrigin = rawMessageBody.Substring(start, end - start);
            isWellFormed = IsBase64String(encOrigin);

            if (!isWellFormed)
            {
                return false;
            }

            isWellFormed = rawMessageBody.Count(c => c.Equals('&')) == 2;

            if (!isWellFormed)
            {
                return false;
            }

            // Is the two encrypted parameters well formed (in base64-encoding)?
            string[] parts = rawMessageBody.Split('&');

            isWellFormed = IsBase64String(parts[1]);
            isWellFormed = isWellFormed && IsBase64String(parts[2]);

            return isWellFormed;
        }   // TODO VERIFIED.

        private static readonly HashSet<char> Base64Characters = new HashSet<char>() 
            { 
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
                'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/', 
                '=' 
            };

        /// <summary>
        /// Helper method for IsRawMessageBodyWellFormed.
        /// </summary>
        /// <param name="testString"></param>
        /// <returns></returns>
        [Pure]
        private static bool IsBase64String(string testString)
        {
            if (testString.Any(c => !Base64Characters.Contains(c)))
            {
                return false;
            }

            return true;
        }  // TODO VERIFIED.

    }
}
