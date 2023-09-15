using AutoMapper;
using ResoReportDataService.Models;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.AutoMapperModule
{
    public static class ProductModule
    {
        public static void ConfigProductModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Product, ProductViewModel>()
                .ReverseMap();
        }
    }
}