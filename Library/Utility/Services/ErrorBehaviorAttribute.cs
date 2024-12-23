using System;
using System.Text;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace GlobalEnglish.ServiceModel
{
    /// <summary>
    /// Associates an error handler with a service instance.
    /// </summary>
    public class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        /// <summary>
        /// An error handler type.
        /// </summary>
        public Type HandlerType { get; private set; }

        #region creating instances
        /// <summary>
        /// Constructs a new ErrorBehaviorAttribute.
        /// </summary>
        /// <param name="argument">an argument</param>
        public ErrorBehaviorAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }

        private IErrorHandler CreateHandler()
        {
            try
            {
                return (IErrorHandler)Activator.CreateInstance(HandlerType);
            }
            catch (MissingMethodException ex)
            {
                string message = HandlerType.Name + " must have a public empty constructor.";
                throw new ArgumentException(message, "HandlerType", ex);
            }
            catch (InvalidCastException ex)
            {
                string message = HandlerType.Name + " must implement IErrorHandler.";
                throw new ArgumentException(message, "HandlerType", ex);
            }
        }
        #endregion


        #region IServiceBehavior Members

        public void AddBindingParameters(
            ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(
            ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler handler = CreateHandler();
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(handler);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion

    } // ErrorBehaviorAttribute
}
