using KvizHub.Api.Data;
using KvizHub.Api.Dtos;
using KvizHub.Api.Dtos.Category;
using KvizHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly KvizHubContext _context;

        public CategoryService(KvizHubContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsArchived)
                .Select(c => new CategoryDto { CategoryID = c.CategoryID, Name = c.Name })
                .ToListAsync();
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && !c.IsArchived))
            {
                return null; 
            }

            var newCategory = new Models.Category { Name = dto.Name };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return new CategoryDto { CategoryID = newCategory.CategoryID, Name = newCategory.Name };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int categoryId, CreateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null || category.IsArchived)
            {
                return null;
            }

            var nameExists = await _context.Categories
                                   .AnyAsync(c => c.Name == dto.Name && c.CategoryID != categoryId);

            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return new CategoryDto { CategoryID = category.CategoryID, Name = category.Name };
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            category.IsArchived = true; // Logičko brisanje
            await _context.SaveChangesAsync();
            return true;
        }
    }
}