using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Responses
{
    public class ErrorResponse
    {
        public string ResponseDescription { get; set; }
        public object Data { get; set; }


        public static T Create<T>(string errorMessage) where T : BasicResponse, new()
        {
            var response = new T
            {
                IsSuccessful = false,
                Error = new ErrorResponse
                {
                    ResponseDescription = errorMessage
                }
            };
            return response;
        }
    }
}
