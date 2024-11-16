using System.Text.Json;
using ArtOfResourceOptimization.Domain;
using Bogus;
using StackExchange.Redis;

namespace ArtOfResourceOptimization.Services;

public class BasketService(IConnectionMultiplexer connectionMultiplexer)
{
    public static Faker<MainProduct> ProductFaker = new Faker<MainProduct>()
        .RuleFor(p => p.Id, f => f.IndexFaker + 1)
        .RuleFor(p => p.Name, f => f.Commerce.ProductName() + f.Commerce.ProductName())
        .RuleFor(p => p.Barcode, f => f.Random.Int(10000000, 90000000).ToString())
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription() + f.Commerce.ProductDescription() + f.Commerce.ProductDescription() + f.Commerce.ProductDescription())
        .RuleFor(p => p.Price, f => f.Random.Decimal(1, 100))
        .RuleFor(p => p.Quantity, f => f.Random.Int(0, 100))
        .RuleFor(p => p.IsAvailable, f => f.Random.Bool())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.ExpiryDate, f => f.Date.Future(2))
        .RuleFor(p => p.Brand, f => f.Company.CompanyName())
        .RuleFor(p => p.StoreName, f => f.Company.CompanyName())
        .RuleFor(p => p.StoreAddress, f => f.Address.FullAddress());

    public async Task<MainProduct> AddToBasket(int productId, int storeId, bool wait = true)
    {
        _ = new Virus();
        
        var product = await GetProductFromCache(productId, storeId);

        if (product == null)
        {
            product = await GetProductFromExternalService(productId, storeId, wait);
            await SetProductToCache(product);
            return product;
        }

        return product;
    }

    public async Task ClearAllProducts()
    {
        var server = connectionMultiplexer.GetServer("localhost", 6379); // replace with your Redis server and port
        await server.FlushAllDatabasesAsync();
    }

    protected virtual async Task SetProductToCache<T>(T mainProduct) where T : Product
    {
        var objJson = JsonSerializer.Serialize(mainProduct);
        await connectionMultiplexer.GetDatabase().StringSetAsync($"Product:{mainProduct.Id}:Store:{mainProduct.StoreId}",objJson);
    }

    private async Task<MainProduct> GetProductFromExternalService(int productId, int storeId, bool wait)
    {
        if (wait)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        
        var product = ProductFaker.Generate();
        product.Id = productId;
        product.StoreId = storeId;
        return product;
    }

    protected virtual async Task<MainProduct?> GetProductFromCache(int productId, int storeId)
    {
        var objJson = await connectionMultiplexer.GetDatabase().StringGetAsync($"Product:{productId}:Store:{storeId}");
        if (string.IsNullOrEmpty(objJson))
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<MainProduct?>(objJson!);
    }
}