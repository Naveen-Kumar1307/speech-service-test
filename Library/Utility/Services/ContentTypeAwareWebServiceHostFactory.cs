using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.ServiceModel.Web;

namespace GlobalEnglish.ServiceModel
{
    /// <summary>
    /// A service host factory.
    /// </summary>
    public class ContentTypeAwareWebServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// Returns a new content aware service host.
        /// </summary>
        /// <param name="serviceType">a service type</param>
        /// <param name="baseAddresses">base addresses</param>
        /// <returns>a ServiceHost</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebServiceHost2 host = new WebServiceHost2(serviceType, true, baseAddresses);
            host.Interceptors.Add(new ContentTypeRequestInterceptor());
            return host;
        }
    }
}
