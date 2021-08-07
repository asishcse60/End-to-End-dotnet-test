using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Contracts.Models;
using Catalog.Database.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogItemsController : ControllerBase
    {
        private readonly IItemRepository _repository;
        private readonly ILogger<CatalogItemsController> _logger;

        public CatalogItemsController(IItemRepository repository, ILogger<CatalogItemsController>logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("Items")]
        public async Task<List<Item>> GetItemsAsync(string name = null)
        {
            var items = (await _repository.GetItemsAsync());
            if (!string.IsNullOrEmpty(name))
            {
                items = items.Where(itm => itm.Name == name).ToList();
            }
            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Retrieved {items.Count()} items");
            return (List<Item>)items;
        }
        [HttpGet("Item")]
        public async Task<ActionResult<Item>> GetItemAsync([FromQuery]string id)
        {
            Console.WriteLine("Calling here for single Item..");
            var item = await _repository.GetItemAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Item>> CreateItemAsync(Item item)
        {
            await _repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item);
        }
        [HttpPut("Update")]
        public async Task<ActionResult> UpdateItemAsync(string id, Item itemDto)
        {
            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = itemDto.Name;
            existingItem.Price = itemDto.Price;

            await _repository.UpdateItemAsync(existingItem);

            return NoContent();
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteItemAsync(string id)
        {
            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            await _repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
