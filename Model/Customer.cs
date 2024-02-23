using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RahulAI.Model
{
    /// <summary> 
    /// Represents a customer entity with essential details
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Initializes a new instance of the Customer class.
        /// </summary>
        public Customer()
        {
            CountryName = "India";
        }

        /// <summary>
        /// Primary key for the Customer 
        /// </summary>
        [Key]
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Required field Name of the Customer 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// IsActive of the Customer 
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Address of the Customer 
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Foreign key referencing the Country to which the Customer belongs 
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// Navigation property representing the associated Country
        /// </summary>
        [ForeignKey("CountryName")]
        public Country? CountryNameCountry { get; set; }
        /// <summary>
        /// Collection navigation property representing associated 
        /// </summary>
        public ICollection<Order>? Orders { get; set; }
    }
}