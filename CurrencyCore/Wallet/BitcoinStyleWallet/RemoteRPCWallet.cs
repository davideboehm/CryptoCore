namespace CryptoCurrencyCore.Wallet.BitcoinStyleWallet
{
    using Core;
    using CurrencyCore.Address;
    using CurrencyCore.Coin;
    using CurrencyCore.Wallet;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    internal class RemoteRPCWallet : WalletBase
    {        
        protected NetworkCredential NetCredentials;
        protected Uri Uri;

        public RemoteRPCWallet(Uri uri, NetworkCredential netCredentials)
        {
            this.Uri = uri;
            this.NetCredentials = netCredentials;
        }

        public override List<AddressAlias> GetAliasList()
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(this.SendCommand("listaccounts"));
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                resultDictionary["result"].ToString());
            var returnValue = (from alias in result
                               where alias.Key.Length == 36
                               select new AddressAlias(alias.Key)).ToList();

            return returnValue;
        }

        public override void ImportPrivateKey(AddressPrivateKey privateKey)
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    this.SendCommand("importprivkey", privateKey.WifString));
            if (resultDictionary["error"] != null)
            {
                var i = 0;
                i += 0;
            }

        }

        public override bool IsValidAddress(PublicAddress address)
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    this.SendCommand("validateaddress", address.ToString()));
            var rest = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultDictionary["result"].ToString());
            return bool.Parse(rest["isvalid"]);
        }

        public override void SendToAddress(PublicAddress destinationAddress, CoinAmount amount)
        {
            if (amount > 0)
            {
                this.SendCommand("sendtoaddress", amount, destinationAddress.ToString());
            }
        }

        public override void SendFrom(AddressAlias from, PublicAddress destinationAddress, CoinAmount amount)
        {
            if (amount > 0)
            {
                this.SendCommand("sendfrom", amount, from.ToString(), destinationAddress.ToString());
            }
        }
        public override void SendMany(AddressAlias from, List<(PublicAddress, CoinAmount)> sendTuples)
        {
        }

        public override int GetBlockCount()
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.SendCommand("getblockcount"));
            return int.Parse(dictionary["result"]);
        }

        public override int GetTransactionTimeOfLastTransaction(AddressAlias alias)
        {
            var result = (JContainer)JsonConvert.DeserializeObject(this.SendCommand("listtransactions", alias.ToString()));

            var last = result["result"].LastOrDefault();

            return last != null ? int.Parse(last["blocktime"].ToString()) : 0;
        }

        public override CoinAmount GetAliasBalanceStartingFromTime(AddressAlias alias, long earliestTimeThatCounts)
        {
            var transactions = this.ListTransactions(alias, 30);

            return transactions != null
                ? (CoinAmount)transactions.Where(
                    transaction => transaction.TransactionCategory == TransactionCategory.Receive &&
                                   transaction.BlockTime > earliestTimeThatCounts)
                    .Sum(transaction => transaction.Amount)
                : (CoinAmount)0;
        }

        public override List<PublicAddress> GetAddress(AddressAlias alias)
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(this.SendCommand("getaddressesbyaccount", alias.ToString()));
            var rest = JsonConvert.DeserializeObject<List<string>>(resultDictionary["result"].ToString());
            return null;
        }

        public override bool IsMine(PublicAddress address)
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    this.SendCommand("validateaddress", address.ToString()));
            var rest = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultDictionary["result"].ToString());

            return bool.Parse(rest["ismine"]);
        }

        public override CoinAmount GetBalance(AddressAlias alias)
        {
            var result = this.SendCommand("getbalance", alias.ToString());
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            return (CoinAmount)decimal.Parse(dictionary["result"]);
        }

        public override List<TransactionRecord> ListTransactions(AddressAlias alias, int count = 10, int skip = 0)
        {
            var result =
                (JContainer)
                    JsonConvert.DeserializeObject(
                        this.SendCommand("listtransactions", alias.ToString(), count.ToString(), skip.ToString()));

            return result["result"]?.Select(transaction => new TransactionRecord(transaction)).ToList();
        }

        private string SendCommand(string method, params string[] paramaters)
        {
            return this.SendCommand(method, CoinAmount.Zero, paramaters);
        }

        private string SendCommand(string method, CoinAmount amount, params string[] paramaters)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(this.Uri);
            webRequest.Credentials = this.NetCredentials;
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            var joe = new JObject
            {
                new JProperty("jsonrpc", "1.0"),
                new JProperty("id", "1"),
                new JProperty("method", method)
            };
            // params is a collection values which the method requires..
            if (paramaters.Length == 0)
            {
                joe.Add(new JProperty("params", new JArray()));
            }
            else
            {
                var props = new JArray();
                // add the props in the reverse order!
                foreach (string t in paramaters)
                {
                    props.Add(new JValue(t.Trim()));
                }
                if (amount != decimal.Zero)
                {
                    props.Add(new JValue(amount));
                }
                joe.Add(new JProperty("params", props));
            }
            // serialize json for the request
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = webRequest.GetResponse();
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream);
            var result = streamReader.ReadToEnd();
            return result;
        }

        public override bool IsValidAddress(string address)
        {
            var resultDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(this.SendCommand("validateaddress", address));
            var rest = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultDictionary["result"].ToString());
            return bool.Parse(rest["isvalid"]);
        }
    }
}
