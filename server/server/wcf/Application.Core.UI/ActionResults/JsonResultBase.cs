using System.Web.Mvc;

namespace Application.Core.UI.ActionResults
{
    public abstract class JsonResultBase : JsonResult
    {
        public bool IsSuccessfully
        {
            get;
            protected set;
        }

        protected JsonResultBase(object data)
        {
            Data = data;
        }

		protected JsonResultBase()
        {
        }
    }
}
