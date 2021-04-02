using System;
using System.Net.Http;

namespace SertifiTestApi.Services.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static void ValidateStatusCode(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}