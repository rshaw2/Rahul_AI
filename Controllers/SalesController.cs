using Microsoft.AspNetCore.Mvc;
using RahulAI.Model;
using RahulAI.Data;
using RahulAI.Filter;
using Microsoft.AspNetCore.Authorization;

namespace RahulAI.Controllers
{
    /// <summary>
    /// Controller responsible for managing sales-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting sales information.
    /// </remarks>
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly RahulAIContext _context;

        public SalesController(RahulAIContext context)
        {
            _context = context;
        }

        /// <summary>Adds a new sales to the database</summary>
        /// <param name="model">The sales data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Sales model)
        {
            _context.Sales.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of saless based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of saless</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.Sales.AsQueryable();
            var result = FilterService<Sales>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific sales by its primary key</summary>
        /// <param name="entityId">The primary key of the sales</param>
        /// <returns>The sales data</returns>
        [HttpGet]
        [Route("{entityId:Guid}")]
        public IActionResult GetById([FromRoute] Guid entityId)
        {
            var entityData = _context.Sales.FirstOrDefault(entity => entity.SalesId == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific sales by its primary key</summary>
        /// <param name="entityId">The primary key of the sales</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [Route("{entityId:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid entityId)
        {
            var entityData = _context.Sales.FirstOrDefault(entity => entity.SalesId == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific sales by its primary key</summary>
        /// <param name="entityId">The primary key of the sales</param>
        /// <param name="updatedEntity">The sales data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{entityId:Guid}")]
        public IActionResult UpdateById(Guid entityId, [FromBody] Sales updatedEntity)
        {
            if (entityId != updatedEntity.SalesId)
            {
                return BadRequest("Mismatched SalesId");
            }

            var entityData = _context.Sales.FirstOrDefault(entity => entity.SalesId == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(Sales).GetProperties().Where(property => property.Name != "SalesId").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}