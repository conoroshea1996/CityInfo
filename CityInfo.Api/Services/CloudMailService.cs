using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Api.Services
{
    public class CloudMailService : IMailService
    {
        private readonly IConfiguration _config;

        public CloudMailService(IConfiguration config)
        {
            _config = config;
        }

        public void Send(string subject, string message)
        {
            // send mail - output to debug window
            Debug.WriteLine($"Mail from {_config["mailSettings:mailFromAddress"]} to {_config["mailSettings:mailToAddress"]}, with CloudService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
