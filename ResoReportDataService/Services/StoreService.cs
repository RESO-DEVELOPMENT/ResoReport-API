using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ResoReportDataService.Commons;
using ResoReportDataService.Models;
using ResoReportDataService.ViewModels;

namespace ResoReportDataService.Services
{
    public interface IStoreService
    {
        List<StoreViewModel> GetListStore();


    }

    public class StoreService : IStoreService
    {
        private readonly ProdPassioContext _context;
        private readonly IMapper _mapper;
        
        public StoreService(ProdPassioContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<StoreViewModel> GetListStore()
        {
            return _context.Stores.Where(x => x.IsAvailable == true && x.BrandId == 1)
                .ProjectTo<StoreViewModel>(_mapper.ConfigurationProvider).ToList();
        }
        
        
    }
}