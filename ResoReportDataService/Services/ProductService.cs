//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using AutoMapper.QueryableExtensions;
//using ResoReportDataService.Models;
//using ResoReportDataService.ViewModels;

//namespace ResoReportDataService.Services
//{
    
//    public interface IProductService
//    {
//        List<ProductViewModel>   GetProducts();
//    }
//    public class ProductService:IProductService
//    {
//        private readonly ProdPassioContext _context;
//        private readonly IMapper _mapper;
        
//        public ProductService(ProdPassioContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }


//        public List<ProductViewModel> GetProducts()
//        {
//            return _context.Products
//                .Where(x => x.Active == true && x.IsAvailable && x.ProductType == 6)
//                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider).ToList();
//        }
//    }
//}