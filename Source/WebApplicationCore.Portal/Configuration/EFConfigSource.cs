using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace WebApplicationCore.Portal.Configuration
{
    public class EFConfigSource : IConfigurationSource
    {
        private readonly string _containerName;
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public EFConfigSource(string containerName, Action<DbContextOptionsBuilder> optionsAction)
        {
            _containerName = containerName;
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFConfigProvider(_containerName, _optionsAction);
        }
    }
}
