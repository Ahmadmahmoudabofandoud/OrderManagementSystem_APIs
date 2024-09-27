using AutoMapper;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;

namespace OrderManagementSystem.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.InvoiceId, O => O.MapFrom(s => s.Invoice.InvoiceId))
                .ForMember(d => d.TotalAmount, O => O.MapFrom(s => s.Invoice.TotalAmount));
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<Product, ProductToReturnDto>();
        }
    }
}
