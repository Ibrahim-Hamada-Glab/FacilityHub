using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace FacilityHub.Services.helper
{
    public class ServiceException : Exception
    {
        public string ErrorCode { get; set; }
        public string[] Errors { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ServiceException(string message, string errorCode, string[] errors, HttpStatusCode statusCode)
        : base(message)
        {
            ErrorCode = errorCode;
            Errors = errors;   
            StatusCode = statusCode;
        }
    }
}