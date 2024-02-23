using Microsoft.AspNetCore.Mvc;
using RahulAI.Model;
using RahulAI.Data;
using RahulAI.Filter;
using Microsoft.AspNetCore.Authorization;

namespace RahulAI.Controllers
{
    /// <summary>
    /// Controller responsible for managing customer-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting customer information.
    /// </remarks>
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly RahulAIContext _context;

        public CustomerController(RahulAIContext context)
        {
            _context = context;
        }

        /// <summary>Adds a new customer to the database</summary>
        /// <param name="model">The customer data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Customer model)
        {
            _context.Customer.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of customers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of customers</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.Customer.AsQueryable();
            var result = FilterService<Customer>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific customer by its primary key</summary>
        /// <param name="entityId">The primary key of the customer</param>
        /// <returns>The customer data</returns>
        [HttpGet]
        [Route("{entityId:Guid}")]
        public IActionResult GetById([FromRoute] Guid entityId)
        {
            var entityData = _context.Customer.FirstOrDefault(entity => entity.CustomerId == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific customer by its primary key</summary>
        /// <param name="entityId">The primary key of the customer</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [Route("{entityId:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid entityId)
        {
            var entityData = _context.Customer.FirstOrDefault(entity => entity.CustomerId == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.Customer.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific customer by its primary key</summary>
        /// <param name="entityId">The primary key of the customer</param>
        /// <param name="updatedEntity">The customer data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{entityId:Guid}")]
        public IActionResult UpdateById(Guid entityId, [FromBody] Customer updatedEntity)
        {
            if (entityId != updatedEntity.CustomerId)
            {
                return BadRequest("Mismatched CustomerId");
            }

            var entityData = _context.Customer.FirstOrDefault(entity => entity.CustomerId == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(Customer).GetProperties().Where(property => property.Name != "CustomerId").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}