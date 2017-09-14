namespace ServiceBase.Authorization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Authorization;

    public static class ScopeAuthorizeHelper
    {
        public static IEnumerable<ScopeAuthorizeAttribute>
            GetAllScopeAuthorizeAttributes<TController>(
            Assembly assembly = null, bool fromReferenced = false)
        {
            var ass = assembly ?? Assembly.GetEntryAssembly();
            var ttype = typeof(TController);

            foreach (var typeInfo in ass.DefinedTypes)
            {
                if (!(typeInfo.IsSubclassOf(ttype) ||
                    typeInfo.IsInstanceOfType(ttype)))
                {
                    continue;
                }

                foreach (var methodInfo in typeInfo.GetMethods())
                {
                    foreach (var attribute in methodInfo
                        .GetCustomAttributes<ScopeAuthorizeAttribute>(true))
                    {
                        yield return attribute;
                    }
                }
            }

            if (fromReferenced)
            {
                foreach (var items in ass.GetReferencedAssemblies()
                    .Select(s => ScopeAuthorizeHelper
                        .GetAllScopeAuthorizeAttributes<TController>(
                            Assembly.Load(s))))
                {
                    foreach (var item in items)
                    {
                        yield return item;
                    }
                }
            }
        }

        public static void AddScopePolicies<TController>(
            this AuthorizationOptions options,
            string issuer,
            Assembly assembly = null,
            bool fromReferenced = false)
        {
            foreach (var attr in ScopeAuthorizeHelper
                .GetAllScopeAuthorizeAttributes<TController>(
                    assembly, fromReferenced))
            {
                options.AddScopePolicy(attr.Scope, issuer);
            }
        }

        public static void AddScopePolicy(
            this AuthorizationOptions options, string scope, string issuer)
        {
            options.AddPolicy(
                scope,
                policy => policy.Requirements
                    .Add(new HasScopeRequirement(scope, issuer)));
        }
    }
}
