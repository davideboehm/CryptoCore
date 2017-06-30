namespace ExchangesCore.ExchangeAPICallers
{
    using Core;
    using Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class HmacSha512SignedPostCall : RestCall
    {
        private readonly KeyPair<string, string> keys;
        private static long Nonce = DateTime.UtcNow.Ticks/100;
        public HmacSha512SignedPostCall(Uri uri, ILogger logger, KeyPair<string, string> keys, ConcurrentQueue<Action> requestQueue) : base(uri, logger, requestQueue)
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

        /// <summary>
        /// Call an api method that has parameters.
        /// </summary>
        /// <param name="method">The name of the method you wish to call.</param>
        /// <param name="paramDictionary">
        /// A dictionary that has key/value pairs representing the paramaters you wish to pass to the
        /// Cryptsy method.
        /// </param>
        /// <returns>The string returned by the call to the api or null if there was a failure. </returns>
        protected override WebRequest GetWebRequest(
            string method,
            Dictionary<string, string> paramDictionary)
        {
            var postReq = WebRequest.CreateHttp(this.Uri);
            
            var postData = "command=" + method + "&nonce=" + (DateTime.UtcNow.Ticks / 100);
            postData = paramDictionary.Keys.Aggregate(
                postData,
                (current, key) => current + ("&" + key + "=" + paramDictionary[key]));

            var KeyByte = Encoding.ASCII.GetBytes(this.keys.PrivateKey);
            var HMAcSha = new HMACSHA512(KeyByte);

            var messagebyte = Encoding.ASCII.GetBytes(postData);
            var hashmessage = HMAcSha.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage);
            sign = sign.Replace("-", "").ToLower();

            postReq.Method = "POST";
            postReq.KeepAlive = false;
            postReq.Headers.Add("Key", this.keys.PublicKey.ToString());
            postReq.Headers.Add("Sign", sign);

            postReq.ContentType = "application/x-www-form-urlencoded";
            postReq.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
            postReq.ContentLength = messagebyte.Length;

            var postreqstream = postReq.GetRequestStream();
            postreqstream.Write(messagebyte, 0, messagebyte.Length);
            postreqstream.Close();

            return postReq;
        }
    }
}
