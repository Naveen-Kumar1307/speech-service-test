using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Web;

namespace GlobalEnglish.ServiceModel
{
    /// <summary>
    /// Intercepts a web service request to determine which data format(s) are supported.
    /// </summary>
    public class ContentTypeRequestInterceptor : RequestInterceptor
    {
        /// <summary>
        /// Constructs a new ContentTypeRequestInterceptor.
        /// </summary>
        public ContentTypeRequestInterceptor() : base(true) { }

        /// <summary>
        /// Processes a request by determining which data format(s) are supported.
        /// </summary>
        /// <param name="requestContext">a request context</param>
        public override void ProcessRequest(ref RequestContext requestContext)
        {
            if (requestContext == null) return;
            Message request = requestContext.RequestMessage;
            if (request == null) return;

            HttpRequestMessageProperty prop = 
                (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

            string format = null;
            string accepts = prop.Headers[HttpRequestHeader.Accept];
            if (accepts != null)
            {
                if (accepts.Contains("text/xml") || accepts.Contains("application/xml"))
                {
                    format = "xml";
                }
                else if (accepts.Contains("application/json"))
                {
                    format = "json";
                }
            }
            else
            {
                string contentType = prop.Headers[HttpRequestHeader.ContentType];
                if (contentType != null)
                {
                    if (contentType.Contains("text/xml") || contentType.Contains("application/xml"))
                    {
                        format = "xml";
                    }
                    else if (contentType.Contains("application/json"))
                    {
                        format = "json";
                    }
                }
            }
            if (format != null)
            {
                UriBuilder toBuilder = new UriBuilder(request.Headers.To);
                if (string.IsNullOrEmpty(toBuilder.Query))
                {
                    toBuilder.Query = "format=" + format;
                }
                else if (!toBuilder.Query.Contains("format="))
                {
                    toBuilder.Query += "&format=" + format;
                }
                request.Headers.To = toBuilder.Uri;
            }
        }
    }
}
