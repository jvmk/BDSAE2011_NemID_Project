﻿    // -----------------------------------------------------------------------
    // <copyright file="ThirdPartyServer.cs" company="">
    // TODO: Update copyright text.
    // </copyright>
    // -----------------------------------------------------------------------

    namespace ThirdPartyComponent
    {
        using System;
        using System.Collections.Specialized;
        using System.IO;
        using System.Linq;
        using System.Net;
        using System.Text;
        using Miscellaneoues;

        /// <summary>
        /// Class running the third party server.
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
            /// The URI of the trusted authenticator.
            /// </summary>
            private static readonly string AUTH_URI = "https://localhost:8081/"; // TODO agree on this address

            /// <summary>
            /// Represents the ThirdParty backend database
            /// </summary>
            private ThirdParty database = new ThirdParty();

            private byte[] serversPrivateKey;

            /// <summary>
            /// Initializes a new instance of the ThirdPartyServer class.
            /// Protocol cannot be specified since a server of this type should always use https.
            /// </summary>
            /// <param name="serverDomain">The domain the server should run on, fx "localhost"</param>
            /// <param name="serverPort">The port the server should listen to.</param>
            /// <param name="serversPrivateKey">The private key of this server.</param>
            public ThirdPartyServer(string serverDomain, uint serverPort, byte[] serversPrivateKey)
            {
                this.server = new HttpListener();
                this.serversPrivateKey = serversPrivateKey;
                string serverAddress = @"http://" + serverDomain + ":" + serverPort + @"/"; // TODO update to https after testing!!!
                this.server.Prefixes.Add(serverAddress);
                this.subpagesForPost.AddRange(new[] { @"/request=loginpage", @"/request=usertoken", @"/request=authtoken" });
            }


            /// <summary>
            /// Start the server and listen for requests.
            /// </summary>
            public void RunServer()
            {
                this.server.Start(); // start the server

                // Run forever
                while (true)
                {
                    HttpListenerContext hlc = this.server.GetContext(); // blocking call
                    HttpListenerRequest request = hlc.Request;
                    Console.WriteLine(
                        "SSL connection between 3rd Party and Client established: "
                        + request.IsSecureConnection);
                    Console.WriteLine("<ThirdPartyServer>: User authenticated: " + request.IsAuthenticated);
                    string requestMethod = request.HttpMethod;
                    
                    // Let request method specify action
                    switch (requestMethod) 
                    {
                        case "GET":
                            this.InvokeGetRequest(hlc);
                            break;
                        case "POST":
                            string postData = this.GetPostString(hlc); // read the posted input
                            if (string.IsNullOrWhiteSpace(postData))
                            {
                                // not a valid request, do nothing
                                break;
                            }
                            else
                            {
                                this.ProcessIncomingPost(hlc);
                                break;
                            }

                        default:
                            // Reply that an error has occured: http method was not supported
                            HttpListenerResponse response = hlc.Response;
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

                requestedUrl = requestedUrl.Trim(); // remove white space
                Console.WriteLine("Requested RawURL: " + requestedUrl);
                string responseStr = string.Empty;
                switch (requestedUrl)
                {
                    case @"/request=loginpage":
                        // respond with loginpage
                        responseStr =
                            "<html><head><title>Login</title></head>" +
                            "<body>Login page html goes here</body></html>";
                        break;

                    // no other sub pages designed for get requests
                    default:
                        // direct user to main page
                        response.StatusCode = 404; // NOT FOUND status code
                        response.Redirect(this.server.Prefixes.First());
                        return;
                }

                // Write response...
                Stream output = response.OutputStream;
                byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
                output.Write(buffer, 0, buffer.Length);
                response.Close();
            }

            /// <summary>
            /// Extract the message body of a POST request.
            /// </summary>
            /// <param name="hlc">The context in which the request was recieved.</param>
            /// <returns>The message body as an UTF8 encoded string. Returns null if posting is not allowed to the requested URL.</returns>
            private string GetPostString(HttpListenerContext hlc)
            {
                HttpListenerRequest request = hlc.Request;
                HttpListenerResponse response = hlc.Response;
                string requestedUrl = request.RawUrl;
                if (!this.subpagesForPost.Contains(requestedUrl)) 
                {
                    // if requested url is not part of subpagesForPost, posting is not allowed!
                    response.StatusCode = 405; // Status code: http method not allowed
                    response.Headers.Add(HttpRequestHeader.Allow, "GET"); // inform that only GET is allowed for any page not in subagesForPost
                    response.Close(); // send response
                    return null; // not a valid request
                }

                Stream input = request.InputStream; // access inputstream
                byte[] buf = new byte[input.Length]; // TODO Stream.Length is a long - possible int overflow here
                input.Read(buf, 0, buf.Length);
                string postedData = Encoding.UTF8.GetString(buf);
                input.Close(); // close the stream
                return postedData;
            }

            /// <summary>
            /// Gets the decrypted PKI identifier of the sender.
            /// </summary>
            /// <param name="httpMessageBody">The decrypted content of a http message.</param>
            /// <returns>Gets the decrypted PKI identifier of the sender.</returns>
            //private String GetSenderPkiIdFromHttpMessage(string httpMessageBody)
            //{
                //Contract.Requires(!string.IsNullOrWhiteSpace(httpMessageBody));
                //Contract.Requires(httpMessageBody.Contains("origin="));
                //Contract.Requires(httpMessageBody.Contains('&'));
                //Contract.Requires(httpMessageBody.IndexOf("origin=") < httpMessageBody.IndexOf('&'));
                //Contract.Ensures(Contract.Result<string>() != null);
                //int pkiIdStart = httpMessageBody.IndexOf("origin=") + "origin=".Length;
                //int pkiIdEnd = httpMessageBody.IndexOf('&', pkiIdStart); // pkiId ends at next &
                //String encPkiId = httpMessageBody.Substring(pkiIdStart, pkiIdEnd - pkiIdStart); // Get the pkiId substring
                //String decPkiId = Cryptograph.Decrypt(encPkiId,  /* Third Party Private Key */); // TODO update this according to keys + what if it fails
                //return decPkiId;
            //}

            /// <summary>
            /// Processes an incoming http POST request and takes action according to target resource and the validity of the message body.
            /// </summary>
            /// <param name="hlc">The HttpListenerContext in which the request was recieved.</param>
            private void ProcessIncomingPost(HttpListenerContext hlc)
            {
                HttpListenerResponse response = hlc.Response; // obtain response object
                string rawMessageBody = this.GetPostString(hlc);
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
                string username = inputValues[0]; // value of username is at position 0 in the input array
                string tokenValue = inputValues[1]; // value of token is at position 1 in the input array
                string urlForPost = hlc.Request.RawUrl; // Find out what subpage the data was posted to
                switch (urlForPost)
                {
                    case @"/request=loginpage":
                        this.AnswerLoginpagePost(response, username);
                        break;
                    case @"/request=authtoken":
                        this.AnswerAuthtokenPost(response, rawMessageBody, username, tokenValue);
                        break;
                    case @"/request=usertoken":
                        this.AnswerUsertokenPost(response, username, tokenValue);
                        break;
                    default:
                        // Resource either not found or not meant for posting
                        response.StatusCode = 404; // Status code: NOT FOUND
                        response.StatusDescription = "Resource does either not exist or is not accepting POST messages.";
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
                string senderPkiId = MessageProcessingUtility.GetRequesterDomain(httpMessageBody, this.serversPrivateKey);
                if (ReferenceEquals(senderPkiId, null) || !senderPkiId.Equals(expectedPkiId))
                {
                    // Origin is not determable or not equal to expected origin
                    return false;
                }
                
                // Get the actual message that was decrypted
                string actualMessage = httpMessageBody.Substring(0, httpMessageBody.LastIndexOf('&'));
                
                // Get the message signature
                string signedMessage = httpMessageBody.Substring(
                    httpMessageBody.LastIndexOf('&') + 1, httpMessageBody.Length - 1);
                
                // verify data with sender's public key
                return Cryptograph.VerifyData(actualMessage, signedMessage, Cryptograph.GetPublicKey(expectedPkiId));
            }

            // < HELPER METHODS FOR POST PROCESSING STARTS >

            /// <summary>
            /// Answers a POST to the request=loginpage resource.
            /// </summary>
            /// <param name="response">The HttpListenerResponse obtained from the HttpListenerContext that is associated with the post to the request=loginpage resource.</param>
            /// <param name="username">The username posted to the resource.</param>
            private void AnswerLoginpagePost(HttpListenerResponse response, string username)
            {
                // check username is valid in own database
                if (this.database.ContainsUsername(username))
                {
                    // redirect to NemID
                    response.Redirect(AUTH_URI + "request=redirect&userName=" + username + "&3rd=" + this.server.Prefixes.First());
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
                // check that the post came from the authenticator
                if (this.ValidatePostOrigin(rawMessageBody, AUTH_URI))
                {
                    this.database.SetAuthTokenForAccount(username, tokenValue);
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
            private void AnswerUsertokenPost(HttpListenerResponse response, string username, string tokenValue)
            {
                if (this.database.CompareTokens(tokenValue, username))
                {
                    response.StatusCode = 200; // HTTP OK status
                    response.StatusDescription = "Authentication successful.";
                    response.Close();
                }
                else
                {
                    response = this.SetupForbiddenResponse(response, "Access denied, possible timeout.");
                    response.Close();
                }
            }

            // < / HELPER METHODS FOR POST PROCESSING ENDS >
        }
    }
