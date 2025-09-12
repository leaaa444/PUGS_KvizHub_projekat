using KvizHub.Api.Dtos;
using KvizHub.Api.Dtos.Category;
using KvizHub.Api.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KvizHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newCategory = await _categoryService.CreateCategoryAsync(dto);
            if (newCategory == null)
            {
                return Conflict("Kategorija sa tim imenom već postoji.");
            }

            return CreatedAtAction(nameof(GetCategories), new { id = newCategory.CategoryID }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try { 
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, dto);
                if (updatedCategory == null)
                {
                    return NotFound($"Kategorija sa ID-jem {id} nije pronađena.");
                }

                return Ok(updatedCategory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success) return NotFound();
            return NoContent(); 
        }
    }
}