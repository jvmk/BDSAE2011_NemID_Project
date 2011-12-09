// -----------------------------------------------------------------------
// <copyright file="AuthenticatorHttpProcesser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Session
    {
        /// <summary>
        /// The server waits for the client to request the 
        /// login operation, where they submit user name and
        /// password
        /// </summary>
        public const int AWAIT_SESSION_START = 0;

        /// <summary>
        /// After the client has submitted user name and password
        /// successfully (it has ben accepted by the authenticator)
        /// the session is in this state.
        /// </summary>
        public const int INITIAL_LOGIN_ACCEPTED = 1;

        /// <summary>
        /// After the client has submitted an accepted key-value
        /// from their key card the session is in this state.
        /// </summary>
        public const int KEY_ACCEPTED = 2;

        private int currentState;

        private DateTime timeOfLastUpdate = default(DateTime);

        public Session()
        {
            this.currentState = AWAIT_SESSION_START;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public bool isOperationValid(string operation)
        {
            if (this.TimedOut())
            {
                this.currentState = AWAIT_SESSION_START;
                return false;
            }

            // Is it legal to call the requested operation at the
            // authenticator?
            bool isValid = false;

            // Determine if the operation is valid.
            switch (operation)
            {
                case "login":
                    isValid = this.currentState == AWAIT_SESSION_START;
                    break;
                case "submitKey":
                    isValid = this.currentState == INITIAL_LOGIN_ACCEPTED;
                    break;
                case "createAccount":
                    isValid = this.currentState == AWAIT_SESSION_START;
                    break;
                case "revokeAccount":
                    isValid = this.currentState == KEY_ACCEPTED;
                    break;
            }

            return isValid;
        }

        public void ChangeStateTo(int state)
        {
            this.currentState = state;
        }

        /// <summary>
        /// Helper method used to determine if the user has taken too long
        /// to sign in.
        /// If the user is in the middle of a signing in-process the user has
        /// 1 minute to finish it before the session is timed out.
        /// </summary>
        /// <returns></returns>
        private bool TimedOut()
        {
            bool timedOut = this.timeOfLastUpdate.AddMinutes(1) <= DateTime.Now;
            this.timeOfLastUpdate = DateTime.Now;

            if (this.currentState == AWAIT_SESSION_START)
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
        //private readonly Authenticator authenticator;

        /// <summary>
        /// Represents the server socket of the authenticator.
        /// </summary>
        private readonly AuthenticatorSocket serverSocket;

        private readonly Dictionary<string, Session> userSessions;

        /// <summary>
        /// Indicates whether the service must keep running.
        /// </summary>
        private bool inService = true;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorHttpProcessor class.
        /// </summary>
        /// <param name="authenticatorPort">
        /// The port the authenticator will be listening to.
        /// </param>
        public AuthenticatorService(string authenticatorDomain)
        {
            //this.authenticator = new Authenticator();
            this.userSessions = new Dictionary<string, Session>();
            this.serverSocket = new AuthenticatorSocket(authenticatorDomain);
        }

        /// <summary>
        /// Starts the authenticator service.
        /// </summary>
        public void ServiceLoop()
        {
            this.serverSocket.Start();

            while (this.inService)
            {
                Request processedRequest = this.serverSocket.ReadMessage();

                bool validRequest = false;
                string httpResponseMessageBody = string.Empty;
                switch (processedRequest.RequestedOperation)
                {
                    case "login":
                        string userName = processedRequest.Parameters[0];
                        string password = processedRequest.Parameters[1];

                        // Check if it is legal to call this operation
                        Session userSession = this.userSessions[userName];
                        bool validOperation = userSession.isOperationValid("login");

                        if (validOperation)
                        {
                            // Is the submitted parameters valid?
                            validRequest = true;//this.authenticator.IsLoginValid(
                            //encUserName, encPassword);

                            string keyIndex = string.Empty;

                            if (validRequest)
                            {
                                keyIndex = 304 + ""; //this.authenticator.GetKeyIndex(encUserName);
                                userSession.ChangeStateTo(Session.INITIAL_LOGIN_ACCEPTED);
                            }

                            httpResponseMessageBody = validRequest ? "keyIndex=" + keyIndex : string.Empty;
                            goto default;
                        }

                        httpResponseMessageBody = string.Empty;
                        goto default;
                    case "submitKey":
                        string key = processedRequest.Parameters[0];
                        string userName1 = processedRequest.Parameters[1];

                        // Check if it is legal to call this operation
                        Session userSession1 = this.userSessions[userName1];
                        bool validOperation1 = userSession1.isOperationValid("submitKey");

                        if (validOperation1)
                        {
                            validRequest = true;//this.authenticator.IsHashValueValid(encKey, encUserName1);

                            // If the request is valid, update the user session state.
                            if (validRequest)
                            {
                                userSession1.ChangeStateTo(Session.KEY_ACCEPTED);
                            }

                            httpResponseMessageBody = validRequest
                                                          ? "token=" // + this.authenticator.GenerateToken()
                                                          : string.Empty;
                            goto default;
                        }

                        httpResponseMessageBody = string.Empty;
                        goto default;
                    case "createAccount":
                        string userName2 = processedRequest.Parameters[0];
                        string password2 = processedRequest.Parameters[1];
                        string cprNumber = processedRequest.Parameters[2];

                        // Check if it is legal to call this operation
                        Session userSession2 = this.userSessions[userName2];
                        bool validOperation2 = userSession2.isOperationValid("createAccount");

                        if (validOperation2)
                        {
                            validRequest = true; // this.authenticator.AddNewUser(
                            // encUserName2, encPassword2, encCprNumber);

                            if (validRequest)
                            {
                                userSession2.ChangeStateTo(Session.AWAIT_SESSION_START);
                            }

                            httpResponseMessageBody = string.Empty;
                            goto default;
                        }

                        httpResponseMessageBody = string.Empty;
                        goto default;
                    case "revokeAccount":
                        string userName3 = processedRequest.Parameters[0];

                        // Check if it is legal to call this operation
                        Session userSession3 = this.userSessions[userName3];
                        bool validOperation3 = userSession3.isOperationValid("submit");

                        if (validOperation3)
                        {
                            validRequest = true;//this.authenticator.DeleteUser(encUserName3);

                            if (validRequest)
                            {
                                userSession3.ChangeStateTo(Session.AWAIT_SESSION_START);
                            }

                            httpResponseMessageBody = string.Empty;
                            goto default;
                        }

                        httpResponseMessageBody = string.Empty;
                        goto default;

                    // TODO Is this needed? The user must contact danid to do this, as
                    // TODO it would not be possible to log in.
                    case "newKeyCard":
                        goto default;
                    default:
                        this.serverSocket.SendMessage(processedRequest, validRequest, httpResponseMessageBody);
                        break;
                }
            }
        }

        /// <summary>
        /// Closes down the authentication service
        /// properly.
        /// </summary>
        public void CloseDown()
        {
            this.inService = false;
        }
    }
}
