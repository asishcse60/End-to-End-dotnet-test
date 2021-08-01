using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Contracts.Models
{
    public record ItemDto(string Name, string Description, double Price, DateTimeOffset CreatedDate);
    public record CreateItemDto(string Name, string Description,  double Price);
    public record UpdateItemDto(string Name, string Description, double Price);
}
