using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationCore.Portal.Configuration
{
    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEntityFrameworkConfig(
            this IConfigurationBuilder builder, string containerName, Action<DbContextOptionsBuilder> setup)
        {
            return builder.Add(new EFConfigSource(containerName, setup));
        }
    }
}
