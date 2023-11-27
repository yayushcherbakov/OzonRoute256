using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WarehouseGrpc;

namespace WarehouseService.GrpcServices;

public class GrpcWarehouseService : ProtoWarehouseService.ProtoWarehouseServiceBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly IMapper _mapper;

    public GrpcWarehouseService(IWarehouseService warehouseService, IMapper mapper)
    {
        _warehouseService = warehouseService;
        _mapper = mapper;
    }

    public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request,
        ServerCallContext context)
    {
        var domainProduct = _mapper.Map<Product>(request);
    
        var productId = _warehouseService.CreateProduct(domainProduct);

        var response = new CreateProductResponse()
        {
            ProductId = productId
        };

        return await Task.FromResult(response);
    }

    public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request,
        ServerCallContext context)
    {
        var product = _warehouseService.GetProductById(request.ProductId);

        var grpcProduct = _mapper.Map<GetProductByIdResponse>(product);

        return await Task.FromResult(grpcProduct);
    }

    public override async Task<GetProductsResponse> GetProducts(GetProductsRequest request,
        ServerCallContext context)
    {
        var getProductsPayload = _mapper.Map<ProductQuery>(request);

        var getProductsResult = _warehouseService.GetProducts(getProductsPayload);

        var grpcGetProductsResult = _mapper.Map<GetProductsResponse>(getProductsResult);

        return await Task.FromResult(grpcGetProductsResult);
    }

    public override async Task<Empty> UpdateProductPrice(UpdateProductPriceRequest request,
        ServerCallContext context)
    {
        _warehouseService.UpdateProductPrice(request.ProductId, (decimal)request.NewPrice);

        return await Task.FromResult(new Empty());
    }
}