using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacilityHub.Core.helper
{
    public class EmailConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
    }
}