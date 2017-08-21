using System.Collections.Generic;
using Microsoft.AspNetCore.Razor;

namespace myCircle_api.Controllers.ResponseMessages
{
    public class Error
    {
        public int code { get; set; }
        public string shortMessage { get; set; }

        public Error(int code, string shortMessage)
        {
            this.code = code;
            this.shortMessage = shortMessage;
        }
    }
}