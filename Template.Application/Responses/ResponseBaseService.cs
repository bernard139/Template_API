using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Persistence;
using Template.Application.Responses;

namespace Template.Application.Responses
{
    public class ResponseBaseService
    {
        public ResponseBaseService()
        {
        }
        public ServerResponse<T> SetErrorValidation<T>(ServerResponse<T> response, string responseDesc)
        {
            response.Error = new ErrorResponse
            {
                ResponseDescription = responseDesc

            };
            return response;
        }
        public ServerResponse<T> SetError<T>(ServerResponse<T> response, string responseDesc)
        {
            response.Error = new ErrorResponse
            {
                ResponseDescription = responseDesc

            };
            return response;
        }
        public ServerResponse<T> SetError<T>(ServerResponse<T> response, T data, string responseDesc)
        {
            response.Error = new ErrorResponse
            {
                ResponseDescription = responseDesc,
                Data = data

            };
            return response;
        }
        public ServerResponse<T> SetErroWithMessager<T>(ServerResponse<T> response, string responseDesc)
        {
            response.Error = new ErrorResponse
            {
                ResponseDescription = responseDesc
            };
            return response;
        }
        public ServerResponse<T> SetSuccess<T>(ServerResponse<T> response, T data, string responseDesc)
        {
            response.SuccessMessage = responseDesc;
            response.IsSuccessful = true;
            response.Data = data;
            return response;
        }
    }
}
