using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fastfood.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? Root {  get; set; }
        
    }
}
