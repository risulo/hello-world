using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
            Console.WriteLine("\n");
            Console.WriteLine(result);
        }

        private static byte[] GetBytes(string input) => Encoding.UTF8.GetBytes(input);
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