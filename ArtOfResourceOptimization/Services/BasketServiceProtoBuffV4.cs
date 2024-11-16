using System.Text.Json;
using ArtOfResourceOptimization.Domain;
using ProtoBuf;
using StackExchange.Redis;

namespace ArtOfResourceOptimization.Services;

public class BasketServiceProtoBuffV4(IConnectionMultiplexer connectionMultiplexer) : BasketService(connectionMultiplexer)
{
    protected override async Task SetProductToCache<T>(T mainProduct)
    {
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, new CachedProduct()
            {
                Name = mainProduct.Name,
                Barcode = mainProduct.Barcode,
                Category = mainProduct.Category,
            });
            
            await connectionMultiplexer.GetDatabase().StringSetAsync($"Product:{mainProduct.Id}",ms.ToArray(), null, When.NotExists);
        }
        
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, new CachedProductStore()
            {
                Price = mainProduct.Price,
                Quantity = mainProduct.Quantity,
                StoreName = mainProduct.StoreName,
                StoreId = mainProduct.StoreId,
            });
            
            await connectionMultiplexer.GetDatabase().StringSetAsync($"Product:{mainProduct.Id}:Store:{mainProduct.StoreId}", ms.ToArray(), null, When.NotExists);
        }
    }

    protected override async Task<MainProduct?> GetProductFromCache(int productId, int storeId)
    {
        var productKey = $"Product:{productId}";
        var productStoreKey = $"Product:{productId}:Store:{storeId}";
        var jsons = await connectionMultiplexer.GetDatabase().StringGetAsync([productKey, productStoreKey]);
        if (jsons.Count(p => p.HasValue) == 2)
        {
            return new MainProduct();
        }
        
        return null;
    }
}