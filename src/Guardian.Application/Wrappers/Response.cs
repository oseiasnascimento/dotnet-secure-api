namespace Guardian.Application.Wrappers
{
    public class Response<T>
    {
        public bool Succeeded { get; protected set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }

        public Response()
        {
        }

        public Response(bool succeeded, string message, List<string> errors, T data)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
            Data = data;
        }

        public static Response<T> Success(string message, T data)
        {
            return new Response<T>()
            {
                Message = message,
                Data = data,
                Succeeded = true,
                Errors = []
            };
        }

        public static Response<T> Failure(List<string> errors)
        {
            return new Response<T>()
            {
                Message = "Ops! Ocorreu um erro.",
                Errors = errors,
                Succeeded = false,
                Data = default
            };
        }
    }
}