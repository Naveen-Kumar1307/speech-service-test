using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

using AopAlliance.Intercept;
using GlobalEnglish.Utility.Security;

namespace GlobalEnglish.Utility.Services
{
    /// <summary>
    /// A secure handshake for service method invocation.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a security token</item>
    /// <item>invokes a service method securely</item>
    /// </list>
    /// </remarks>
    public class SecurityContext
    {
        /// <summary>
        /// An authentication token.
        /// </summary>
        public Authentication Token { get; private set; }

        /// <summary>
        /// A deferred proxy method invocation.
        /// </summary>
        public IMethodInvocation Message { get; private set; }

        #region creating instances
        /// <summary>
        /// Returns a new SecureContext.
        /// </summary>
        /// <param name="token">an authentication token</param>
        /// <param name="message">a deferred proxy method invocation</param>
        /// <returns>a new SecureContext</returns>
        public static SecurityContext With(Authentication token, IMethodInvocation message)
        {
            return new SecurityContext(token, message);
        }

        /// <summary>
        /// Constructs a new SecureContext.
        /// </summary>
        /// <param name="token">an authentication token</param>
        /// <param name="message">a deferred proxy method invocation</param>
        private SecurityContext(Authentication token, IMethodInvocation message)
        {
            Token = token;
            Message = message;
        }
        #endregion

        #region invoking services
        /// <summary>
        /// Invokes a service method under protection of a security token.
        /// </summary>
        /// <returns>a service method result</returns>
        /// <remarks>
        /// <h4>Method:</h4>
        /// <list type="bullet">
        /// <item>Obtains a security token from the incoming SOAP message header</item>
        /// <item>Authenticates the security token</item>
        /// <item>Verifies the token has not expired</item>
        /// <item>Invokes the selected service method</item>
        /// <item>Returns the result of the service method, OR</item>
        /// <item>Logs a problem and reports it back to the client</item>
        /// </list>
        /// </remarks>
        public object InvokeServiceWithAuthenticToken()
        {
            Token.CheckTokenLifetime();
            return Message.Proceed();
        }
        #endregion

        #region invoking web services
        /// <summary>
        /// Invokes a web service method under protection of a security token.
        /// </summary>
        /// <returns>a web service method result</returns>
        /// <remarks>
        /// <h4>Method:</h4>
        /// <list type="bullet">
        /// <item>Injects a security token value into the outgoing SOAP message header</item>
        /// <item>Invokes the selected service method</item>
        /// <item>Obtains a new token value from the incoming SOAP message header</item>
        /// <item>Returns the result of the service method</item>
        /// </list>
        /// </remarks>
        public object InvokeWebServiceWithSecurityToken()
        {
            return InvokeChannelService(
                    new Func<Object>(InvokeWebService));
        }

        /// <summary>
        /// Invokes a web service method.
        /// </summary>
        /// <returns>a web service method result</returns>
        public object InvokeWebService()
        {
            return Message.Proceed();
        }

        /// <summary>
        /// Invokes a web service method and updates the security token.
        /// </summary>
        /// <returns>a web service method result</returns>
        public object InvokeWebServiceUpdatingToken()
        {
            try
            {
                return Message.Proceed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FillTokenValueFromSOAP();
            }
        }
        #endregion

        #region managing security token
        /// <summary>
        /// The security token value.
        /// </summary>
        public string TokenValue
        {
            get { return Token.TicketValue.Value; }
            set { Token.TicketValue.Value = value; }
        }

        /// <summary>
        /// Fills the token with a value from the SOAP message header.
        /// </summary>
        public void FillTokenValueFromSOAP()
        {
            Token.TicketValue.WithValueFromSOAP();
        }

        /// <summary>
        /// Injects the token value into the SOAP message header.
        /// </summary>
        public void InjectTokenValueIntoSOAP()
        {
            Token.TicketValue.ComposeOutgoingHeader();
        }

        /// <summary>
        /// Invokes a web service with a security token.
        /// </summary>
        /// <param name="service">a web service</param>
        /// <returns>a web service result</returns>
        private object InvokeChannelService(Func<Object> service)
        {
            return Token.TicketValue.OperateWith<Object>(Channel, service);
        }

        /// <summary>
        /// A message channel.
        /// </summary>
        private IContextChannel Channel
        {
            get { return (IContextChannel)Message.Target; }
        }
        #endregion

    } // SecurityContext
}
