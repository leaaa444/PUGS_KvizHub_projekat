using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
    }
}