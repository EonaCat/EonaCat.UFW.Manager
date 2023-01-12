using EonaCat.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace EonaCat.UFW.Manager.Helpers
{
    public static class SessionExtensions
    {
        private static bool HasSessionSupport(HttpContext httpContext)
        {
            return httpContext.Features.Get<ISessionFeature>()?.Session != null && httpContext.Session.IsAvailable;
        }

        public static bool Set(this HttpContext context, string key, object value)
        {
            if (context == null || !HasSessionSupport(context))
            {
                return false;
            }
            context.Session.SetString(key, JsonHelper.ToJson(value));
            return true;
        }

        public static T Get<T>(this HttpContext context, string key)
        {
            if (context == null || !HasSessionSupport(context))
            {
                return default;
            }
            var str = context.Session.GetString(key);
            return JsonHelper.ToObject<T>(str);
        }
    }
}
