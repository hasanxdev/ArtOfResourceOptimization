using System.Text.Json;
using ArtOfResourceOptimization.Domain;
using StackExchange.Redis;

namespace ArtOfResourceOptimization.Services;

public class BasketServiceFlyWightV3(IConnectionMultiplexer connectionMultiplexer) : BasketService(connectionMultiplexer)
{
    protected override async Task SetProductToCache<T>(T mainProduct)
    {
        var productJson = JsonSerializer.Serialize(new CachedProduct()
        {
            Name = mainProduct.Name,
            Barcode = mainProduct.Barcode,
            Category = mainProduct.Category,
        });
        
        var productStoreJson = JsonSerializer.Serialize(new CachedProductStore()
        {
            Price = mainProduct.Price,
            Quantity = mainProduct.Quantity,
            StoreName = mainProduct.StoreName,
            StoreId = mainProduct.StoreId,
        });
        
        await connectionMultiplexer.GetDatabase().StringSetAsync($"Product:{mainProduct.Id}",productJson, null, When.NotExists);
        await connectionMultiplexer.GetDatabase().StringSetAsync($"Product:{mainProduct.Id}:Store:{mainProduct.StoreId}",productStoreJson, null, When.NotExists);
    }

    protected override async Task<MainProduct?> GetProductFromCache(int productId, int storeId)
    {
        var productKey = $"Product:{productId}";
        var productStoreKey = $"Product:{productId}:Store:{storeId}";
        var jsons = await connectionMultiplexer.GetDatabase().StringGetAsync([productKey, productStoreKey]);
        if (jsons.Length != 2)
        {
            return null;
        }
        
        var cashedProduct = JsonSerializer.Deserialize<CachedProduct>(jsons[0]);
        var cashedProductStore = JsonSerializer.Deserialize<CachedProductStore>(jsons[1]);
        return new MainProduct()
        {
            Barcode = cashedProduct.Barcode,
            Category = cashedProduct.Category,
            Name = cashedProduct.Name,
            StoreName = cashedProductStore.StoreName,
            StoreId = cashedProductStore.StoreId,
            Price = cashedProductStore.Price,
            Quantity = cashedProductStore.Quantity,
        };
    }
}