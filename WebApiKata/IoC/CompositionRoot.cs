using LightInject;
using DataAccess.Interfaces;
using DataAccess.ExternalApi;
using AutoMapper;
using WebApiKata.ModelMapping;
using WebApiKata.Providers;
using System.Net.Http;

namespace WebApiKata.IoC
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            // Singletons
            RegisterAutoMapper(serviceRegistry);
            serviceRegistry.Register<IConfigProvider, ConfigProvider>(new PerContainerLifetime());
            serviceRegistry.Register<IExternalApiPathProvider, ExternalApiPathProvider>(new PerContainerLifetime());
            serviceRegistry.Register<ISerializer, JsonSerializer>(new PerContainerLifetime());
            serviceRegistry.Register<IHttpClientFactory, DataAccess.ExternalApi.HttpClientFactory>();

            // Transients
            serviceRegistry.Register<IUserRepository, UserRepository>();
            serviceRegistry.Register<IProductRepository, ProductRepository>();
        }

        private static void RegisterAutoMapper(IServiceRegistry serviceRegistry)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            serviceRegistry.Register<IMapper>((c) => mapper, new PerContainerLifetime());
        }
    }
}
