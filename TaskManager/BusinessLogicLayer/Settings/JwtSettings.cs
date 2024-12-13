using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public double ExpiryMinutes { get; set; }
        public string Key { get; set; }
    }
}
