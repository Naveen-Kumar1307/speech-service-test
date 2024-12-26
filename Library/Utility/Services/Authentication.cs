using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

using AopAlliance.Intercept;
using GlobalEnglish.Utility.Security;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Services
{
    /// <summary>
    /// Contains an authenticated token value.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows an authentic token value</item>
    /// <item>invokes a web service method with a security token</item>
    /// <item>invokes a service method with an authorized token</item>
    /// </list>
    /// <para>Configure instances of this class using the Spring.NET framework as advice for a
    /// proxy instance, on both the client and server sides of a web service interface.
    /// </para>
    /// </remarks>
    public class Authentication : IMethodInterceptor
    {
        /// <summary>
        /// The authentication method name.
        /// </summary>
        public static readonly String Authenticate = "Authenticate";

        /// <summary>
        /// An authentic token value.
        /// </summary>
        public ContextValue TicketValue { get; internal set; }

        /// <summary>
        /// Indicates where this code is executing (server or client).
        /// </summary>
        private bool ServerCode { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a new Authentication.
        /// </summary>
        /// <returns>a new Authentication (configured as a client)</returns>
        public static Authentication AsClient()
        {
            return From("as.client");
        }

        /// <summary>
        /// Returns a new Authentication.
        /// </summary>
        /// <param name="tokenValue">an authentic token value</param>
        /// <returns>a new Authentication</returns>
        public static Authentication From(string tokenValue)
        {
            Argument.Check("tokenValue", tokenValue);
            return new Authentication(tokenValue);
        }

        /// <summary>
        /// Constructs a new Authentication.
        /// </summary>
        /// <param name="tokenValue">an authentic token value</param>
        private Authentication(string tokenValue)
        {
            ServerCode = false;
            TicketValue = EmptyToken.With(tokenValue);
        }

        /// <summary>
        /// Constructs a new Authentication.
        /// </summary>
        public Authentication()
        {
            ServerCode = true;
            TicketValue = EmptyToken.With(string.Empty);
        }

        /// <summary>
        /// A new token value.
        /// </summary>
        private ContextValue EmptyToken
        {
            get { return ContextValue.Named(TokenName); }
        }

        /// <summary>
        /// The authentication token SOAP header name.
        /// </summary>
        private string TokenName
        {
            get { return ConfiguredValue.Named("Token.Name", "AuthHeader"); }
        }
        #endregion

        #region invoking service methods
        /// <inheritdoc/>
        public object Invoke(IMethodInvocation invocation)
        {
            // proceed with invocation depending on code location
            return (ServerCode ? 
                    InvokeService(invocation) : 
                    InvokeWebService(invocation));
        }

        /// <summary>
        /// Invokes a web service method with a security token.
        /// </summary>
        /// <param name="invocation">a web service method</param>
        /// <returns>a web service method result</returns>
        private object InvokeWebService(IMethodInvocation invocation)
        {
            if (invocation.Method.Name == Authenticate)
            {
                TicketValue.Value = invocation.Proceed() as String;
                return TicketValue.Value;
            }
            else
            {
                return SecurityContext
                        .With(this, invocation)
                            .InvokeWebServiceWithSecurityToken();
            }
        }

        /// <summary>
        /// Invokes a service method after checking authentication and authorization.
        /// </summary>
        /// <param name="invocation">a service method</param>
        /// <returns>a service method result</returns>
        private object InvokeService(IMethodInvocation invocation)
        {
            return SecurityContext
                    .With(this, invocation)
                        .InvokeServiceWithAuthenticToken();
        }
        #endregion

        #region checking authentication
        /// <summary>
        /// Checks for a stale authentication token.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">
        /// if the security token from the SOAP header is stale</exception>
        public void CheckTokenLifetime()
        {
            SecurityToken token = GetSecurityToken();
            if (token.IsExpired)
            {
                string message = "The authentication token expired at " + 
                                 token.ExpirationTime.ToShortTimeString();

                throw new UnauthorizedAccessException(message);
            }
        }

        /// <summary>
        /// Returns a security token whose value is derived from the SOAP header.
        /// </summary>
        /// <returns>a security token</returns>
        public SecurityToken GetSecurityToken()
        {
            TicketValue.WithValueFromSOAP();
            return SecurityToken.From(TicketValue.Value);
        }
        #endregion

    } // Authentication
}
