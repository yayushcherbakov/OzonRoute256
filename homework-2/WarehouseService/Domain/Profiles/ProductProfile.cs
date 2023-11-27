using AutoMapper;
using DataAccess.Models.Products;
using Domain.Models;

namespace Domain.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDal>()
            .ForMember(x => x.Id,
                x => x.MapFrom(y => y.Id))
            .ForMember(x => x.Name,
                x => x.MapFrom(y => y.Name))
            .ForMember(x => x.Price,
                x => x.MapFrom(y => y.Price))
            .ForMember(x => x.Weight,
                x => x.MapFrom(y => y.Weight))
            .ForMember(x => x.Type,
                x => x.MapFrom(y => (ProductTypeDal)y.Type))
            .ForMember(x => x.CreationDate,
                x => x.MapFrom(y => y.CreationDate))
            .ForMember(x => x.WarehouseId,
                x => x.MapFrom(y => y.WarehouseId));

        CreateMap<ProductDal, Product>()
            .ForCtorParam(nameof(Product.Id),
                x => x.MapFrom(y => y.Id))
            .ForCtorParam(nameof(Product.Name),
                x => x.MapFrom(y => y.Name))
            .ForCtorParam(nameof(Product.Price),
                x => x.MapFrom(y => y.Price))
            .ForCtorParam(nameof(Product.Weight),
                x => x.MapFrom(y => y.Weight))
            .ForCtorParam(nameof(Product.Type),
                x => x.MapFrom(y => (ProductType)y.Type))
            .ForCtorParam(nameof(Product.CreationDate),
                x => x.MapFrom(y => y.CreationDate))
            .ForCtorParam(nameof(Product.WarehouseId),
                x => x.MapFrom(y => y.WarehouseId));

        CreateMap<ProductQuery, ProductQueryDal>()
            .ForMember(x => x.PageNumber,
                x => x.MapFrom(y => y.PageNumber))
            .ForMember(x => x.PageSize,
                x => x.MapFrom(y => y.PageSize))
            .ForMember(x => x.Type,
                x => x.MapFrom(y => (ProductTypeDal?)y.Type))
            .ForMember(x => x.CreationDate,
                x => x.MapFrom(y => y.CreationDate))
            .ForMember(x => x.WarehouseId,
                x => x.MapFrom(y => y.WarehouseId));
        
        
        CreateMap<GetProductsResultDal, GetProductsResult>()
            .ForCtorParam(nameof(GetProductsResult.Total),
                x => x.MapFrom(y => y.Total))
            .ForCtorParam(nameof(GetProductsResult.Products),
                x => x.MapFrom(y => y.Products));
    }
}