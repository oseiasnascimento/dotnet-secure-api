namespace Guardian.Application.Parameters
{
    /// <summary>
    /// Parâmetros básicos que podem ser usados em uma requisição.
    /// </summary>
    public class RequestParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public RequestParameter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public RequestParameter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}