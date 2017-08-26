using System.Collections.Generic;
using myCircle_api.Controllers.ResponseMessages;
using myCircle_api.Model;
using myCircle_api.Repository;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace myCircle_api.Controllers
{
    [Route("v1/[controller]")]
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _categoryRepository;
        public CategoryController()
        {
            _categoryRepository = new CategoryRepository();
        }
        
        // GET v1/category
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _categoryRepository.GetAll();
        }
        
        // POST v1/category
        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            try
            {
                _categoryRepository.Add(category);
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    // Duplicate key
                    case 1048:
                        var error = new Error(e.Number, "CategoryAlreadyExists");
                        return NotFound(error);
                }
            }
            return Ok();
        }
        
        // PUT v1/category/name
        [HttpPut("{name}")]
        public void Put(string name, [FromBody] Category category)
        {
            _categoryRepository.Update(category);
        }
        
        // DELETE v1/category/name
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            _categoryRepository.Delete(name);
        }
    }
}