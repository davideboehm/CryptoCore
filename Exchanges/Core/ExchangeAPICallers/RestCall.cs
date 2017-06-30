namespace ExchangesCore.ExchangeAPICallers
{
    using Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public abstract class RestCall
    {
        protected readonly ILogger Logger;
        protected readonly Uri Uri;
        protected readonly ConcurrentQueue<Action> RequestQueue;
        protected RestCall(Uri uri, ILogger logger, ConcurrentQueue<Action> requestQueue)
        {
            this.RequestQueue = requestQueue;
            this.Uri = uri;
            this.Logger = logger;
        }

        public async ValueTask<string> MakeCall(string method, Dictionary<string, string> paramDictionary = null)
        {
            if (paramDictionary == null)
            {
                paramDictionary = new Dictionary<string, string>();
            }
            return await this.GetResponse(method, paramDictionary);
        }

        protected abstract WebRequest GetWebRequest(string method);

        protected abstract WebRequest GetWebRequest(string method, Dictionary<string, string> paramDictionary);

        protected async ValueTask<string> GetResponse(string method, Dictionary<string, string> paramDictionary = null)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            this.RequestQueue.Enqueue(
            () =>
            {
                try
                {
                    var request = this.GetWebRequest(method, paramDictionary);

                    using (var responseStream = request.GetResponse().GetResponseStream())
                    {
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            var responseString = responseStreamReader.ReadToEnd().ToLower();

                            if (responseString.IndexOf("return\":false", StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                this.Logger.LogError("apiQuery failed with: " + responseString);
                                tcs.SetException(new Exception(responseString));
                            }
                            else
                            {
                                tcs.SetResult(responseString);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    this.Logger.LogError("apiQuery failed with" + ex.Message);
                    tcs.SetException(ex);
                }

                return;
            });

            return await tcs.Task;
        }
    }
}


