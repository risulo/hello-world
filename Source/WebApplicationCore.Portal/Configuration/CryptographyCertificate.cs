using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationCore.Portal.Configuration
{
    public class CryptographyCertificate
    {
        public CryptographyCertificate()
        {
            FindValue = "0555x6666";
        }

        public string FindValue { get; set; }

        public string StoreLocation { get; set; }

        public string StoreName { get; set; }

        public string X509FindType { get; set; }
    }
}
