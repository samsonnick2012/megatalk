namespace Application.Core.UI.ActionResults
{
    public class UnsuccessResult : JsonResultBase
    {
        public UnsuccessResult()
        {
            IsSuccessfully = false;
            Data = new { isSuccessfully = IsSuccessfully };
        }

        public UnsuccessResult(object data)
            : base(data)
        {
            IsSuccessfully = false;
            Data = new { isSuccessfully = IsSuccessfully, data };
        }
    }
}
