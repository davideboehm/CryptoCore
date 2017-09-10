namespace ExchangesCore.ExchangeAPICallers
{
    using Core;
    using Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class SignedPostCaller : ApiCaller
    {
        protected readonly KeyPair<string, string> keys;
        protected static long Nonce = DateTime.UtcNow.Ticks / 100;
        public SignedPostCaller(Uri uri, ILogger logger, KeyPair<string, string> keys, ConcurrentQueue<Action> requestQueue) : base(uri, logger, requestQueue)
        {
            this.keys = keys;
        }

        /// <summary>
        /// Call an api method that does not have any paramaters.
        /// </summary>
        /// <param name="method">The name of the method you wish to call.</param>
        /// <returns>The string returned by the call to the api or null if there was a failure.</returns>
        protected override WebRequest GetWebRequest(string method)
        {
            return (this.GetWebRequest(method, new Dictionary<string, string>()));
        }

        protected HttpWebRequest CreateRequest(Uri uri, string postData, NameValueCollection headers = null)
        {
            var request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.KeepAlive = false;

            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";

            if (headers != null)
            {
                request.Headers.Add(headers);
            }

            var data = Encoding.ASCII.GetBytes(postData);
            var KeyByte = Encoding.ASCII.GetBytes(this.keys.PrivateKey);
            var HMAcSha = new HMACSHA512(KeyByte);
            var hashmessage = HMAcSha.ComputeHash(data);
            var signature = BitConverter.ToString(hashmessage);
            signature = signature.Replace("-", "").ToLower();

            this.AddSignature(request, signature);

            request.ContentLength = data.Length;
            var postreqstream = request.GetRequestStream();
            postreqstream.Write(data, 0, data.Length);
            postreqstream.Close();

            return request;
        }

        protected abstract void AddSignature(HttpWebRequest request, string signature);

    }
}
