namespace OverwatchArcade.API.Dtos
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string[] ErrorMessages { get; set; } = Array.Empty<string>();
        public int StatusCode { get; set; } = 200;

        public void SetError(int statusCode, string errorMessage)
        {
            Success = false;
            StatusCode = statusCode;
            ErrorMessages =  new[] { errorMessage };
        }
    
        public void SetError(int statusCode, string[] errorMessages)
        {
            Success = false;
            StatusCode = statusCode;
            ErrorMessages =  errorMessages;
        }
    }
}
