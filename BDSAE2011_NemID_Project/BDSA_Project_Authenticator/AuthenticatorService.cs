// -----------------------------------------------------------------------
// <copyright file="AuthenticatorHttpProcesser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using BDSA_Project_Communication;

    /// <summary>
    /// Records a client session making sure that the authenticator operations
    /// are requested in the correct order.
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann.
    /// </author>
    internal class ClientSession
    {
        /// <summary>
        /// The domain of the third party that this session
        /// is linked to.
        /// </summary>
        private string thirdPartyDomain;

        /// <summary>
        /// The current state of the 
        /// </summary>
        private SessionState currentState;

        /// <summary>
        /// Time of the last valid request submitted by the client.
        /// </summary>
        private DateTime timeOfLastValidRequest;

        /// <summary>
        /// Counts how many attempts has
        /// </summary>
        private int numberOfAttempts = 0;

        /// <summary>
        /// Initializes a new instance of the ClientSession class.
        /// </summary>
        public ClientSession()
        {
            this.currentState = SessionState.AwaitRedirection;
            this.timeOfLastValidRequest = DateTime.Now;
        }

        /// <summary>
        /// The states that a client session can be in.
        /// </summary>
        public enum SessionState
        {
            /// <summary>
            /// The server wait for the client to be redirected from the
            /// third party to the authenticator log-in screen.
            /// </summary>
            AwaitRedirection,

            /// <summary>
            /// The server waits for the client to request the 
            /// login operation, where they submit user name and
            /// password
            /// </summary>
            AwaitLogin,

            /// <summary>
            /// After the client has submitted user name and password
            /// successfully (it has ben accepted by the authenticator)
            /// the session is awaiting the submission of a key from the 
            /// user's keycard.
            /// </summary>
            InitialLoginAccepted,

            /// <summary>
            /// After the client has submitted an accepted key-value
            /// from their key card the session is in this state.
            /// </summary>
            KeyAccepted
        }

        /// <summary>
        /// Is this operation valid to request?
        /// </summary>
        /// <param name="operation">
        /// String represention of the operation.
        /// </param>
        /// <returns>
        /// True if the operation is valid, false otherwise.
        /// </returns>
        public bool IsOperationValid(string operation)
        {
            if (operation.Equals("abort"))
            {
                return true;
            }

            if (this.currentState == SessionState.AwaitRedirection)
            {
                return true;
            }

            // If the session is awaiting either login and key submission...
            if (this.currentState == SessionState.AwaitLogin ||
                this.currentState == SessionState.InitialLoginAccepted)
            {
                // ... and the user unsuccesfully has submitted 3 requests
                // when in either of these states...
                if (++this.numberOfAttempts > 3)
                {
                    // ...the operation is not valid.
                    return false;
                }
            }

            if (this.TimedOut())
            {
                //this.currentState = SessionState.AwaitRedirection;
                return false;
            }

            // Is it legal to call the requested operation at the
            // authenticator?
            switch (operation)
            {
                case "redirect":
                    return this.currentState == SessionState.AwaitRedirection;
                case "login":
                    return this.currentState == SessionState.AwaitLogin;
                case "submitKey":
                    return this.currentState == SessionState.InitialLoginAccepted;
                case "proceed":
                    return this.currentState == SessionState.KeyAccepted;
                case "abort":
                    return this.currentState != SessionState.AwaitRedirection;
                case "createAccount":
                    return this.currentState == SessionState.AwaitLogin;
                case "revokeAccount":
                    return this.currentState == SessionState.KeyAccepted;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Change te current state of the session to the 
        /// specified state.
        /// </summary>
        /// <param name="state">
        /// The state that the session is to change to.
        /// </param>
        public void ChangeStateTo(SessionState state)
        {
            // Reset number of attempts when the user session is changed.
            this.numberOfAttempts = 0;
            this.timeOfLastValidRequest = DateTime.Now;

            this.currentState = state;
        }

        /// <summary>
        /// Gets or sets the third party domain that the user is currently
        /// trying to get authenticated for.
        /// What third party is the client session currently linked to?
        /// </summary>
        public string ThirdPartyDomain
        {
            get
            {
                return this.thirdPartyDomain;
            }

            set
            {
                this.thirdPartyDomain = value;
            }
        }

        /// <summary>
        /// Helper method used to determine if the user has taken too long
        /// to sign in.
        /// If the user is in the middle of a signing in-process the user has
        /// 1 minute to finish it before the session is timed out.
        /// </summary>
        /// <returns>
        /// True if the client session i timed out, false otherwise.
        /// </returns>
        private bool TimedOut()
        {
            bool timedOut = this.timeOfLastValidRequest.AddMinutes(1) <= DateTime.Now;
            //this.timeOfLastValidRequest = DateTime.Now;

            if (this.currentState == SessionState.AwaitRedirection)
            {
                return false;
            }

            // Al other states
            if (timedOut)
            {
                return true;
            }

            // No time out.
            return false;
        }
    }

    /// <summary>
    /// Handles the requests sent from authentiator service clients."
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann
    /// </author>
    public class AuthenticatorService
    {
        /// <summary>
        /// The authenticator service provider.
        /// </summary>
        private readonly Authenticator authenticator;

        /// <summary>
        /// Represents the server socket of the authenticator.
        /// </summary>
        private readonly AuthenticatorServer serverSocket;

        /// <summary>
        /// All registered authenticator users and their sessions.
        /// </summary>
        private readonly Dictionary<string, ClientSession> userSessions;

        /// <summary>
        /// List containing string representations of the operations
        /// that is supported by the authentication service.
        /// </summary>
        private readonly List<string> supportedOperations = new List<string>()
            {
                "redirect", "login", "submitKey", "proceed", "abort", "createAccount",  "revokeAccount"
            };

        /// <summary>
        /// The private key of the authenticator.
        /// </summary>
        private readonly byte[] authenticatorPrivateKey;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorService class.
        /// </summary>
        /// <param name="authenticatorDomain">
        /// The url of the new service instance.
        /// </param>
        /// <param name="authenticatorPrivateKey">
        /// The private key of the authenticator.
        /// </param>
        public AuthenticatorService(string authenticatorDomain, byte[] authenticatorPrivateKey)
        {
            Contract.Requires(MessageProcessingUtility.IsValidUrl(authenticatorDomain));
            Contract.Requires(authenticatorPrivateKey != null);

            this.authenticator = new Authenticator();
            this.userSessions = new Dictionary<string, ClientSession>();

            // Initialize the clientSessions with all the clients current
            // registered in the authenticator database.
            foreach (string userName in this.authenticator.GetAllUsers())
            {
                this.userSessions.Add(userName, new ClientSession());
            }

            this.serverSocket = new AuthenticatorServer(authenticatorDomain, authenticatorPrivateKey);
            this.authenticatorPrivateKey = authenticatorPrivateKey;
        }

        /// <summary>
        /// Starts the authenticator service.
        /// Will not terminate. // TODO good enough?
        /// </summary>
        public void ServiceLoop()
        {
            this.serverSocket.Start();

            // The server never has to terminate.
            while (true)
            {
                Request processedRequest = this.serverSocket.ReadMessage();

                Console.WriteLine("Server received resquest from: " + processedRequest.RequesterDomain);

                // Is the request valid?
                bool validRequest = false;

                // The contents of the message body in the reponse http message
                // sent to the client. Must contain a non-empty string if the request
                // is valid. If it is invalid, can be empty.
                string httpResponseMessageBody = string.Empty;

                //// If the parameters is null...
                //if (processedRequest.Parameters == null)
                //{
                //    // ... the request in invalid.
                //    Console.WriteLine("Client request processed with success: " + validRequest);
                //    Console.WriteLine("Server is responding to: " + processedRequest.RequesterDomain);
                //    this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                //    continue;
                //}

                // Check if the requested operation is supported.
                if (!this.supportedOperations.Contains(processedRequest.RequestedOperation))
                {
                    // If it does not, respond to the requester with a negative response
                    Console.WriteLine("Client request processed with success: " + validRequest);
                    Console.WriteLine("Server is responding to: " + processedRequest.RequesterDomain);
                    this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                    continue;
                }

                switch (processedRequest.RequestedOperation)
                {
                    case "redirect":
                        this.ProcessRedirect(processedRequest, ref validRequest);
                        Console.WriteLine("Client request processed with success: " + validRequest);
                        Console.WriteLine("Server is responding to: " + processedRequest.RequesterDomain);
                        this.serverSocket.SendMessage(processedRequest, validRequest);
                        break;
                    case "login":
                        string keyIndex = this.ProcessLogin(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "keyIndex=" + keyIndex : string.Empty;
                        goto default;
                    case "submitKey":
                        this.ProcessSubmitKey(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "accepted=true" : string.Empty;
                        goto default;
                    case "proceed":
                        string sessionToken = this.ProcessProceed(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "token=" + sessionToken : string.Empty;
                        goto default;
                    case "abort":
                        this.ProcessAbort(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "abort=true" : string.Empty;
                        goto default;
                    case "createAccount":
                        this.ProcessCreateAccount(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "createAccount=true" : string.Empty;
                        goto default;
                    case "revokeAccount":
                        this.ProcessRevokeAccount(processedRequest, ref validRequest);
                        httpResponseMessageBody = validRequest ? "revokeAccount=true" : string.Empty;
                        goto default;
                    default:
                        Console.WriteLine("Client request processed with success: " + validRequest);
                        Console.WriteLine("Server is responding to: " + processedRequest.RequesterDomain);
                        this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a client's requested redirect-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating success of the client's request.
        /// </param>
        private void ProcessRedirect(Request processedRequest, ref bool validRequest)
        {
            // Get the raw url of the request.
            string rawUrl = processedRequest.RawUrl;

            Console.WriteLine("RAWURL: " + rawUrl);

            // If the redirected url is not well formed...
            if (!this.IsRedirectUrlWellFormed(rawUrl))
            {
                // ...the request is not valid.
                validRequest = false;
                return;
            }

            // Retrieve the user name specified in the redirection
            // url.
            int start = rawUrl.IndexOf("username=") + "username=".Length;   // TODO Validated
            int end = rawUrl.IndexOf('&', start);

            string userName = rawUrl.Substring(start, end - start);

            Console.WriteLine("USERNAME AT AUTHENTCATOR: " + userName);

            // If the user is not in the userSession dictionary, the user
            // is not in the authenticator database: Invalid request.
            if (!this.userSessions.ContainsKey(userName))
            {
                validRequest = false;
                return;
            }

            ClientSession userSession = this.userSessions[userName];

            // If the requeseted operation is not valid...
            if (!userSession.IsOperationValid("redirect"))
            {
                // ...the request is not valid.
                validRequest = false;
                return;
            }


            // Retrieve the third party domain in the raw url.
            start = rawUrl.LastIndexOf("3rd=") + "3rd=".Length;         // TODO validated
            end = rawUrl.Length;

            // Update the client session with the new third party url.
            string thirdParty = rawUrl.Substring(start, end - start);
            userSession.ThirdPartyDomain = thirdParty;

            // If this third party domain is not trusted by the authenticator...
            if (!this.authenticator.IsThisThirdPartyTrusted(thirdParty))
            {
                // ...then send a negative response back to the client
                validRequest = false;
                return;
            }

            validRequest = true;

            // Update the client session state.
            userSession.ChangeStateTo(ClientSession.SessionState.AwaitLogin);
        }

        /// <summary>
        /// Processes a client's requested login-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating success of the client's request.
        /// </param>
        /// <returns>
        /// Returns null if the request was not accepted by the authenticator.
        /// </returns>
        private string ProcessLogin(Request processedRequest, ref bool validRequest)
        {
            Contract.Requires(processedRequest.Parameters != null);

            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];
            string password = processedRequest.Parameters[1];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("login");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...check if the submitted user name and password are valid.
                validRequest = this.authenticator.IsLoginValid(userName, password);
                if (validRequest)
                {
                    // The request is valid, update the user's session...
                    userSession.ChangeStateTo(ClientSession.SessionState.InitialLoginAccepted);

                    // ...and get the key index which corresponding key 
                    // must be submitted by the client next.
                    return this.authenticator.GetKeyIndex(userName);
                }
            }

            validRequest = false;
            return null;
        }

        /// <summary>
        /// Processes a client's requested submitKey-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating succss of the client's request.
        /// </param>
        private void ProcessSubmitKey(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string key = processedRequest.Parameters[0];
            string userName = processedRequest.Parameters[1];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("submitKey");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...check if the submitted key is valid.
                validRequest = this.authenticator.IsKeycardValueValid(key, userName);

                // If the key is valid, update the user session state.
                if (validRequest)
                {
                    // Update the state of the client session.
                    userSession.ChangeStateTo(ClientSession.SessionState.KeyAccepted);
                    return;
                }
            }

            validRequest = false;
        }

        /// <summary>
        /// Processes a client's requested proceed-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating succss of the client's request.
        /// </param>
        /// <returns>
        /// The shared secret between the client and the third party.
        /// </returns>
        private string ProcessProceed(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("proceed");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...the request is valid.
                validRequest = true;

                // Update the state of the client session.
                userSession.ChangeStateTo(ClientSession.SessionState.AwaitRedirection);

                // Generate session token for client and third party.
                string sessionToken = this.GenerateToken();

                // Send session token to third party:
                ClientSocket thirdPartyClient = new ClientSocket(
                    userSession.ThirdPartyDomain, StringData.AuthUri, this.authenticatorPrivateKey);

                thirdPartyClient.SendMessage("authtoken", "username=" + userName + "&token=" + sessionToken);

                return sessionToken;

            }

            validRequest = false;

            return null;
        }

        /// <summary>
        /// Processes a client's requested abort-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating succss of the client's request.
        /// </param>
        private void ProcessAbort(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("abort");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...the request is valid.
                validRequest = true;

                // Update the state of the client session.
                userSession.ChangeStateTo(ClientSession.SessionState.AwaitRedirection);
                return;
            }

            validRequest = false;
        }

        /// <summary>
        /// Processes a client's requested submitKey-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating succss of the client's request.
        /// </param>
        private void ProcessCreateAccount(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];
            string password = processedRequest.Parameters[1];
            string cprNumber = processedRequest.Parameters[2];
            string email = processedRequest.Parameters[3];

            // Check if it is legal to call this operation
            // ClientSession userSession = this.userSessions[userName];
            // bool validOperation = userSession.IsOperationValid("createAccount");

            // If the requested operation is valid...
            // if (validOperation)
            //{
            // ...check if the request is valid.
            validRequest = this.authenticator.AddNewUser(userName, password, cprNumber, email);

            if (validRequest)
            {
                // Update the state of the client session.
                // userSession.ChangeStateTo(ClientSession.SessionState.AwaitRedirection);

                // The new account must be added to client sessions.
                this.userSessions.Add(userName, new ClientSession());

                // Multicast new user account to trusted third parties.
                foreach (string trustedThirdPartyURI in this.authenticator.TrustedThirdPartyURIs)
                {
                    ClientSocket socket = new ClientSocket(trustedThirdPartyURI, StringData.AuthUri, this.authenticatorPrivateKey);
                    socket.SendMessage("newuseradded", "userName=" + userName);
                }

                return;
                //}
            }

            validRequest = false;
        }

        /// <summary>
        /// Processes a client's requested recokeAccount-operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-struct representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating succss of the client's request.
        /// </param>
        private void ProcessRevokeAccount(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("revokeAccount");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...check if the request is valid.
                validRequest = this.authenticator.DeleteUser(userName);

                if (validRequest)
                {
                    // The user has been deleted from the authenticator database, and must
                    // also be deleted from the client sessions.
                    this.userSessions.Remove(userName);
                    // Multicast new user account to trusted third parties.
                    foreach (string trustedThirdPartyURI in this.authenticator.TrustedThirdPartyURIs)
                    {
                        ClientSocket socket = new ClientSocket(trustedThirdPartyURI, StringData.AuthUri, this.authenticatorPrivateKey);
                        socket.SendMessage("userdeleted", "userName=" + userName);
                    }
                    return;
                }
            }

            validRequest = false;
        }

        /// <summary>
        /// Generates the shared secret between the client and the
        /// third party used for establishing a validated connection.
        /// </summary>
        /// <returns>
        /// String representation of the generated token.
        /// </returns>
        private string GenerateToken()
        {
            return new Random().Next(1000, 9999).ToString();
        }

        /// <summary>
        /// Determines if the given url is a well formed
        /// redirection url.
        /// </summary>
        /// <param name="url">
        /// string url to be queried.
        /// </param>
        /// <returns>
        /// True if the redirection url is well formed, false
        /// otherwise.
        /// </returns>
        [Pure]
        private bool IsRedirectUrlWellFormed(string url)
        {
            bool isValid = false;

            isValid = url.Contains("request=redirect");
            isValid = isValid && url.Contains("username=");
            isValid = isValid && url.Contains("3rd=");

            if (!isValid)
            {
                return false;
            }

            isValid = url.Count(c => c.Equals('&')) == 2;

            if (!isValid)
            {
                return false;
            }

            int start = url.IndexOf("username=") + "username=".Length;
            int end = url.IndexOf('&', start) - 1;

            // Minimum 6 character user name string TODO agreed?
            isValid = start < end;                  // TODO Said start + 6 < end !!!!!!!!!

            if (!isValid)
            {
                return false;
            }

            start = url.IndexOf("3rd=") + "3rd=".Length;
            end = url.Length;

            return end > start;
        }
    }
}
