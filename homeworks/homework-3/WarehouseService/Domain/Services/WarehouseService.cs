using AutoMapper;
using DataAccess.Contracts;
using DataAccess.Models.Products;
using Domain.Contracts;
using Domain.Models;

namespace Domain.Services;

internal class WarehouseService : IWarehouseService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public WarehouseService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public int CreateProduct(Product product)
    {
        return _repository.Add(_mapper.Map<ProductDal>(product));
    }

    public Product GetProductById(int productId)
    {
        return _mapper.Map<Product>(_repository.Get(productId));
    }

    public GetProductsResult GetProducts(ProductQuery productQuery)
    {
        var productQueryDal = _mapper.Map<ProductQueryDal>(productQuery);
        
        var result = _repository.Get(productQueryDal);

        var response = _mapper.Map<GetProductsResult>(result);
        
        return response;
    }

    public void UpdateProductPrice(int productId, decimal newPrice)
    {
        var product = _repository.Get(productId);

        product.Price = newPrice;

        _repository.Update(product);
    }
}