using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ResoReportDataService.AutoMapperModule;

namespace ResoReport
{
    public static class AutoMapperConfig
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.ConfigStoreModule();
                mc.ConfigProductModule();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
