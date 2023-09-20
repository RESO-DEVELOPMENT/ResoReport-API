using Microsoft.Extensions.DependencyInjection;
using ResoReportDataService.Services;

namespace ResoReport
{
    public static class DependencyInjectionResolver
    {
        public static void ConfigureDI(this IServiceCollection services)
        {
            //services.AddScoped<IReportService, ReportService>();
            //services.AddScoped<IPaymentReportService, PaymentReportService>();
            //services.AddScoped<IPromotionReportService, PromotionReportService>();
            //services.AddScoped<ISystemReportService, SystemReportService>();
            //services.AddScoped<IStoreService, StoreService>();
            //services.AddScoped<IProductReportService, ProductReportService>();
            services.AddScoped<ICategoryReportService, CategoryReportService>();
            //services.AddScoped<IRevenueReportService, RevenueReportService>();
            //services.AddScoped<IRawQueryService, RawQueryService>();
            //services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IOverviewService, OverviewService>();
            services.AddScoped<IHomeService, HomeService>();
            //services.AddScoped<IInsightService, InsightService>();
        }
    }
}