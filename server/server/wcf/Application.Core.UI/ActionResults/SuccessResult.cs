namespace Application.Core.UI.ActionResults
{
	public class SuccessResult : JsonResultBase
	{
		public SuccessResult()
		{
            IsSuccessfully = true;
            Data = new { isSuccessfully = IsSuccessfully };
		}

		public SuccessResult(object data)
			: base(data)
		{
			IsSuccessfully = true;
			Data = new { isSuccessfully = IsSuccessfully, data };
		}
	}
}
