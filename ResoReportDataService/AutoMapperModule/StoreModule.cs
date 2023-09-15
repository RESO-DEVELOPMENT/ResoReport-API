using AutoMapper;
using ResoReportDataService.Models;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.AutoMapperModule
{
    public static class StoreModule
    {
        public static void ConfigStoreModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Store, StoreViewModel>()
                .ReverseMap();
        }
    }
}