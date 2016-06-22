using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nature.Core
{
    public static class AppContext
    {
        public static IServiceProvider ApplicationServices { get; private set; }
        private static HttpContextHolder ContextHolder
        {
            get
            {
                return CallContext.LogicalGetData("HttpContextHolder") as HttpContextHolder;
            }
            set
            {
                CallContext.LogicalSetData("HttpContextHolder",value);
            }
        }

        public static HttpContext CurrentHttpContext
        {
            get { return ContextHolder?.HttpContext; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Null value not allowed.  Call Clear method instead.");
                }

                ContextHolder = new HttpContextHolder
                {
                    HttpContext = value,
                    ManagedThreadId = Thread.CurrentThread.ManagedThreadId
                };

                //Place the context holder in the httpContext's items dictionary where
                //we can get to it later when we need to clear it
                value.Items["HttpContextHolder"] = ContextHolder;
            }
        }


        /// <summary>
        /// Clears the Internal HttpContext
        /// </summary>
        /// <param name="httpContext"></param>
        public static void Clear(HttpContext httpContext)
        {
            HttpContextHolder holder = (HttpContextHolder)httpContext.Items["HttpContextHolder"];
            httpContext.Items.Remove("HttpContextHolder");

            holder.HttpContext = null;
            holder.ManagedThreadId = null;
        }

        public static void SetApplicationServices(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }


        private class HttpContextHolder
        {
            public HttpContext HttpContext { get; set; }
            public int? ManagedThreadId { get; set; }
        }

        public static void UseAppContextService(this IApplicationBuilder app)
        {
            ApplicationServices = app.ApplicationServices;
            app.Use(async (context, next) =>
            {
                AppContext.CurrentHttpContext = context.Request.HttpContext;

                await next(); 

                AppContext.Clear(context);
            });
        }
    }
}
