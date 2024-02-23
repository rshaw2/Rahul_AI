using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RahulAI.Model
{
    /// <summary> 
    /// Represents a country entity with essential details
    /// </summary>
    public class Country
    {
        /// <summary>
        /// CountryId of the Country 
        /// </summary>
        public Guid? CountryId { get; set; }
        /// <summary>
        /// Code of the Country 
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Primary key for the Country 
        /// </summary>
        [Key]
        public string? Name { get; set; }
        /// <summary>
        /// Collection navigation property representing associated 
        /// </summary>
        public ICollection<Customer>? Customers { get; set; }
    }
}