using ArtOfResourceOptimization.Domain;
using StackExchange.Redis;

namespace ArtOfResourceOptimization.Services;

public class BasketServiceWithoutRedundantDataV2(IConnectionMultiplexer connectionMultiplexer) : BasketService(connectionMultiplexer)
{
    protected override Task SetProductToCache<T>(T mainProduct)
    {
        return base.SetProductToCache(new Product()
        {
            Id = mainProduct.Id,
            Barcode = mainProduct.Barcode,
            StoreName = mainProduct.StoreName,
            Category = mainProduct.Category,
            Name = mainProduct.Name,
            StoreId = mainProduct.StoreId,
            Price = mainProduct.Price,
            Quantity = mainProduct.Quantity,
        });
    }
}