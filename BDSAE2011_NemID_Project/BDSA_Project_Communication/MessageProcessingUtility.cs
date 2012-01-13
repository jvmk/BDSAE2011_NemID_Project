// -----------------------------------------------------------------------
// <copyright file="MessageProcessingUtility.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Communication
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// HTTP message body processing utilities.
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann
    /// </author>
    public class MessageProcessingUtility
    {
        /// <summary>
        /// Can I get the text from this stream?
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
            Contract.Requires(stream != null);

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
        /// Does this URL contain a request parameter?
        /// Determines if he url contains a request parameter.
        /// </summary>
        /// <param name="rawUrl">
        /// Raw URL to be processed.
        /// </param>
        /// <returns>
        /// True if the url contains a request, false otherwise.
        /// </returns>
        [Pure]
        public static bool DoesUrlContainRequest(string rawUrl)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(rawUrl));
            Contract.Requires(IsValidUrl(rawUrl));
            return rawUrl.Contains("request=");
        }

        /// <summary>
        /// Is this URL a valid URL?
        /// Checks if the given url is a valid url.
        /// </summary>
        /// <param name="url">
        /// Stirng representation of the URL.
        /// </param>
        /// <returns>
        /// True if it is a valid URL, false otherwise.
        /// </returns>
        [Pure]
        public static bool IsValidUrl(string url)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(url));
            Uri uri = new Uri(url);
            return Uri.TryCreate(url, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeHttp;
        }

        /// <summary>
        /// Get the requested operation from the this URL.
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
            Contract.Requires(url.Contains('&') || (url.LastIndexOf('/') == url.Length - 1));

            int start = url.IndexOf("request=") + "request=".Length;
            int end = -1;

            // The redirect url will contain '&' characters, all others
            // will not.
            end = url.Contains('&') ? url.IndexOf('&') : url.IndexOf('/', start);
            return url.Substring(start, end - start);
        }

        /// <summary>
        /// Get the requester’s domain from this raw HTTP message body.
        /// Gets the requester domain specified in the given
        /// raw HTTP message body.
        /// </summary>
        /// <param name="rawMessageBody">
        /// The raw HTTP message body to be processed.
        /// </param>
        /// <param name="serverPrivateKey">
        /// The private key of the server.
        /// </param>
        /// <returns>
        /// The requester's domain.
        /// Returns null if the message body has been tangled with.
        /// </returns>
        public static string GetRequesterDomain(string rawMessageBody, byte[] serverPrivateKey)
        {
            Contract.Requires(IsRawMessageBodyWellFormed(rawMessageBody));
            Contract.Requires(serverPrivateKey != null);

            int start = rawMessageBody.IndexOf("origin=") + "origin=".Length;

            // Get the index of the last character i in the encrypted domain
            // string:
            int end = rawMessageBody.IndexOf('&');

            // This string is encrypted in the authenticator's public key.
            string encRequesterDomain = rawMessageBody.Substring(start, end - start);
            return Cryptograph.Decrypt(encRequesterDomain, serverPrivateKey);
        }

        /// <summary>
        /// Gets the parameters in the specified raw HTTP message body.
        /// If verification failed, returns null.
        /// </summary>
        /// <param name="rawMessageBody">
        /// The raw HTTP message body to be processed.
        /// </param>
        /// <param name="clientIdentifier">
        /// The domain of the requester, who sent the original HTTP message.
        /// Used for verification of the signed message body part.
        /// </param>
        /// <param name="serverPrivateKey">
        /// The private key of the server.
        /// </param>
        /// <returns>
        /// String representations of the parameters in the specified raw
        /// message body.
        /// Returns null if the message body has been tangled with.
        /// </returns>
        public static string[] GetRequesterParameters(
            string rawMessageBody, string clientIdentifier, byte[] serverPrivateKey)
        {
            Contract.Requires(IsRawMessageBodyWellFormed(rawMessageBody));
            Contract.Requires(Cryptograph.KeyExists(clientIdentifier));
            Contract.Requires(serverPrivateKey != null);

            string decryptedMessage = ProcessRequesterMessageData(rawMessageBody, clientIdentifier, serverPrivateKey);

            // If the decrypted message is null, there are inconsistencies with the 
            // provided client identifier, indicating that the message body has been
            // tangled with.
            if (decryptedMessage == null)
            {
                return null;
            }

            // Process the decrypted messagebody to obtain parameters sent from
            // the client.
            int numberOfParameters = decryptedMessage.Count(c => c.Equals('&')) + 1;
            string[] parameters = new string[numberOfParameters];

            int currentIndex = 0;

            for (int i = 0; i < numberOfParameters; i++)
            {
                int s = decryptedMessage.IndexOf('=', currentIndex) + 1;
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
                    e = decryptedMessage.IndexOf('&', s); // TODO added
                }

                parameters[i] = decryptedMessage.Substring(s, e - s);

                currentIndex = e;
            }

            return parameters;
        }

        /// <summary>
        /// Helper method for GetParameters.
        /// Gets the data sent in the specified raw message body by verifying
        /// and decrypting the message.
        /// </summary>
        /// <param name="rawMessageBody">
        /// The raw http message body to be processed.
        /// </param>
        /// <param name="clientIdentifier">
        /// The unique identifier of the key.
        /// </param>
        /// <param name="serverPrivateKey">
        /// The private key of the server.
        /// </param>
        /// <returns>
        /// The decrypted messagebody.
        /// Returns null if the verification or decryption of
        /// the message failed, indicating wrong client identifier.
        /// </returns>
        private static string ProcessRequesterMessageData(
            string rawMessageBody, string clientIdentifier, byte[] serverPrivateKey)
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
                encMessageBody, signedEncMessageBody, Cryptograph.GetPublicKey(clientIdentifier));

            if (verified)
            {
                return Cryptograph.Decrypt(
                    encMessageBody, serverPrivateKey);
            }

            return null;
        }

        /// <summary>
        /// Is this raw HTTP message body well formed?
        /// Determines if the specified raw message body of an HTTP message
        /// is well formed as defined in the README.
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

        /// <summary>
        /// Set of all the characters that is in the base64 encoding.
        /// </summary>
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
        /// Determines if the passed string only contains base64
        /// characters.
        /// </summary>
        /// <param name="testString">
        /// The string to be tested.
        /// </param>
        /// <returns>
        /// True if the test string only contains characters in 
        /// the base64 encoding, false otherwise.
        /// </returns>
        [Pure]
        private static bool IsBase64String(string testString)
        {
            if (testString.Any(c => !Base64Characters.Contains(c)))
            {
                return false;
            }

            return true;
        }
    }
}
