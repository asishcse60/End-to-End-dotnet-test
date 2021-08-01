using Catalog.Contracts.Models;

namespace Catalog.Contracts.Extensions
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
