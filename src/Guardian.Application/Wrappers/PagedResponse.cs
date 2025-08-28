
namespace Guardian.Application.Wrappers
{
    public class PagedResponse<T> : Response<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public PagedResponse()
        {
        }

        public PagedResponse(List<string> errors)
        {
            this.Message = "Itens recuperados com sucesso.";
            this.Data = null;
            this.Succeeded = false;
            this.Errors = errors;
        }

        public PagedResponse(T data, int pageNumber, int pageSize, int total)
        {
            this.Message = "Itens recuperados com sucesso.";
            this.Data = data;
            this.Succeeded = true;
            this.Errors = [];

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Total = total;
        }
    }
}