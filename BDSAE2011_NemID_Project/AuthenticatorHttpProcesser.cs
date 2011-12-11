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
                "login", "submitKey", "proceed", "abort", "createAccount",  "revokeAccount"
            };

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

                // Check if the requested operation is supported.
                if (!this.supportedOperations.Contains(processedRequest.RequestedOperation))
                {
                    // If it does not, respond to the requester with a negative response
                    Console.WriteLine("Server is responding.");
                    this.serverSocket.SendMessage(processedRequest, false, string.Empty);
                    continue;
                }

                bool validRequest = false;
                string httpResponseMessageBody = string.Empty;
                switch (processedRequest.RequestedOperation)
                {
                    case "redirect":
                    // is the third party trusted?
                    // this.userSessions.ContainsKey()
                    case "login":
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

                            // Retrieve the third party domain in the raw url:
                            int start = processedRequest.RawUrl.LastIndexOf('&') + 1;
                            int end = processedRequest.RawUrl.Length;

                            // Update the client session with the new third party url.
                            string thirdParty = processedRequest.RawUrl.Substring(start, end - start);
                            userSession.ThirdPartyDomain = thirdParty;

                            // Key index which corresponding key must be submitted by the client.
                            string keyIndex = string.Empty;

                            if (validRequest)
                            {
                                keyIndex = this.authenticator.GetKeyIndex(userName);
                                userSession.ChangeStateTo(ClientSession.SessionState.InitialLoginAccepted);
                            }

                            httpResponseMessageBody = validRequest ? "keyIndex=" + keyIndex : string.Empty;
                            goto default;
                        }

                        goto default;
                    case "submitKey":
                        // The parameters for the requested operation.
                        string key = processedRequest.Parameters[0];
                        string userName1 = processedRequest.Parameters[1];

                        // Check if it is legal to call this operation
                        ClientSession userSession1 = this.userSessions[userName1];
                        bool validOperation1 = userSession1.IsOperationValid("submitKey");

                        // If the requested operation is valid...
                        if (validOperation1)
                        {
                            // ...check if the submitted key is valid.
                            validRequest = this.authenticator.IsHashValueValid(key, userName1);

                            // If the key is valid, update the user session state.
                            if (validRequest)
                            {
                                // Update the state of the client session.
                                userSession1.ChangeStateTo(ClientSession.SessionState.KeyAccepted);
                            }

                            httpResponseMessageBody = validRequest ? "accepted=true" : string.Empty;
                            goto default;
                        }

                        // The requested operation was not valid.
                        goto default;
                    case "proceed":
                        // The parameters for the requested operation.
                        string userName2 = processedRequest.Parameters[1];

                        // Check if it is legal to call this operation
                        ClientSession userSession2 = this.userSessions[userName2];
                        bool validOperation2 = userSession2.IsOperationValid("proceed");

                        // If the requested operation is valid...
                        if (validOperation2)
                        {
                            // ...the request is valid.
                            validRequest = true;

                            // Update the state of the client session.
                            userSession2.ChangeStateTo(ClientSession.SessionState.AwaitSessionStart);

                            // Generate session token for client and third party.
                            string sessionToken = this.authenticator.GenerateToken();
                            httpResponseMessageBody = "token=" + sessionToken;

                            // Send session token to third party:
                            ClientSocket thirdPartyClient =
                                new ClientSocket(userSession2.ThirdPartyDomain, "authenticator"); // TODO client identifier.
                            thirdPartyClient.SendMessage(
                                "authtoken", "username=" + userName2 + "&token=" + sessionToken);

                            // TODO Call read to send complete the send?

                            goto default;
                        }

                        // The requested operation was not valid.
                        goto default;
                    case "abort":
                        // The parameters for the requested operation.
                        string userName3 = processedRequest.Parameters[1];

                        // Check if it is legal to call this operation
                        ClientSession userSession3 = this.userSessions[userName3];
                        bool validOperation3 = userSession3.IsOperationValid("abort");

                        // If the requested operation is valid...
                        if (validOperation3)
                        {
                            // ...the request is valid.
                            validRequest = true;

                            // Update the state of the client session.
                            userSession3.ChangeStateTo(ClientSession.SessionState.AwaitSessionStart);

                            httpResponseMessageBody = "abort=true";

                            goto default;
                        }

                        // The requested operation was not valid.
                        goto default;
                    case "createAccount":
                        // The parameters for the requested operation.
                        string userName4 = processedRequest.Parameters[0];
                        string password4 = processedRequest.Parameters[1];
                        string cprNumber4 = processedRequest.Parameters[2];

                        // Check if it is legal to call this operation
                        ClientSession userSession4 = this.userSessions[userName4];
                        bool validOperation4 = userSession4.IsOperationValid("createAccount");

                        // If the requested operation is valid...
                        if (validOperation4)
                        {
                            // ...check if the request is valid.
                            validRequest = this.authenticator.AddNewUser(userName4, password4, cprNumber4);

                            if (validRequest)
                            {
                                // Update the state of the client session.
                                userSession4.ChangeStateTo(ClientSession.SessionState.AwaitSessionStart);

                                // The new account must be added to client sessions.
                                this.userSessions.Add(userName4, new ClientSession());
                                httpResponseMessageBody = "createAccount=true";
                                goto default;
                            }

                            goto default;
                        }

                        // The requested operation was not valid.
                        goto default;
                    case "revokeAccount":
                        // The parameters for the requested operation.
                        string userName5 = processedRequest.Parameters[0];

                        // Check if it is legal to call this operation
                        ClientSession userSession5 = this.userSessions[userName5];
                        bool validOperation5 = userSession5.IsOperationValid("submit");

                        // If the requested operation is valid...
                        if (validOperation5)
                        {
                            // ...check if the request is valid.
                            validRequest = this.authenticator.DeleteUser(userName5);

                            if (validRequest)
                            {
                                // Update the state of the client session.
                                userSession5.ChangeStateTo(ClientSession.SessionState.AwaitSessionStart);

                                // The user has been deleted from the authenticator database, and must
                                // also be deleted from the client sessions.
                                this.userSessions.Remove(userName5);
                                httpResponseMessageBody = "revokeAccount=true";
                                goto default;
                            }

                            goto default;
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
    }
}
