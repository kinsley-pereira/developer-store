using AutoMapper;
using Sales.Contracts.Dto;
using Sales.Domain.Entities;

namespace Sales.Application.Mapping
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            CreateMap<Sale, SaleDto>();
                //.ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Where(i => !i.IsCancelled)));

            CreateMap<SaleItem, SaleItemDto>();
        }
    }
}
