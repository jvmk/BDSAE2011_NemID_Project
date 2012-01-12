﻿// -----------------------------------------------------------------------
// <copyright file="ThirdPartyServer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// Class running the third party server.
    /// <author>Janus Varmarken</author>
    /// </summary>
    public class ThirdPartyServer
    {
        /// <summary>
        /// The server that listens for requests.
        /// </summary>
        private HttpListener server;

        /// <summary>
        /// A collection with addresses for sub pages where data posting is permitted.
        /// </summary>
        private readonly StringCollection subpagesForPost = new StringCollection();

        /// <summary>
        /// Represents the ThirdParty backend database
        /// </summary>
        private ThirdParty database = new ThirdParty();

        private byte[] serversPrivateKey;

        /// <summary>
        /// Initializes a new instance of the ThirdPartyServer class.
        /// Protocol cannot be specified since a server of this type should always use https.
        /// </summary>
        /// <param name="fullURI">The full URI for this server.</param>
        /// <param name="serversPrivateKey">The private key of this server.</param>
        public ThirdPartyServer(string serverAddress, byte[] serversPrivateKey)
        {
            this.server = new HttpListener();
            this.serversPrivateKey = serversPrivateKey;
            this.server.Prefixes.Add(serverAddress); // TODO update to https after testing!!!
            this.subpagesForPost.AddRange(new[] { "/request=loginpage", "/request=usertoken/", "/request=authtoken/", "/request=newuseradded/", "/request=userdeleted/" });
        }


        /// <summary>
        /// Start the server and listen for requests.
        /// </summary>
        public void RunServer()
        {
            this.server.Start(); // start the server
            Console.WriteLine(".Start() called on ThirdPartyServer...");

            // Run forever
            while (true)
            {
                Console.WriteLine("ThirdPartyServer is now listening for request... (entered GetContext loop).");
                HttpListenerContext hlc = this.server.GetContext(); // blocking call
                HttpListenerRequest request = hlc.Request;
                HttpListenerResponse response = hlc.Response;

                Console.WriteLine(
                    "SSL connection between 3rd Party and Client established: "
                    + request.IsSecureConnection);
                Console.WriteLine("<ThirdPartyServer>: User authenticated: " + request.IsAuthenticated);

                // Extract the HTTP method of the request
                string requestMethod = request.HttpMethod;

                // Get the encrypted content of the message
                string rawMessageBody = MessageProcessingUtility.ReadFrom(request.InputStream);
                // Always close streams (don't allocate resources that are not needed)
                request.InputStream.Close();

                Console.WriteLine("HTTP request to " + this.server.Prefixes.First() + " recieved.");
                Console.WriteLine("Request method: " + requestMethod + " to resource " + request.RawUrl);
                Console.WriteLine("--- REQUEST MESSAGE BODY BEGINS: ---");
                Console.WriteLine(rawMessageBody);
                Console.WriteLine("--- REQUEST MESSAGE BODY ENDS ---");

                // Let request method specify action
                switch (requestMethod)
                {
                    case "GET":
                        this.InvokeGetRequest(hlc);
                        break;
                    case "POST":
                        if (!this.IsPostingAllowed(hlc))
                        {
                            // If requested url is not part of subpagesForPost, posting is not allowed!
                            response.StatusCode = 405; // Status code: http method not allowed
                            response.Headers.Add(HttpRequestHeader.Allow, "GET"); // inform that only GET is allowed for any page not in subpagesForPost
                            response.Close(); // send response
                        }
                        else
                        {
                            Console.WriteLine("Processing the POST request...");
                            this.ProcessIncomingPost(hlc, rawMessageBody);
                            Console.WriteLine("POST request processed!");
                        }
                        break;
                    default:
                        // Reply that an error has occured: http method was not supported
                        response.StatusCode = 501; // method not supported
                        response.StatusDescription = "HTTP method not supported by server.";
                        response.ProtocolVersion = HttpVersion.Version11;
                        response.Close();
                        break;
                }
            }
        }

        /// <summary>
        /// If HTTP method is GET, call this method to handle the request.
        /// </summary>
        /// <param name="hlc">The context in which the request was recieved.</param>
        private void InvokeGetRequest(HttpListenerContext hlc)
        {
            HttpListenerRequest request = hlc.Request;
            HttpListenerResponse response = hlc.Response; // response to send
            string requestedUrl = request.RawUrl; // requested sub page
            if (ReferenceEquals(requestedUrl, null))
            {
                // guard against null ref if unspecified get string
                requestedUrl = string.Empty;
            }
            Console.WriteLine("Requested RawURL: " + requestedUrl);
            switch (requestedUrl)
            {
                case @"/request=loginpage":
                    response.StatusCode = 200;

                    byte[] responseBytes = Encoding.UTF8.GetBytes("OK");

                    response.OutputStream.Write(responseBytes, 0, responseBytes.Length); // the html for the loginpage should be here - write it to response outputstream
                    response.OutputStream.Flush();
                    response.OutputStream.Close();

                    break;

                // no other sub pages designed for get requests
                default:
                    // direct user to main page
                    response.StatusCode = 404; // NOT FOUND status code
                    response.Close();
                    break;
            }
        }

        /// <summary>
        /// Determines if the target resource accepts POST requests.
        /// </summary>
        /// <param name="hlc">The HttpListenerContext in which the request is contained.</param>
        /// <returns>True if posting is allowed, false otherwise.</returns>
        private bool IsPostingAllowed(HttpListenerContext hlc)
        {
            Contract.Requires(!ReferenceEquals(hlc, null));
            HttpListenerRequest request = hlc.Request;
            string requestedUrl = request.RawUrl;
            if (this.subpagesForPost.Contains(requestedUrl))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes an incoming http POST request and takes action according to target resource and the validity of the message body.
        /// </summary>
        /// <param name="hlc">The HttpListenerContext in which the request was recieved.</param>
        private void ProcessIncomingPost(HttpListenerContext hlc, string rawMessageBody)
        {
            Contract.Requires(!ReferenceEquals(hlc, null));
            Contract.Requires(this.IsPostingAllowed(hlc));
            HttpListenerResponse response = hlc.Response; // obtain response object
            string urlForPost = hlc.Request.RawUrl; // Find out what subpage the data was posted to

            if (urlForPost.Equals("/request=loginpage"))
            {
                // special case where clientPkiId cannot be determined (no email to identify public key)
                string encUsername = rawMessageBody;
                // decrypt username
                string decUsername = Cryptograph.Decrypt(encUsername, this.serversPrivateKey);

                Console.WriteLine("Encrypted user name: " + encUsername);
                Console.WriteLine("Decrypted user name: " + decUsername);

                this.AnswerLoginpagePost(response, decUsername);
                return;
            }

            string clientPki = MessageProcessingUtility.GetRequesterDomain(rawMessageBody, this.serversPrivateKey);
            if (!this.ValidatePostOrigin(rawMessageBody, clientPki))
            {
                // message was not valid - message has been tampered with, end request here
                response = this.SetupForbiddenResponse(
                    response,
                    "The server recognized an attempt to modify your message, the transaction was aborted.");
                response.Close();
                return;
            }

            string[] inputValues = MessageProcessingUtility.GetRequesterParameters(
                rawMessageBody, clientPki, this.serversPrivateKey); // Processes the message body and obtains the values associated with the request's parametres

            switch (urlForPost)
            {
                case @"/request=authtoken/":
                    Console.WriteLine("Processing /request=authtoken request with username=" + inputValues[0] + " and token value=" + inputValues[1]);
                    this.AnswerAuthtokenPost(response, rawMessageBody, inputValues[0], inputValues[1]); // index 0 is username, index 1 is token
                    break;
                case @"/request=usertoken/":
                    Console.WriteLine("Processing /request=usertoken request with username=" + inputValues[0] + " and token value=" + inputValues[1]);
                    this.AnswerUsertokenPost(clientPki, response, inputValues[0], inputValues[1]); // index 0 is username, index 1 is token
                    break;
                case @"/request=newuseradded/":
                    Console.WriteLine("Processing /request=newuseradded/ request...");
                    this.ProcessUserDatabaseUpdate(response, rawMessageBody, true);
                    break;
                case @"/request=userdeleted/":
                    Console.WriteLine("Processing /request=userdeleted/ request...");
                    this.ProcessUserDatabaseUpdate(response, rawMessageBody, false);
                    break;
                default:
                    // Resource either not found or not meant for posting
                    Console.WriteLine("Requested resource not found.");
                    response.StatusCode = 404; // Status code: NOT FOUND
                    response.StatusDescription = "Resource not found.";
                    break;
            }
        }

        /// <summary>
        /// Helper method for setting up the response to indicate the request was forbidden.
        /// </summary>
        /// <param name="response">The response to modify</param>
        /// <param name="description">Status description for the response.</param>
        /// <returns>The response that is not setup to be a forbidden response.</returns>
        private HttpListenerResponse SetupForbiddenResponse(HttpListenerResponse response, string description)
        {
            Contract.Requires(!ReferenceEquals(response, null));
            Contract.Requires(!ReferenceEquals(description, null));
            Contract.Ensures(!ReferenceEquals(response, null));
            response.StatusDescription = description;
            response.StatusCode = 403;
            return response;
        }

        /// <summary>
        /// Verify that the message came from the expected source.
        /// </summary>
        /// <param name="httpMessageBody">The encrypted http message body.</param>
        /// <param name="expectedPkiId">The PKI identifier to verify against.</param>
        /// <returns>True if the token was sent by expected source, false otherwise.</returns>
        private bool ValidatePostOrigin(string httpMessageBody, string expectedPkiId)
        {
            Contract.Requires(!ReferenceEquals(httpMessageBody, null));
            Contract.Requires(!ReferenceEquals(expectedPkiId, null));
            string senderPkiId = MessageProcessingUtility.GetRequesterDomain(httpMessageBody, this.serversPrivateKey);

            if (ReferenceEquals(senderPkiId, null) || !senderPkiId.Equals(expectedPkiId))
            {
                // Origin is not determable or not equal to expected origin
                return false;
            }

            string[] parts = httpMessageBody.Split('&');

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Parameter " + i + ": " + parts[i]);
            }

            // encMessageBody is encrypted in the authenticator's public
            // key. This text represents the text that is signed by the
            // client.
            string encMessageBody = parts[1];

            // signedEncMessageBody is the signed text of the encMessageBody.
            // It has been signed by the client's private key.
            string signedEncMessageBody = parts[2];

            return Cryptograph.VerifyData(
                encMessageBody, signedEncMessageBody, Cryptograph.GetPublicKey(senderPkiId));
        }

        // < HELPER METHODS FOR POST PROCESSING STARTS >

        /// <summary>
        /// Answers a POST to the request=loginpage resource.
        /// </summary>
        /// <param name="response">The HttpListenerResponse obtained from the HttpListenerContext that is associated with the post to the request=loginpage resource.</param>
        /// <param name="username">The username posted to the resource.</param>
        private void AnswerLoginpagePost(HttpListenerResponse response, string username)
        {
            Contract.Requires(!ReferenceEquals(response, null));
            Contract.Requires(!ReferenceEquals(username, null));
            // check username is valid in own database
            if (this.database.ContainsUsername(username))
            {
                // redirect to authenticator
                response.StatusCode = 200;
                // response.StatusDescription = "Redirecting you to authenticator.";
                response.Redirect(StringData.AuthUri + "request=redirect&username=" + username + "&3rd=" + this.server.Prefixes.First());
                response.Close();
            }
            else
            {
                response = this.SetupForbiddenResponse(response, "Username not found.");
                response.Close();
            }
        }

        /// <summary>
        /// Answers a POST to the request=authtoken resource.
        /// Only the trusted authenticator is permitted to perform a post to this resource.
        /// </summary>
        /// <param name="response">The HttpListenerResponse obtained from the HttpListenerContext that is associated with the post to the request=authtoken resource.</param>
        /// <param name="rawMessageBody">The full (encrypted) content of the request's message body.</param>
        /// <param name="username">The username extracted from the rawMessageBody indicating what account to associate the tokenValue with.</param>
        /// <param name="tokenValue">A token (nonce) generated by the authenticator to associate with the specified username.</param>
        private void AnswerAuthtokenPost(HttpListenerResponse response, string rawMessageBody, string username, string tokenValue)
        {
            Contract.Requires(!ReferenceEquals(response, null));
            Contract.Requires(!ReferenceEquals(rawMessageBody, null));
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Requires(!ReferenceEquals(tokenValue, null));
            // check that the post came from the authenticator
            if (this.ValidatePostOrigin(rawMessageBody, StringData.AuthUri))
            {
                this.database.SetAuthTokenForAccount(username, tokenValue);
                response.StatusCode = 200;
                response.StatusDescription = "Authenticator token successfully submitted.";
                response.Close();
            }
            else
            {
                // Forbidden for anyone else than the authenticator
                response = this.SetupForbiddenResponse(
                    response, "You do not have rights to post to this resource.");
                response.Close();
            }
        }

        /// <summary>
        /// Answers a POST to the request=usertoken resource.
        /// This methods performs the final step in the authentication process assuring that the user provided and authenticator provided tokens are equal (and grants access if that is indeed the case).
        /// </summary>
        /// <param name="response">The HttpListenerResponse obtained from the HttpListenerContext that is associated with the post to the request=userhtoken resource.</param>
        /// <param name="username">The username that provides the token (nonce)</param>
        /// <param name="tokenValue">The token (nonce) the user has send which is compared to the authenticator provided token.</param>
        private void AnswerUsertokenPost(string clientPki, HttpListenerResponse response, string username, string tokenValue)
        {
            Contract.Requires(!ReferenceEquals(response, null));
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Requires(!ReferenceEquals(tokenValue, null));
            if (this.database.CompareTokens(tokenValue, username))
            {
                response.StatusCode = 200; // HTTP OK status
                response.StatusDescription = "Authentication successful.";

                String responseMessage = this.CompileMessageBody(clientPki, "Authenticated");
                byte[] messageBytes = Encoding.UTF8.GetBytes(responseMessage);
                response.OutputStream.Write(messageBytes, 0, messageBytes.Length);

                response.Close();
            }
            else
            {
                response = this.SetupForbiddenResponse(response, "Access denied, possible timeout.");
                response.Close();
            }
        }

        private string CompileMessageBody(string clientPki, string message)
        {
            Contract.Requires(message != null);

            StringBuilder messageBody = new StringBuilder();

            // Domain encrypted in third party's public key.
            string encDomain = Cryptograph.Encrypt(
                StringData.ThirdUri, Cryptograph.GetPublicKey(clientPki));

            messageBody.Append("origin=" + encDomain + "&");

            // Encrypt message in third party's public key.
            string encMessage = Cryptograph.Encrypt(
                message, Cryptograph.GetPublicKey(clientPki));

            messageBody.Append(encMessage + "&");

            // Sign encMessage in client's private key.
            string signedEncMessage = Cryptograph.SignData(encMessage, this.serversPrivateKey);

            messageBody.Append(signedEncMessage);

            return messageBody.ToString();
        }

        /// <summary>
        /// Perform an update the third party database (add or delete a user).
        /// Update will only be performed if the request comes from the authenticator.
        /// </summary>
        /// <param name="response">The HttpListernerResponse that is the response to the current request.</param>
        /// <param name="rawMessageBody">The string representation of the message body of the current request.</param>
        /// <param name="addUser">Set to: true if this is a request to add a user to the database,
        /// false if this is a request to delete a user from the database.</param>
        private void ProcessUserDatabaseUpdate(HttpListenerResponse response, string rawMessageBody, bool addUser)
        {
            // Request is from the authenticator, process the request and add the new user to the database...
            bool authIsSender = false;
            string[] parameters = this.GetMessageParametersAndEnsureAuthenticatorIsSender(rawMessageBody, ref authIsSender);
            // If auth is sender it implies that the parameters array is not null and has a value on index 0.
            if (authIsSender)
            {
                string username = parameters[0];
                if (addUser)
                {
                    bool userAdded = this.database.AddUserAccount(username);
                    Console.WriteLine("Added the username: " + username + " to the third party database: " + userAdded);
                    response.StatusDescription = "User successfully added to database: " + userAdded + " (false indicates that user was already in the database).";
                }
                else
                {
                    bool userDeleted = this.database.DeleteUserAccount(username);
                    Console.WriteLine("Deleted user with username " + username + " from the third party database: " + userDeleted);
                    response.StatusDescription = "User successfully found and deleted from database: " + userDeleted + " (false indicates that user was not found in database).";
                }
                response.StatusCode = 200;
                response.Close();
            }
            else
            {
                this.SetupForbiddenResponse(response, "You do not have rights to post to this resource.");
                response.Close();
            }
        }

        /// <summary>
        /// Obtain the parameters of a request.
        /// </summary>
        /// <param name="rawMessageBody">The full message body of the HTTP request.</param>
        /// <param name="authIsSender">A ref parameter indicating (after method execution) whether or not the authenticator is the sender of the request.</param>
        /// <returns>A string array with the parameters. Null if the parameters cannot be obtained (fx decryption or verification fails)</returns>
        private string[] GetMessageParametersAndEnsureAuthenticatorIsSender(string rawMessageBody, ref bool authIsSender)
        {
            string[] parameters = MessageProcessingUtility.GetRequesterParameters(rawMessageBody, StringData.AuthUri, serversPrivateKey);
            if (ReferenceEquals(parameters, null) || parameters.Length == 0) // Validation not sucessfull if either is true
            {
                // Message was tangled with or the sender was not the authenticator...
                authIsSender = false;
                return null;
            }
            authIsSender = true;
            return parameters;
        }

        // < / HELPER METHODS FOR POST PROCESSING ENDS >
    }
}
