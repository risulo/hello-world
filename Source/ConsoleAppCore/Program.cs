using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World Updated!");

            var accountRequestId = "533e8b81-5818-4a1b-bfb9-e893de48bb17";

            var openBankingClaim = new OpenBankingClaim
            {
                IdToken = new OpenBankingTokenInfo
                {
                    OpenbankingIntentId = new OpenBankingIntentInfo
                    {
                        Value = accountRequestId,
                        Essential = true
                    }
                }
            };


            //Header added by AddHeader are ignored and basic implementation adds 2 headers manually this way:
            //  header.Add("typ", "JWT");
            //  header.Add("alg", _algorithm.Name);
            //Therefore the final result is different than we expect.
            /*
            var builder = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret("secret");
            builder.AddHeader(HeaderName.Algorithm, "none");
            builder.AddHeader(HeaderName.Type, "JWT");
            builder.AddClaim(ClaimName.Audience, @"https://modelobank.o3bank.co.uk:4501");
            builder.AddClaim(ClaimName.Issuer, "e6238e22-dc0f-4a51-8311-ea5350591063");
            builder.AddClaim("scope", "openid accounts");
            builder.AddClaim("claims", openBankingClaim);
            var str = builder.Build();
            Console.WriteLine(str);
            */

            #region JWT token

            var jsonSerializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var algorithm = new HMACSHA256Algorithm();
            var segments = new List<string>(3);

            var header = new Dictionary<string, object>();
            header.Add("alg", "none");
            header.Add("typ", "JWT");
            var headerBytes = GetBytes(jsonSerializer.Serialize(header));
            segments.Add(urlEncoder.Encode(headerBytes));

            var payload = new Dictionary<string, object>()
            {
                { "aud", "https://modelobank.o3bank.co.uk:4501" },
                { "iss", "e6238e22-dc0f-4a51-8311-ea5350591063" },
                { "scope", "openid accounts" },
                { "claims", openBankingClaim }
            };
            var payloadBytes = GetBytes(jsonSerializer.Serialize(payload));
            segments.Add(urlEncoder.Encode(payloadBytes));

            var stringToSign = String.Join(".", segments.ToArray());
            var bytesToSign = GetBytes(stringToSign);
            var key = GetBytes("secret");
            var signature = algorithm.Sign(key, bytesToSign);
            segments.Add(urlEncoder.Encode(signature));

            var result = String.Join(".", segments.ToArray());
            Console.WriteLine(result);
            Console.WriteLine("\n");

            #endregion JWT token


            #region Get Intelliflo clients

            var response = SendGetRequest("clients");
            Console.WriteLine(response);
            Console.WriteLine("\n");

            #endregion Get Intelliflo clients
            
            #region Post OB account request

            var bodyData = new Dictionary<string, object>()
            {
                { "Permissions", new string[] { "ReadAccountsBasic", "ReadAccountsDetail", "ReadBalances", "ReadTransactionsBasic", "ReadTransactionsCredits", "ReadTransactionsDebits", "ReadTransactionsDetail" } },
                { "TransactionFromDateTime", new DateTime(2015, 9, 28) },
                { "TransactionToDateTime", new DateTime(2015, 9, 29) }
            };

            var body = new Dictionary<string, object>()
            {
                { "Data", bodyData },
                { "Risk", new object() }
            };

            var data = JsonConvert.SerializeObject(body);
            //var response2 = SendPostRequest("account-requests", data);
            //Console.WriteLine(response2);

            #endregion Post OB account request
        }

        private static byte[] GetBytes(string input) => Encoding.UTF8.GetBytes(input);

        private static string SendGetRequest(string action)
        {
            string baseUrl = "https://api.intelliflo.com/v2";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/" + action);
            HttpWebResponse httpResponse = null;
            string result = string.Empty;

            try
            {
                httpWebRequest.Method = "GET";
                AddAuthHeader_Intelliflo(httpWebRequest);

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponseAsync().Result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                //ParseResult(httpResponse);
                return result;
            }
            catch (Exception ex)
            {
                //this.LogException(ex);
                var msg = ex.Message;
                throw;
            }
        }

        private static string SendPostRequest(string action, string jsonData)
        {
            string baseUrl = "https://modelobank.o3bank.co.uk:4101/open-banking/v1.1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/" + action);
            HttpWebResponse httpResponse = null;

            try
            {
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                AddAuthHeader_OB(httpWebRequest);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStreamAsync().Result))
                {
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();

                    string result;
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponseAsync().Result;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                //this.LogException(ex);
                var msg = ex.Message;
                throw;
            }
        }

        private static void AddAuthHeader_OB(HttpWebRequest httpWebRequest)
        {
            var accessToken = "23acbbc7-bf91-4e52-8ca1-6aa6d0cc1779";
            httpWebRequest.Headers["x-fapi-financial-id"] = "7umx5nTR33811QyQfi";
            httpWebRequest.Headers["x-fapi-customer-last-logged-time"] = DateTime.UtcNow.ToString();
            httpWebRequest.Headers["x-fapi-customer-ip-address"] = "10.1.1.10";
            httpWebRequest.Headers["x-fapi-interaction-id"] = Guid.NewGuid().ToString();
            httpWebRequest.Headers["Authorization"] = "Bearer " + accessToken;
        }

        private static void AddAuthHeader_Intelliflo(HttpWebRequest httpWebRequest)
        {
            var accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Impvb25tcFBvaUtmMm9LMkdfVUdhOGJ3dEk1cyIsImtpZCI6Impvb25tcFBvaUtmMm9LMkdfVUdhOGJ3dEk1cyJ9.eyJpc3MiOiJodHRwczovL2lkc3J2My5jb20iLCJhdWQiOiJodHRwczovL2lkc3J2My5jb20vcmVzb3VyY2VzIiwiZXhwIjoxNTE1Nzc1MDgxLCJuYmYiOjE1MTU3NzE0ODEsImNsaWVudF9pZCI6ImFwcC0wZTk2Njk1LXRjZi1kM2Q3ZWNhYWIzYmE0OThlODZkNGJmNDdjOTVlZDcxZSIsInJlYWNoIjoidGVuYW50IiwidGVuYW50X2lkIjoiMTI1NTEiLCJ0ZW5hbnRfZ3VpZCI6IjQyOWQ3M2I1LTgzYjUtNGFiYS1hZmIwLTkzM2U0MTA3MDRmNyIsInNjb3BlIjpbImNsaWVudF9kYXRhIiwiY2xpZW50X2ZpbmFuY2lhbF9kYXRhIiwiZmlybV9kYXRhIiwiZnVuZF9kYXRhIl19.AA8AkVNRIhcVK0q5dooLgLoCm6GieV1SYx2uhkvwAvi2stsYV_opxT0AXzU_EpCPwo2J85plBwGYeh5la9BQmH2CE6qqAxfWzrc61EjAJRYcr1J4BGebAgzvkQW38j6pJxC1LgfMy6hVZXqOO3xWD7Qv-KLOozWB2HCu6GQLa2fRrWCau_640lUJPwoTGua71ZvCLXKDeMTkdnVdwiRfCMXbn5tMTO0daLDsZTAu-yv_gRvzna0fs3ttOmUDdrMpjOvmcYGeX_57PYaMG-JDWO_JsBpJog6kzKZPwfrgJzTRh6Ywu1n6qIgWitrFPQ23b43FD9qrdaRBL94CZv1SyA";
            httpWebRequest.Headers["x-api-key"] = "app-0e96695-c31c347ba17447e6860c8e3b2abb208d";
            httpWebRequest.Headers["Authorization"] = "Bearer " + accessToken;
        }
    }

    public class OpenBankingClaim
    {
        [JsonProperty(PropertyName = "id_token")]
        public OpenBankingTokenInfo IdToken { get; set; }
    }

    public class OpenBankingTokenInfo
    {
        [JsonProperty(PropertyName = "openbanking_intent_id")]
        public OpenBankingIntentInfo OpenbankingIntentId { get; set; }
    }

    public class OpenBankingIntentInfo
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "essential")]
        public bool Essential { get; set; }
    }
}