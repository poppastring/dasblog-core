using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace AutoMapper
{
	// **** this is a dasBlog adaptation of the original Automapper Extension class
	// to address problems with loading assemblies
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions to scan for AutoMapper classes and register the configuration, mapping, and extensions with the service collection
    /// - Finds <see cref="Profile"/> classes and initializes a new <see cref="MapperConfiguration" />
    /// - Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/>, <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" /> and <see cref="IMappingAction{TSource,TDestination}"/> implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Registers <see cref="IConfigurationProvider"/> as <see cref="ServiceLifetime.Singleton"/>
    /// - Registers <see cref="IMapper"/> as <see cref="ServiceLifetime.Scoped"/> with a service factory of the scoped <see cref="IServiceProvider"/>
    /// After calling AddAutoMapper you can resolve an <see cref="IMapper" /> instance from a scoped service provider, or as a dependency
    /// To use <see cref="QueryableExtensions.Extensions.ProjectTo{TDestination}(IQueryable,IConfigurationProvider, System.Linq.Expressions.Expression{System.Func{TDestination, object}}[])" /> you can resolve the <see cref="IConfigurationProvider"/> instance directly for from an <see cref="IMapper" /> instance.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction)
        {
            return AddAutoMapperClasses(services, additionalInitAction, AppDomain.CurrentDomain.GetAssemblies());
        }

        private static readonly Action<IMapperConfigurationExpression> DefaultConfig = cfg => { };



        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assembliesToScan)
        {
            // Just return if we've already added AutoMapper to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(IMapper)))
                return services;

            additionalInitAction = additionalInitAction ?? DefaultConfig;
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();
	        HashSet<string> excludedAssemblies = new HashSet<string>
	        {
		        "CookComputing",
		        "System.WEb"
	        };
#if true
	        List<TypeInfo> typeList = new List<TypeInfo>();
	        foreach (Assembly ass in assembliesToScan)
	        {
		        
		        System.Diagnostics.Debug.WriteLine(ass.GetName().Name);
		        try
		        {
					foreach (TypeInfo ti in ass.GetTypes())
					{
						typeList.Add(ti);
					}
		        }
		        catch (Exception e)
		        {
			        System.Diagnostics.Debug.Write(" FAILED");
		        }
	        }
	        var allTypes = typeList;
#else
	        
	        var allTypes = assembliesToScan
			        .Where(a => a.GetName().Name != nameof(AutoMapper) && !a.GetName().Name.Contains("CookComputing"))
			        .SelectMany(a => a.DefinedTypes)
		        .ToArray();
#endif

            var profiles = allTypes
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
	        
            void ConfigAction(IMapperConfigurationExpression cfg)
            {
                additionalInitAction(cfg);

                foreach (var profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            }

            IConfigurationProvider config = new MapperConfiguration(ConfigAction);

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IMappingAction<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass 
                    && !t.IsAbstract 
                    && t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
            }

            services.AddSingleton(config);
            return services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));
        }

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
