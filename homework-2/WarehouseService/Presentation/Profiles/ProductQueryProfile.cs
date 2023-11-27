using AutoMapper;
using Domain.Models;
using WarehouseGrpc;

namespace WarehouseService.Profiles;

public class ProductQueryProfile : Profile
{
    public ProductQueryProfile()
    {
        CreateMap<GetProductsRequest, ProductQuery>()
            .ForCtorParam(nameof(ProductQuery.PageNumber),
                x => x.MapFrom(y => y.PageNumber))
            .ForCtorParam(nameof(ProductQuery.PageSize),
                x => x.MapFrom(y => y.PageSize))
            .ForCtorParam(nameof(ProductQuery.Type),
                x => x.MapFrom(y => y.Type.HasValue ? (ProductType?)y.Type.Value : null))
            .ForCtorParam(nameof(ProductQuery.CreationDate),
                x => x.MapFrom(y => y.CreationDate.HasValue ? (DateTime?)y.CreationDate.Value.ToDateTime() : null))
            .ForCtorParam(nameof(ProductQuery.WarehouseId),
                x => x.MapFrom(y => y.WarehouseId.HasValue ? (int?)y.WarehouseId.Value : null));
    }
}