namespace ExchangesCore.ExchangeAPICallers
{
    using Logging;
    using System.Net;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class GetCall : RestCall
    {
        public GetCall(Uri uri, ILogger logger, ConcurrentQueue<Action> requestQueue) : base(uri,logger, requestQueue)
        {
        }
        
        protected override WebRequest GetWebRequest(string method)
        {
            return this.GetWebRequest(method, new Dictionary<string, string>());
        }

        protected override WebRequest GetWebRequest(string method, Dictionary<string, string> paramDictionary)
        {
            var fullUrl = this.Uri + "?command=" + method;
            fullUrl = paramDictionary.Keys.Aggregate(fullUrl, (current, key) => current + ("&" + key + "=" + paramDictionary[key]));

            var request = WebRequest.Create(fullUrl);
            request.Method = "GET";
            return request;
        }
    }
}
