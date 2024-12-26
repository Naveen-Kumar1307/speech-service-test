using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Common.Logging;

namespace GlobalEnglish.ServiceModel
{
    /// <summary>
    /// Handles errors and exceptions that occur within a service.
    /// </summary>
    public class ServiceErrorHandler : IErrorHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceErrorHandler));

        #region IErrorHandler Members

        public bool HandleError(Exception error)
        {
            Logger.Fatal(error.ToString());
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            fault = Message.CreateMessage(version, 
                new FaultException<string>(error.Message, 
                    new FaultReason(error.Message)).CreateMessageFault(), "");
        }

        #endregion

    } // ServiceErrorHandler
}
