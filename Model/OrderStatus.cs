using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RahulAI.Model
{
    /// <summary> 
    /// Represents a orderstatus entity with essential details
    /// </summary>
    public class OrderStatus
    {
        /// <summary>
        /// Primary key for the OrderStatus 
        /// </summary>
        [Key]
        public Guid? OrderStatusId { get; set; }
        /// <summary>
        /// Name of the OrderStatus 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Collection navigation property representing associated 
        /// </summary>
        public ICollection<Order>? Orders { get; set; }
    }
}