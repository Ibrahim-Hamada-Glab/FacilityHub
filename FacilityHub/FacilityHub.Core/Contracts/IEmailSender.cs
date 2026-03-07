using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacilityHub.Core.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);
         
    }
}