namespace DM.Core.DTOs.General
{
    public class APIResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class APIResponse : APIResponse<object>
    {
    }
    public class APIPagingResponse : APIResponse<object>
    {
        public int Total { get; set; }
    }
}
