using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ResoReportDataService.Models;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{

    public interface IProductService
    {
        List<ProductViewModel> GetProducts();
    }
    public class ProductService : IProductService
    {
        private readonly PosSystemContext _context;
        private readonly IMapper _mapper;

        public ProductService(PosSystemContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public List<ProductViewModel> GetProducts()
        {
            return _context.Products
                .Where(x => x.Status.Equals("Active"))
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider).ToList();
        }
    }
}