using AutoMapper;
using Domain.Models;
using WarehouseGrpc;

namespace WarehouseService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, GetProductByIdResponse>()
            .ForMember(x => x.Id,
                operation => operation.MapFrom(x => x.Id))
            .ForMember(x => x.Name,
                operation => operation.MapFrom(x => x.Name))
            .ForMember(x => x.Price,
                operation => operation.MapFrom(x => (double)x.Price))
            .ForMember(x => x.Type,
                operation => operation.MapFrom(x => (GrpcProductType)x.Type))
            .ForMember(x => x.Weight,
                operation => operation.MapFrom(x => x.Weight))
            .ForMember(x => x.CreationDate,
                operation =>
                    operation.MapFrom(x => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(x.CreationDate)))
            .ForMember(x => x.WarehouseId,
                operation => operation.MapFrom(x => x.WarehouseId));
        
        CreateMap<CreateProductRequest, Product>()
            .ForCtorParam(nameof(Product.Id),
                x => x.MapFrom(y => 0))
            .ForCtorParam(nameof(Product.Name),
                x => x.MapFrom(y => y.Name))
            .ForCtorParam(nameof(Product.Price),
                x => x.MapFrom(y => (decimal)y.Price))
            .ForCtorParam(nameof(Product.Weight),
                x => x.MapFrom(y => y.Weight))
            .ForCtorParam(nameof(Product.Type),
                x => x.MapFrom(y => (ProductType)y.Type))
            .ForCtorParam(nameof(Product.CreationDate),
                x => x.MapFrom(y => y.CreationDate.ToDateTime()))
            .ForCtorParam(nameof(Product.WarehouseId),
                x => x.MapFrom(y => y.WarehouseId));
    }
}