using LightInject;
using WebApiKata.Interfaces;
using WebApiKata.Services;
using AutoMapper;
using WebApiKata.Api.ModelMapping;
using WebApiKata.Api.Providers;
using System.Net.Http;

namespace WebApiKata.Api.IoC
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
            serviceRegistry.Register<IHttpClientFactory, WebApiKata.Services.HttpClientFactory>();

            // Transients
            serviceRegistry.Register<IUserService, UserService>();
            serviceRegistry.Register<IProductService, ProductService>();
            //serviceRegistry.Register<ITrolleyCalculator, TrolleyCalculator>();
            serviceRegistry.Register<ITrolleyCalculator, TrolleyCalculator2>();
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
