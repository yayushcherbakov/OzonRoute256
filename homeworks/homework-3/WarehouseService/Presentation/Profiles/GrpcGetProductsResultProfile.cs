using AutoMapper;
using Domain.Models;
using WarehouseGrpc;

namespace WarehouseService.Profiles;

public class GrpcGetProductsResultProfile : Profile
{
    public GrpcGetProductsResultProfile()
    {
        CreateMap<GetProductsResult, GetProductsResponse>()
            .ForMember(x => x.Total,
                operation => operation.MapFrom(x => x.Total))
            .ForMember(x => x.Products,
                operation => operation.MapFrom(x => x.Products));
        
        CreateMap<Product, GrpcProduct>()
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
    }
}