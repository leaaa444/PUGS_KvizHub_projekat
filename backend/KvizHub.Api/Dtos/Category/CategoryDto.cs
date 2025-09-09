using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Category
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}