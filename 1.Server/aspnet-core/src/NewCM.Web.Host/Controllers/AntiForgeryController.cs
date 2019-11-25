using Microsoft.AspNetCore.Antiforgery;
using NewCM.Controllers;

namespace NewCM.Web.Host.Controllers
{
    public class AntiForgeryController : NewCMControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
