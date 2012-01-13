// -----------------------------------------------------------------------
// <copyright file="ThirdPartyServer.cs" company="NemID Open Source Alternative">
//  NemID Open Source Alternative
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
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
        private readonly HttpListener server;

        /// <summary>
        /// A collection with addresses for sub pages where data posting is permitted.
        /// </summary>
        private readonly StringCollection subpagesForPost = new StringCollection();

        /// <summary>
        /// Represents the ThirdParty backend database
        /// </summary>
        private readonly ThirdParty database = new ThirdParty();

        /// <summary>
        /// The private key of the ThirdPartyServer.
        /// </summary>
        private readonly byte[] serversPrivateKey;

        /// <summary>
        /// Initializes a new instance of the ThirdPartyServer class.
        /// Protocol cannot be specified since a server of this type should always use https.
        /// </summary>
        /// <param name="serverAddress">The full URI for this server.</param>
        /// <param name="serversPrivateKey">The private key of this server.</param>
        public ThirdPartyServer(string serverAddress, byte[] serversPrivateKey)
        {
            this.server = new HttpListener();
            this.serversPrivateKey = serversPrivateKey;
            this.server.Prefixes.Add(serverAddress);
            this.subpagesForPost.AddRange(new[] { "/request=loginpage", "/request=usertoken/", "/request=authtoken/", "/request=newuseradded/", "/request=userdeleted/" });
        }


        /// <summary>
        /// Start the server and listen for requests.
        /// </summary>
        public void RunServer()
        {
            this.server.Start(); // start the server
            Console.WriteLine("[ThirdPartyServer]: Server started, waiting for incomning request...");

            // Run forever
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("[ThirdPartyServer]: Waiting for incoming request...");
                HttpListenerContext hlc = this.server.GetContext(); // blocking call
                Console.WriteLine();
                Console.WriteLine("[ThirdPartyServer]: Request received.");
                HttpListenerRequest request = hlc.Request;
                HttpListenerResponse response = hlc.Response;

                Console.WriteLine(
                    "[ThirdPartyServer]: SSL connection between client and ThirdPartyServer established: "
                    + request.IsSecureConnection);
                
                // Extract the HTTP method of the request
                string requestMethod = request.HttpMethod;

                // Get the encrypted content of the message
                string rawMessageBody = MessageProcessingUtility.ReadFrom(request.InputStream);
                // Always close streams (don't allocate resources that are not needed)
                request.InputStream.Close();

                Console.WriteLine("[ThirdPartyServer]: Request method: " + requestMethod + ", to resource: " + request.RawUrl);
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
                            Console.WriteLine("[ThirdPartyServer]: Processing the POST request...");
                            this.ProcessIncomingPost(hlc, rawMessageBody);
                            Console.WriteLine("[ThirdPartyServer]: POST request processed!");
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
            switch (requestedUrl)
            {
                case @"/request=loginpage":
                    response.StatusCode = 200;
                    byte[] responseBytes = Encoding.UTF8.GetBytes("OK");
                    response.OutputStream.Write(responseBytes, 0, responseBytes.Length); // the html for the loginpage should be here - write it to response outputstream
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                    response.Close();
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
        /// <param name="rawMessageBody">The encrypted content of the request (the message body).</param>
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
                Console.WriteLine("[ThirdPartyServer]: Decrypting username...");
                string decUsername = Cryptograph.Decrypt(encUsername, this.serversPrivateKey);
                if (ReferenceEquals(decUsername, null))
                {
                    Console.WriteLine("[ThirdPartyServer]: The username was not encrypted in the public key of this server; aborting.");
                    response.StatusCode = 400;
                    response.StatusDescription =
                        "Username not encrypted in the public key of the server, cannot process request.";
                    response.Close();
                    return;
                }
                Console.WriteLine("[ThirdPartyServer]: Decrypted user name: " + decUsername);
                this.AnswerLoginpagePost(response, decUsername);
                return;
            }
            // Try to obtain the origin value attached to the message.
            string clientPki = MessageProcessingUtility.GetRequesterDomain(rawMessageBody, this.serversPrivateKey);

            if (ReferenceEquals(clientPki, null) || !Cryptograph.KeyExists(clientPki))
            {
                // Not possible to indentify origin value attached to message (or origin value not found in PKI)
                Console.WriteLine("[ThirdPartyServer]: Message origin could not be determined through the PKI; aborting request.");
                response = this.SetupForbiddenResponse(response, "Origin could not be determined through the PKI.");
                response.Close();
                return;
            }

            switch (urlForPost)
            {
                case @"/request=authtoken/":
                    this.AnswerAuthtokenPost(response, rawMessageBody);
                    break;
                case @"/request=usertoken/":
                    this.AnswerUsertokenPost(response, clientPki, rawMessageBody);
                    break;
                case @"/request=newuseradded/":
                    this.ProcessUserDatabaseUpdate(response, rawMessageBody, true);
                    break;
                case @"/request=userdeleted/":
                    this.ProcessUserDatabaseUpdate(response, rawMessageBody, false);
                    break;
                default:
                    // Resource either not found or not meant for posting
                    Console.WriteLine("[ThirdPartyServer]: Requested resource not found; aborting request.");
                    response.StatusCode = 404; // Status code: NOT FOUND
                    response.StatusDescription = "Resource not found.";
                    response.Close();
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
                Console.WriteLine("[ThirdPartyServer]: Username '" + username + "' successfully found in user-database; redirecting to authenticator.");
                // redirect to authenticator
                response.StatusCode = 200;
                // response.StatusDescription = "Redirecting you to authenticator.";
                response.Redirect(StringData.AuthUri + "request=redirect&username=" + username + "&3rd=" + this.server.Prefixes.First());
                response.Close();
                Console.WriteLine("[ThirdPartyServer]: Successfully redirected '" + username + "' to " + 
                    StringData.AuthUri + "request=redirect&username=" + username + "&3rd=" + this.server.Prefixes.First());
            }
            else
            {
                Console.WriteLine("[ThirdPartyServer]: Username '" + username + "' not found in user-database; aborting request.");
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
        private void AnswerAuthtokenPost(HttpListenerResponse response, string rawMessageBody)
        {
            bool authIsSender = false;
            string[] parameters = this.GetMessageParametersAndEnsureAuthenticatorIsSender(
                rawMessageBody, ref authIsSender);
            if (authIsSender)
            {
                if(parameters.Length < 2)
                {
                    // Malformed request for this resource
                    Console.WriteLine("[ThirdPartyServer]: Authenticator tried to post a token but the message was malformed; aborting request.");
                    response.StatusCode = 400;
                    response.StatusDescription = "Too few parameters for this resource.";
                    response.Close();
                }
                else
                {
                    Console.WriteLine("[ThirdPartyServer]: Updating authenticator token for username: " + parameters[0] + " to token value: " + parameters[1]);
                    this.database.SetAuthTokenForAccount(parameters[0], parameters[1]); // index 0 is username, index 1 is token value.
                    response.StatusCode = 200;
                    response.StatusDescription = "Authenticator token sucessfully submitted.";
                    response.Close();
                    Console.WriteLine("[ThirdPartyServer]: Authenticator token sat for username: " + parameters[0] + ". New token value: " + parameters[1]);
                }
            }
            else
            {
                // Request could not be verified to come from the authenticator
                Console.WriteLine("[ThirdPartyServer]: Attempt to post authenticator token without authenticator signature was denied; aborting request.");
                response = this.SetupForbiddenResponse(response, "You do not have rights to POST to this resource.");
                response.Close();
            }
        }

        /// <summary>
        /// Answers a POST to the request=usertoken resource.
        /// This methods performs the final step in the authentication process assuring that the user provided and authenticator provided tokens are equal (and grants access if that is indeed the case).
        /// </summary>
        /// <param name="response">The HttpListenerResponse obtained from the HttpListenerContext that is associated with the post to the request=userhtoken resource.</param>
        /// <param name="clientPki">The PKI identifier extracted from the origin parameter of the message.</param>
        /// <param name="rawMessageBody">The full (encrypted) message body.</param>
        private void AnswerUsertokenPost(HttpListenerResponse response, string clientPki, string rawMessageBody)
        {
            string[] parameters = MessageProcessingUtility.GetRequesterParameters(
                rawMessageBody, clientPki, this.serversPrivateKey);
            if (ReferenceEquals(parameters, null))
            {
                // Request decryption and/or verification has failed
                Console.WriteLine("[ThirdPartyServer]: Could not decrypt/verify a post to submit a usertoken; aborting request.");
                response.StatusCode = 400;
                response.StatusDescription =
                    "Could not decrypt and verify according to given origin.";
                response.Close();
                return;
            }
            if (parameters.Length < 2)
            {
                // Malformed request for this resource
                response.StatusCode = 400;
                response.StatusDescription = "Malformed message, too few parameters for this resource.";
                response.Close();
                Console.WriteLine("[ThirdPartyServer]: Attempt to submit a usertoken was denied because of missing parameters in the request message.");
            }
            else
            {
                // (parameters: index 1 is tokenValue, index 0 is username)
                if (!this.database.ContainsUsername(parameters[0]) || !clientPki.Equals(this.database.PkiIdForAccount(parameters[0])))
                {
                    // If the username is not found in the database or the pki id specified in origin does not match the pki
                    // id of the account, the sender cannot be verified as the actual owner of the account.
                    Console.WriteLine("[ThirdPartyServer]: PKI of request did not match PKI of the target account for which the user token was submitted; aborting request.");
                    response = this.SetupForbiddenResponse(response, "You do not have rights to post to this resource.");
                    response.Close();
                    return;
                }
                // Sender is now verified to be the account owner, now comnpare tokens.
                if (this.database.CompareTokens(parameters[1], parameters[0]))
                {
                    Console.WriteLine("[ThirdPartyServer]: Usertoken submission accepted (Username: '" + parameters[0] + "' Token: " + parameters[1] + ").");
                    response.StatusCode = 200; // HTTP OK status
                    response.StatusDescription = "Authentication successful.";
                    String responseMessage = this.CompileMessageBody(clientPki, "Authenticated");
                    byte[] messageBytes = Encoding.UTF8.GetBytes(responseMessage);
                    response.OutputStream.Write(messageBytes, 0, messageBytes.Length);
                    response.Close();
                    Console.WriteLine("[ThirdPartyServer]: Authentication successful for user: " + parameters[0] + " (PKI id for account is: " + 
                        this.database.PkiIdForAccount(parameters[0]) + ").");
                }
                else
                {
                    // Token incorrect
                    response = this.SetupForbiddenResponse(
                        response, "Token incorrect. You must restart authentication process.");
                    response.Close();
                    Console.WriteLine("[ThirdPartyServer]: Incorrect token submitted for user '" + parameters[0] + "' (PKI id = "
                        + this.database.PkiIdForAccount(parameters[0]) +
                        "). Submitted token value: " + parameters[1]);
                }
            }
        }

        /// <summary>
        /// Used to encrypt and setup a message that corresponds to the standards used for messages.
        /// </summary>
        /// <param name="clientPki">The PKI identifier for the recipent of the message.</param>
        /// <param name="message">The to-be encrypted complete message body.</param>
        /// <returns>The encrypted message body to be send via a HTTP message.</returns>
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
                string pkiIdEmail = parameters[1];
                if (addUser)
                {
                    bool userAdded = this.database.AddUserAccount(username, pkiIdEmail);
                    Console.WriteLine("[ThirdPartyServer]: Added the username: '" + username + "' to the user-database: " + userAdded);
                    response.StatusDescription = "User successfully added to database: " + userAdded + " (false indicates that user was already in the database).";
                }
                else
                {
                    bool userDeleted = this.database.DeleteUserAccount(username);
                    Console.WriteLine("[ThirdPartyServer]: Deleted user with username '" + username + "' from the user-database: " + userDeleted);
                    response.StatusDescription = "User successfully found and deleted from database: " + userDeleted + " (false indicates that user was not found in database).";
                }
                response.StatusCode = 200;
                response.Close();
            }
            else
            {
                Console.WriteLine("[ThirdPartyServer]: Attempt to update user database denied (Request was not send by the authenticator).");
                response = this.SetupForbiddenResponse(response, "You do not have rights to post to this resource.");
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
