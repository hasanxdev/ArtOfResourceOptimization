using ArtOfResourceOptimization.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtOfResourceOptimization.Controllers;

public class AddToBasketController(BasketServiceProtoBuffV4 basketService) : ControllerBase
{
    [Route("AddToBasket")]
    [HttpGet]
    public async Task<IActionResult> AddToBasket(int productId, int storeId)
    {
        await basketService.AddToBasket(productId, storeId);
        return Ok("Successfully added to Basket");
    }

    [Route("BulkAddToBasket")]
    [HttpGet]
    public async Task<IActionResult> BulkToBasket(int countProducts, int countStores)
    {
        await Parallel.ForAsync(0, countStores,
            async (storeId, _) =>
            {
                await Parallel.ForAsync(0, countProducts, _, async (productId, _) =>
                {
                    await basketService.AddToBasket(productId, storeId, false);
                });
            });

        return Ok("Successfully added to Basket");
    }

    [Route("ClearAllProducts")]
    [HttpGet]
    public async Task<IActionResult> ClearAllProducts()
    {
        await basketService.ClearAllProducts();

        return Ok("Successfully added to Basket");
    }
    
    [Route("ClearGC")]
    [HttpGet]
    public IActionResult ClearGc()
    {
        GC.Collect();

        return Ok("Successfully");
    }
}