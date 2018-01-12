using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WebApplicationCore.API
{
    public partial class Startup
    {
        private void ConfigureAuth(IApplicationBuilder app)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5000/",
                //IssuerSigningKey = new X509SecurityKey(new X509Certificate2(certLocation)),
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                Audience = "http://localhost:5001/",
                Authority = "http://localhost:5000/",
                AutomaticAuthenticate = true
            });
        }
    }
}
