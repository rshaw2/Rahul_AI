using Microsoft.AspNetCore.Mvc;
using RahulAI.Model;
using RahulAI.Data;
using RahulAI.Filter;
using Microsoft.AspNetCore.Authorization;

namespace RahulAI.Controllers
{
    /// <summary>
    /// Controller responsible for managing country-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting country information.
    /// </remarks>
    [Route("api/[controller]")]
    [Authorize]
    public class CountryController : ControllerBase
    {
        private readonly RahulAIContext _context;

        public CountryController(RahulAIContext context)
        {
            _context = context;
        }

        /// <summary>Adds a new country to the database</summary>
        /// <param name="model">The country data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Country model)
        {
            _context.Country.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of countrys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of countrys</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.Country.AsQueryable();
            var result = FilterService<Country>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific country by its primary key</summary>
        /// <param name="entityId">The primary key of the country</param>
        /// <returns>The country data</returns>
        [HttpGet]
        [Route("{entityId}")]
        public IActionResult GetById([FromRoute] string entityId)
        {
            var entityData = _context.Country.FirstOrDefault(entity => entity.Name == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific country by its primary key</summary>
        /// <param name="entityId">The primary key of the country</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [Route("{entityId}")]
        public IActionResult DeleteById([FromRoute] string entityId)
        {
            var entityData = _context.Country.FirstOrDefault(entity => entity.Name == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.Country.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific country by its primary key</summary>
        /// <param name="entityId">The primary key of the country</param>
        /// <param name="updatedEntity">The country data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{entityId}")]
        public IActionResult UpdateById(string entityId, [FromBody] Country updatedEntity)
        {
            if (entityId != updatedEntity.Name)
            {
                return BadRequest("Mismatched Name");
            }

            var entityData = _context.Country.FirstOrDefault(entity => entity.Name == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(Country).GetProperties().Where(property => property.Name != "Name").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}