using System;

namespace OWArcadeBackend.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200;
        public DateTime Time { get; set; } = DateTime.Now;

        public void SetError(int statusCode, string message)
        {
            Success = false;
            StatusCode = statusCode;
            Message = message;
        }
    }
}
