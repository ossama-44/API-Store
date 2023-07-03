using APIStore.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.OrderAggregate;
using System.Security.Cryptography;

namespace APIStore.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        { 
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductBrand, option => option.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.ProductType, option => option.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom<ProductUrlResolver>());
            
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<ShippingAddress, ShippingAddressDto>().ReverseMap();

            CreateMap<Order, OrderDetailsDto>()
                .ForMember(dest => dest.DeliveryMethod, option => option.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.ShippingPrice, option => option.MapFrom(src => src.DeliveryMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.ItemOrderd.ProductItemId))
                .ForMember(dest => dest.ProductName, option => option.MapFrom(src => src.ItemOrderd.ProductName))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom(src => src.ItemOrderd.PictureUrl))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom<OrderItemUrlResolver>());

        }
    }
}
