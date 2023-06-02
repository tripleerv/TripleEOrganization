using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace OrgChart.Web
{
    public class AuthorizeUserFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            //context.Result = new RedirectToPageResult("/Index");
            /*
            if (context.HttpContext.Request.Cookies["OrgChart_CookieAuth"] == null)
            {
                string user_name = "";// context.HttpContext.User.Identity.Name;

                if (context.HttpContext.Request.Host.Value.Contains("localhost"))
                {
                    user_name = context.HttpContext.User.Identity.Name;
                    user_name = "cwaldner";
                }
                else
                {
                    user_name = context.HttpContext.User.Identity.Name;
                    user_name = user_name == null ? "" : user_name;

                    if (user_name.StartsWith("TRIPLEE"))
                    {
                        user_name = context.HttpContext.User.Identity.Name.Substring(8, (user_name.Length - 8));
                    }
                }

                var json_user = JsonConvert.SerializeObject(new Models.LoginModel
                {
                    UserName = user_name
                });

                context.HttpContext.Session.SetString("User", json_user);
                
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    var new_pc = new UserPrincipal(pc);

                    var ad_user = UserPrincipal.FindByIdentity(pc, user_name);

                    if (ad_user != null)
                    {
                        List<Claim> claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Name, user_name));

                        var groups = ad_user.GetAuthorizationGroups();

                        if (groups != null)
                        {
                            if (groups.Any(x => x.Name.Equals("IT") || x.Name.Equals("HR Admin")))
                            {
                                claims.Add(new Claim(ClaimTypes.Role, "HR"));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "TEMP"));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, "OrgChart_CookieAuth");

                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
                            IsPersistent = false,
                            IssuedUtc = DateTimeOffset.UtcNow,
                        };

                        context.HttpContext.SignInAsync("OrgChart_CookieAuth",
                                                    claimsPrincipal,
                                                    authProperties);

                        //context.Result = new RedirectToPageResult("/Index");
                    }
            */
                
                
            
        }
    }
}
