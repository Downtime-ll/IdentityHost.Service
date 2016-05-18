using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        //[HttpGet]
        //public async Task<HttpOkResult> Login(string signin = null)
        //{
        //    var signInMessage = signInMessageCookie.Read(signin);
        //    if (signInMessage == null)
        //    {
        //        Logger.Info("No cookie matching signin id found");
        //        return HandleNoSignin();
        //    }

        //    Logger.DebugFormat("signin message passed to login: {0}", JsonConvert.SerializeObject(signInMessage, Formatting.Indented));

        //    var preAuthContext = new PreAuthenticationContext { SignInMessage = signInMessage };
        //    await userService.PreAuthenticateAsync(preAuthContext);

        //    var authResult = preAuthContext.AuthenticateResult;
        //    if (authResult != null)
        //    {
        //        if (authResult.IsError)
        //        {
        //            Logger.WarnFormat("user service returned an error message: {0}", authResult.ErrorMessage);

        //            await eventService.RaisePreLoginFailureEventAsync(signin, signInMessage, authResult.ErrorMessage);

        //            if (preAuthContext.ShowLoginPageOnErrorResult)
        //            {
        //                Logger.Debug("ShowLoginPageOnErrorResult set to true, showing login page with error");
        //                return await RenderLoginPage(signInMessage, signin, authResult.ErrorMessage);
        //            }
        //            else
        //            {
        //                Logger.Debug("ShowLoginPageOnErrorResult set to false, showing error page with error");
        //                return RenderErrorPage(authResult.ErrorMessage);
        //            }
        //        }

        //        Logger.Info("user service returned a login result");

        //        await eventService.RaisePreLoginSuccessEventAsync(signin, signInMessage, authResult);

        //        return await SignInAndRedirectAsync(signInMessage, signin, authResult);
        //    }

        //    if (signInMessage.IdP.IsPresent())
        //    {
        //        Logger.InfoFormat("identity provider requested, redirecting to: {0}", signInMessage.IdP);
        //        return await LoginExternal(signin, signInMessage.IdP);
        //    }

        //    return await RenderLoginPage(signInMessage, signin);
        //}
    }
}
