using KvizHub.Api.Dtos;
using KvizHub.Api.Dtos.Category;
using KvizHub.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Category
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(int categoryId, CreateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int categoryId);        
    }
}