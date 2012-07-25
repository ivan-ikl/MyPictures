namespace MyPictures.Web.Helpers
{
    using System.Net.Http.Headers;
    using System.Web.Http.Filters;

    public class HandleBrowserResponseTypeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            if (actionExecutedContext.ActionContext.Request.Headers.UserAgent.ToString().Contains("MSIE"))
            {
                actionExecutedContext.Response.Content.Headers.ContentType = 
                    new MediaTypeHeaderValue("text/plain");
            }
        }
    }
}
