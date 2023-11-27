using AutoMapper;
using WarehouseService.Profiles;

namespace WarehouseServiceUnitTest.Helpers;

public static class MapperHelper
{
    public static IMapper GetMapper()
    {
        var configuration = new MapperConfiguration(expression =>
        {
            expression.AddProfile<Domain.Profiles.ProductProfile>();
            expression.AddProfile<ProductProfile>();
            expression.AddProfile<GrpcGetProductsResultProfile>();
            expression.AddProfile<ProductQueryProfile>();
        });
        var mapper = new Mapper(configuration);
        return mapper;
    }
}