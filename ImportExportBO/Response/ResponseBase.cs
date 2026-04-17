namespace ImportExportBO.Response
{
    public class ResponseBase<T> where T : class
    {
        public long Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
