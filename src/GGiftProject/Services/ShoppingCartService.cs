using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GGiftProject.Services
{
    public static class ShoppingCartService
    {
        public const string CartSessionKey = "CartId";
  
        public static void SetCartId(HttpContext httpContext)
        {
            if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
            {
                httpContext.Session.SetString(CartSessionKey, httpContext.User.Identity.Name);
            }
        }

        public static void ResetCartId(HttpContext httpContext)
        {
            httpContext.Session.Remove(CartSessionKey);
        }

        public static string GetCartId(HttpContext httpContext)
        {
            if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
            {
                httpContext.Session.SetString(CartSessionKey, httpContext.User.Identity.Name);
            }
            else if (httpContext.Session.GetString(CartSessionKey) == null)
            {
                // Generate a new random GUID using System.Guid class.     
                Guid tempCartId = Guid.NewGuid();
                httpContext.Session.SetString(CartSessionKey, tempCartId.ToString());
            }

            return httpContext.Session.GetString(CartSessionKey).ToString();
        }
    }
}
