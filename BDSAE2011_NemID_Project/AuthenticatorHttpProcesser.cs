// -----------------------------------------------------------------------
// <copyright file="AuthenticatorHttpProcesser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticatorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using ClientComponent;

    /// <summary>
    /// 
    /// </summary>
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
        /// Initializes a new instance of the ClientSession class.
        /// </summary>
        public ClientSession()
        {
            this.currentState = SessionState.AwaitSessionStart;
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
            AwaitSessionStart,

            /// <summary>
            /// After the client has submitted user name and password
            /// successfully (it has ben accepted by the authenticator)
            /// the session is in this state.
            /// </summary>
            InitialLoginAccepted,

            /// <summary>
            /// After the client has submitted an accepted key-value
            /// from their key card the session is in this state.
            /// </summary>
            KeyAccepted
        }

        /// <summary>
        /// Is the specified operation valid?
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public bool IsOperationValid(string operation)
        {
            if (this.TimedOut())
            {
                this.currentState = SessionState.AwaitSessionStart;
                return false;
            }

            // Is it legal to call the requested operation at the
            // authenticator?
            switch (operation)
            {
                case "redirect":
                    return this.currentState == SessionState.AwaitRedirection;
                case "login":
                    return this.currentState == SessionState.AwaitSessionStart;
                case "submitKey":
                    return this.currentState == SessionState.InitialLoginAccepted;
                case "proceed":
                    return this.currentState == SessionState.KeyAccepted;
                case "abort":
                    return this.currentState != SessionState.AwaitSessionStart;
                case "createAccount":
                    return this.currentState == SessionState.AwaitSessionStart;
                case "revokeAccount":
                    return this.currentState == SessionState.KeyAccepted;
                default:
                    return false;
            }
        }

        public void ChangeStateTo(SessionState state)
        {
            this.currentState = state;
        }

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
            this.timeOfLastValidRequest = DateTime.Now;

            if (this.currentState == SessionState.AwaitSessionStart)
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
    /// TODO: Update summary.
    /// </summary>
    public class AuthenticatorService
    {
        /// <summary>
        /// The authenticator service provider.
        /// </summary>
        private readonly Authenticator authenticator;

        /// <summary>
        /// Represents the server socket of the authenticator.
        /// </summary>
        private readonly AuthenticatorSocket serverSocket;

        private readonly Dictionary<string, ClientSession> userSessions;

        /// <summary>
        /// List containing string representations of the operations
        /// that is supported by the authentication service.
        /// </summary>
        private readonly List<string> supportedOperations = new List<string>()
            {
                "redirect", "login", "submitKey", "proceed", "abort", "createAccount",  "revokeAccount"
            };

        private byte[] authenticatorPrivateKey;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorHttpProcessor class.
        /// </summary>
        /// <param name="authenticatorDomain">
        /// The port the authenticator will be listening to.
        /// </param>
        public AuthenticatorService(string authenticatorDomain, byte[] authenticatorPrivateKey)
        {
            Contract.Requires(IsValidURL(authenticatorDomain));

            this.authenticator = new Authenticator();
            this.userSessions = new Dictionary<string, ClientSession>(); // TODO initialize with all usernames.
            this.serverSocket = new AuthenticatorSocket(authenticatorDomain, authenticatorPrivateKey);
            this.authenticatorPrivateKey = authenticatorPrivateKey;
        }

        /// <summary>
        /// Starts the authenticator service.
        /// Will not terminate. // TODO good enough?
        /// </summary>
        public void ServiceLoop()
        {
            this.serverSocket.Start();

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

                // Check if the requested operation is supported.
                if (!this.supportedOperations.Contains(processedRequest.RequestedOperation))
                {
                    // If it does not, respond to the requester with a negative response
                    Console.WriteLine("Server is responding.");
                    this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                    continue;
                }

                switch (processedRequest.RequestedOperation)
                {
                    case "redirect":
                        this.processRedirect(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "validRequest=true";
                        }

                        goto default;
                    case "login":
                        string keyIndex = this.processLogin(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "keyIndex=" + keyIndex;
                        }

                        goto default;
                    case "submitKey":
                        this.processSubmitKey(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "accepted=true";
                        }

                        goto default;
                    case "proceed":
                        string sessionToken = this.processProceed(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "token=" + sessionToken;
                        }

                        goto default;
                    case "abort":
                        this.processAbort(processedRequest, ref validRequest);

                        if (validRequest)
                        {
                            httpResponseMessageBody = "abort=true";
                        }

                        goto default;
                    case "createAccount":
                        this.processCreateAccount(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "createAccount=true";
                        }

                        goto default;
                    case "revokeAccount":
                        this.processRevokeAccount(processedRequest, ref validRequest);
                        if (validRequest)
                        {
                            httpResponseMessageBody = "revokeAccount=true";
                        }

                        goto default;
                    default:
                        Console.WriteLine("Server is responding");
                        this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a client's requested operation.
        /// </summary>
        /// <param name="processedRequest">
        /// The Request-object representing properties of the client's
        /// request.
        /// </param>
        /// <param name="validRequest">
        /// A bool reference indicating success of the client's request.
        /// </param>
        private void processRedirect(Request processedRequest, ref bool validRequest)
        {
            // Get the raw url of the request.
            string rawUrl = processedRequest.RawUrl;

            if (!this.IsRedirectUrlWellFormed(rawUrl))
            {
                validRequest = false;
                return;
            }

            // Retrieve the user name specified in the redirection
            // url.
            int start = rawUrl.IndexOf("username=") + "username=".Length;
            int end = rawUrl.IndexOf('&', start);

            string userName = rawUrl.Substring(start, end - start);

            // If the user is not in the userSession dictionary, the user
            // is not in the authenticator database.
            if (!this.userSessions.ContainsKey(userName))
            {
                validRequest = false;
                return;
            }

            ClientSession userSession = this.userSessions[userName];

            // The submitted operation is not valid.
            if (!userSession.IsOperationValid("redirect"))
            {
                validRequest = false;
                return;
            }

            // Retrieve the third party domain in the raw url.
            start = rawUrl.LastIndexOf("3rd=") + "3rd=".Length;
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
            userSession.ChangeStateTo(ClientSession.SessionState.AwaitSessionStart);
        }

        private string processLogin(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];
            string password = processedRequest.Parameters[1];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("login");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...check if the submitted key is valid.
                validRequest = this.authenticator.IsLoginValid(userName, password);

                // Key index which corresponding key must be submitted by the client.
                string keyIndex = string.Empty;

                if (validRequest)
                {
                    userSession.ChangeStateTo(ClientSession.SessionState.InitialLoginAccepted);
                    return this.authenticator.GetKeyIndex(userName);
                }
            }

            validRequest = false;

            return null;
        }

        private void processSubmitKey(Request processedRequest, ref bool validRequest)
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

        private string processProceed(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[1];

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
                ClientSocket thirdPartyClient = new ClientSocket(                   // TODO right 2nd parameter?
                    userSession.ThirdPartyDomain, "authenticator", this.authenticatorPrivateKey);

                thirdPartyClient.SendMessage("authtoken", "username=" + userName + "&token=" + sessionToken);

                return sessionToken;
                // TODO Call read to send complete the send?
            }

            validRequest = false;

            return null;
        }

        private void processAbort(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[1];

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

        private void processCreateAccount(Request processedRequest, ref bool validRequest)
        {
            // The parameters for the requested operation.
            string userName = processedRequest.Parameters[0];
            string password = processedRequest.Parameters[1];
            string cprNumber = processedRequest.Parameters[2];

            // Check if it is legal to call this operation
            ClientSession userSession = this.userSessions[userName];
            bool validOperation = userSession.IsOperationValid("createAccount");

            // If the requested operation is valid...
            if (validOperation)
            {
                // ...check if the request is valid.
                validRequest = this.authenticator.AddNewUser(userName, password, cprNumber);

                if (validRequest)
                {
                    // Update the state of the client session.
                    userSession.ChangeStateTo(ClientSession.SessionState.AwaitRedirection);

                    // The new account must be added to client sessions.
                    this.userSessions.Add(userName, new ClientSession());
                    return;
                }
            }

            validRequest = false;
        }

        private void processRevokeAccount(Request processedRequest, ref bool validRequest)
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
        /// Source: http://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
        /// </summary>
        /// <param name="URL">
        /// Stirng representation of the URL.
        /// </param>
        /// <returns>
        /// True if it is a valid URL, false otherwise.
        /// </returns>
        [Pure]
        public static bool IsValidURL(string URL)
        {
            Uri uri = new Uri(URL);
            return Uri.TryCreate(URL, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeHttp;
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
            isValid = start + 6 < end;

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
