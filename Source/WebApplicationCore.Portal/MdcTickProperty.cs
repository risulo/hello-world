using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationCore.Portal
{
    public class MdcTickProperty
    {
        public static readonly MdcTickProperty Default = new MdcTickProperty();

        private MdcTickProperty()
        {
        }

        public override string ToString()
        {
            return Environment.TickCount.ToString();
        }
    }
}
