using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Responses
{
    public class ServerResponse<T> : BasicResponse
    {
        public ServerResponse(bool success = false)
        {
            IsSuccessful = success;
        }
        public T Data { get; set; }
        public string SuccessMessage { get; set; }
        public int TotalCount { get; set; }


    }
    public static class ServerResponseExtensions
    {
        public static ServerResponse<T> WithTotalCount<T>(this ServerResponse<T> response, int totalCount)
        {
            response.TotalCount = totalCount;
            return response;
        }
    }
}
