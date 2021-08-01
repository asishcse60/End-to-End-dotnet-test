using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Contracts.Models
{
    public class Item
    {
        public Item()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Required]
        public string Id { get; set; }  
        public string Name { get; set; }    
        public double Price { get; set; }    
        public string Description { get; set; }     
        public DateTimeOffset CreatedDate { get; set; }
    }
}
