using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    // -----------------------------------------------------------------------
    // <copyright file="ThirdPartyServer.cs" company="">
    // TODO: Update copyright text.
    // </copyright>
    // -----------------------------------------------------------------------

    namespace ThirdPartyComponent
    {
        using System;
        using System.Collections;
        using System.Collections.Specialized;
        using System.Diagnostics.Contracts;
        using System.IO;
        using System.Net;

        using Miscellaneoues;

        /// <summary>
        /// Class running the third party server.
        /// </summary>
        public class ThirdPartyServer
        {
            public static void Main(string[] args)
            {
                // setup a server
                var tpserver = new ThirdPartyServer("localhost", 8082);
                tpserver.RunServer();
            }

            /// <summary>
            /// The server that listens for requests.
            /// </summary>
            private HttpListener server;

            /// <summary>
            /// A collection with addresses for sub pages where data posting is permitted.
            /// </summary>
            private StringCollection subpagesForPost = new StringCollection();

            /// <summary>
            /// The URI of the trusted authenticator.
            /// </summary>
            private static readonly string AUTH_URI = "https://localhost:8081"; // TODO agree on this address

            /// <summary>
            /// Represents the ThirdParty backend database
            /// </summary>
            private ThirdParty database = new ThirdParty();

            /// <summary>
            /// Initializes a new instance of the ThirdParty Server class.
            /// Protocol cannot be specified since a server of this type should always use https.
            /// </summary>
            /// <param name="serverDomain">The domain the server should run on, fx "localhost"</param>
            /// <param name="serverPort">The port the server should listen to.</param>
            public ThirdPartyServer(string serverDomain, uint serverPort)
            {
                this.server = new HttpListener();
                string serverAddress = @"http://" + serverDomain + ":" + serverPort + @"/"; // TODO update to https after testing!!!
                this.server.Prefixes.Add(serverAddress);
                subpagesForPost.AddRange(new string[] { @"/loginpage", @"/usertoken", @"/authtoken" });
            }


            /// <summary>
            /// Start the server and listen for requests.
            /// </summary>
            public void RunServer()
            {
                this.server.Start(); // start the server

                while (true) // TODO run forever?
                {
                    HttpListenerContext hlc = this.server.GetContext(); // blocking call
                    HttpListenerRequest request = hlc.Request;
                    Console.WriteLine(
                        "SSL connection between 3rd Party and Client established: "
                        + request.IsSecureConnection);
                    Console.WriteLine("<ThirdPartyServer>: User authenticated: " + request.IsAuthenticated);
                    string requestMethod = request.HttpMethod;
                    switch (requestMethod) // Let request method specify action
                    {
                        case "GET":
                            this.InvokeGetRequest(hlc);
                            break;
                        case "POST":
                            string postData = this.GetPostString(hlc); // read the posted input
                            if (ReferenceEquals(postData, null))
                            {
                                // not a valid request, do nothing
                                break;
                            }
                            else
                            {
                                this.ProcessIncomingPost(hlc);
                                // Process the post and redirect user to nem id
                                // TODO ...read the posted data here...
                                string senderPkiId = this.GetSenderPkiIdFromHttpMessage(postData);
                                // Finally redirect the user to NemID
                                //response.Redirect("https://localhost:8081"); // TODO need to agree on this URI (NemID's)
                                break;
                            }

                        default:
                            // Reply with that an error has occured: http method was not supported
                            HttpListenerResponse response = hlc.Response;
                            response.StatusCode = 501; // method not supported
                            response.StatusDescription = "HTTP method not supported by server";
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
                if (ReferenceEquals(requestedUrl, null)) // guard against null ref if unspecified get string
                {
                    requestedUrl = string.Empty;
                }

                requestedUrl = requestedUrl.Trim(); // remove white space
                Console.WriteLine("Requested RawURL: " + requestedUrl);
                string responseStr = "";
                switch (requestedUrl)
                {
                    case (@"/loginpage"):
                        // respond with loginpage
                        responseStr =
                            "<html><head><title>Login</title></head>" +
                            "<body>Login page html goes here</body></html>";
                        break;
                    // no other sub pages designed for get requests
                    default:
                        // direct user to main page
                        response.StatusCode = 404; // NOT FOUND status code
                        response.Redirect(server.Prefixes.First());
                        return;
                }
                // write response...
                Stream output = response.OutputStream;
                byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
                output.Write(buffer, 0, buffer.Length);
                response.Close();
            }

            /// <summary>
            /// Extract the message body of a POST request.
            /// </summary>
            /// <param name="hlc">The context in which the request was recieved.</param>
            /// <returns>The message body as an UTF8 encoded string.</returns>
            private String GetPostString(HttpListenerContext hlc)
            {
                HttpListenerRequest request = hlc.Request;
                HttpListenerResponse response = hlc.Response;
                string requestedUrl = request.RawUrl;
                if (!subpagesForPost.Contains(requestedUrl)) // if requested url is not part of subpagesForPost, posting is not allowed!
                {
                    response.StatusCode = 405; // method not allowed
                    response.Headers.Add(HttpRequestHeader.Allow, "GET"); // inform that only GET is allowed for any page not in supagesForPost
                    response.Close(); // send response
                    return null; // not a valid request
                }
                // Handle valid post to login page
                Stream input = request.InputStream; // access inputstream
                byte[] buf = new byte[input.Length]; // TODO Stream.Length is a long - possible int overflow here
                input.Read(buf, 0, buf.Length);
                string postedData = System.Text.Encoding.UTF8.GetString(buf);
                return postedData;
            }

            /// <summary>
            /// Gets the decrypted PKI identifier of the sender.
            /// </summary>
            /// <param name="httpMessageBody">The decrypted content of a http message.</param>
            /// <returns>Gets the decrypted PKI identifier of the sender.</returns>
            private String GetSenderPkiIdFromHttpMessage(string httpMessageBody)
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(httpMessageBody));
                Contract.Requires(httpMessageBody.Contains("origin="));
                Contract.Requires(httpMessageBody.Contains('&'));
                Contract.Requires(httpMessageBody.IndexOf("origin=") < httpMessageBody.IndexOf('&'));
                Contract.Ensures(Contract.Result<string>() != null);
                int pkiIdStart = httpMessageBody.IndexOf("origin=") + "origin=".Length;
                int pkiIdEnd = httpMessageBody.IndexOf('&', pkiIdStart); // pkiId ends at next &
                String encPkiId = httpMessageBody.Substring(pkiIdStart, pkiIdEnd - pkiIdStart); // Get the pkiId substring
                String decPkiId = Cryptograph.Decrypt(encPkiId,  /* Third Party Private Key */); // TODO update this according to keys + what if it fails
                return decPkiId;
            }

            private void ProcessIncomingPost(HttpListenerContext hlc)
            {
                HttpListenerResponse response = hlc.Response; // obtain response object
                if (!this.ValidatePostOrigin(this.GetPostString(hlc), this.GetSenderPkiIdFromHttpMessage(this.GetPostString(hlc))))
                {
                    // message was not valid - message has been tampered with, end request here
                    response = this.SetupForbiddenResponse(response,
                        "The server recognized an attempt to modify your message, the transaction was aborted.");
                    response.Close();
                    return;
                }
                HttpListenerRequest request = hlc.Request;
                String urlForPost = request.RawUrl; // Find out what subpage the data was posted to
                switch (urlForPost)
                {
                    case (@"/loginpage"):
                        // read and store username
                        // redirect to NemID
                        break;
                    case (@"/authtoken"):
                        // check that the post came from the authenticator
                        if (this.ValidatePostOrigin(this.GetPostString(hlc), AUTH_URI))
                        {
                            database.SetAuthTokenForAccount(/* TODO extract username here */, /* TODO extract token here*/ );
                        }
                        else
                        {
                            // Forbidden for anyone else than the authenticator
                            response = this.SetupForbiddenResponse(
                                response, "You do not have rights to post to this resource.");
                            response.Close();
                        }
                        break;
                    case (@"/usertoken"):
                    // check that the post came from the user (already done above)
                        if(database.CompareTokens(/* TODO extract client token here */, /* TODO extract username here */))
                        {
                            response.StatusCode = 200; // HTTP OK status
                            response.StatusDescription = "Authentication successful.";
                            // TODO What to send back? Html?
                            response.Close();
                        }
                        else
                        {
                            response = this.SetupForbiddenResponse(response, "Access denied, possible timeout.");
                            response.Close();
                        }
                        break;
                    default:
                    // TODO handle error
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
                String senderPkiId = this.GetSenderPkiIdFromHttpMessage(httpMessageBody);
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
                // verify data with senders' public key
                return Cryptograph.VerifyData(actualMessage, signedMessage, Cryptograph.GetPublicKey(expectedPkiId));
            }
        }
    }
