using System;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

using GlobalEnglish.Utility.Security;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Services
{
    /// <summary>
    /// Transfers a named value between a client and a service.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a value and its name (and a namespace)</item>
    /// <item>injects a named value into a SOAP message header before a service call</item>
    /// <item>obtains a named value from a SOAP message header during a service invocation</item>
    /// </list>
    /// </remarks>
    public class ContextValue
    {
        /// <summary>
        /// A named value.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// A value name.
        /// </summary>
        public String ValueName { get; private set; }

        /// <summary>
        /// A name space.
        /// </summary>
        public String Namespace { get; private set; }

        #region creating instances
        /// <summary>
        /// Returns a new ContextValue.
        /// </summary>
        /// <param name="valueName">a value name</param>
        /// <returns>a new ContextValue</returns>
        public static ContextValue Named(String valueName)
        {
            return Named(valueName, "http://globalenglish.com"); // default namespace
        }

        /// <summary>
        /// Returns a new ContextValue.
        /// </summary>
        /// <param name="valueName">a value name</param>
        /// <param name="nameSpace">a name space</param>
        /// <returns>a new ContextValue</returns>
        public static ContextValue Named(String valueName, String nameSpace)
        {
            Argument.Check("valueName", valueName);
            Argument.Check("nameSpace", nameSpace);
            return new ContextValue(valueName, nameSpace);
        }

        /// <summary>
        /// Constructs a new ContextValue.
        /// </summary>
        /// <param name="valueName">a value name</param>
        /// <param name="nameSpace">a name space</param>
        private ContextValue(String valueName, String nameSpace)
        {
            ValueName = valueName;
            Namespace = nameSpace;
        }

        /// <summary>
        /// Establishes the value for this.
        /// </summary>
        /// <param name="namedValue">a named value</param>
        /// <returns>this ContextValue</returns>
        public ContextValue With(String namedValue)
        {
            Value = namedValue;
            return this;
        }
        #endregion

        #region forming context SOAP header
        /// <summary>
        /// Adds this named value as a SOAP header and calls a web service.
        /// </summary>
        /// <typeparam name="ResultType">a service method result type</typeparam>
        /// <param name="channel">a service channel</param>
        /// <param name="ServiceAction">a service method call</param>
        /// <returns>the result of a web service method</returns>
        public ResultType OperateWith<ResultType>(
            IContextChannel channel, Func<ResultType> ServiceAction)
        {
            using (OperationContextScope scope = 
                    new OperationContextScope(channel))
            {
                ComposeOutgoingHeader();
                return ServiceAction();
            }
        }

        /// <summary>
        /// Composes an outgoing message header with this token value.
        /// </summary>
        public void ComposeOutgoingHeader()
        {
            OutgoingHeaders.Add(OutgoingValueHeader);
        }

        /// <summary>
        /// An outgoing message header.
        /// </summary>
        private MessageHeader OutgoingValueHeader
        {
            get { return MessageHeader.CreateHeader(ValueName, Namespace, OutgoingValue); }
        }

        /// <summary>
        /// An outgoing context value.
        /// </summary>
        private String OutgoingValue
        {
            get { return Value; }
        }

        /// <summary>
        /// The outgoing message headers.
        /// </summary>
        private MessageHeaders OutgoingHeaders
        {
            get { return OperationContext.Current.OutgoingMessageHeaders; }
        }
        #endregion

        #region accessing SOAP headers
        /// <summary>
        /// Obtains the named value from incoming SOAP headers.
        /// </summary>
        /// <returns>this ContextValue</returns>
        public ContextValue WithValueFromSOAP()
        {
            String codedValue = IncomingValue;
            if (codedValue.Length > 0) Value = codedValue;
            return this;
        }

        /// <summary>
        /// An incoming context value.
        /// </summary>
        private String IncomingValue
        {
            get
            {
                MessageHeaders headers = IncomingHeaders;
                if (headers == null) return String.Empty;
                return headers.GetHeader<String>(ValueName, Namespace);
            }
        }

        /// <summary>
        /// The incoming message headers.
        /// </summary>
        private MessageHeaders IncomingHeaders
        {
            get
            {
                OperationContext context = OperationContext.Current;
                if (context == null) return null;
                return context.IncomingMessageHeaders;
            }
        }
        #endregion

    } // ContextValue
}
