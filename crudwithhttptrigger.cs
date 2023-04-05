using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShoppingCartList.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingCartList
{
    public class ShoppingCartApi
    {
        static List<ShoppingCartItem>shoppingCartItems=new();

        [FunctionName("GetShoppingCartItems")]
        public static async Task<IActionResult> GetShoppingCartItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shoppingcartitem")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting All Shopping Cart Items");

            return new OkObjectResult(shoppingCartItems);
        }

        [FunctionName("GetShoppingCartItemById")]
        public static async Task<IActionResult> GetShoppingCartItemById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shoppingcartitem/{id}")]

            HttpRequest req, ILogger log, string id)
        {
            log.LogInformation($"Getting Shopping Cart Item with ID: {id}");

           return new OkObjectResult(shoppingCartItems);
        }

        [FunctionName("CreateShoppingCartItem")]
        public static async Task<IActionResult> CreateShoppingCartItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shoppingcartitem")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating Shopping Cart Item");
            string requestData = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<CreateShoppingCartItem>(requestData);

            var item = new ShoppingCartItem
            {
                ItemName = data.ItemName,
            };

            shoppingCartItems.Add(item);
            ////await shoppingCartItemsOut.AddAsync(item);

            return new OkObjectResult(item);
        }

        [FunctionName("PutShoppingCartItem")]
        public static async Task<IActionResult> PutShoppingCartItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "shoppingcartitem/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Updating Shopping Cart Item with ID: {id}");
            var shoppingCartItem=shoppingCartItems.FirstOrDefault(q=>q.Id==id);
            if (shoppingCartItem==null)
            {
                return new NotFoundResult();
            }
            string requestData = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UpdateShoppingCartItem>(requestData);
            shoppingCartItem.Collected=data.Collected;
            return new ObjectResult(shoppingCartItem);
        }

        [FunctionName("DeleteShoppingCartItem")]
        public static async Task<IActionResult> DeleteShoppingCartItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "shoppingcartitem/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Deleting Shopping Cart Item with ID: {id}");
            var shoppingCartItem=shoppingCartItems.FirstOrDefault(q=>q.Id==id);
        if(shoppingCartItem==null)
        {
            return new NotFoundResult();
        }
        shoppingCartItems.Remove(shoppingCartItem);
        return new OkResult();
    }
}
}