using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RahulAI.Model
{
    /// <summary> 
    /// Represents a order entity with essential details
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Primary key for the Order 
        /// </summary>
        [Key]
        public Guid? OrderID { get; set; }
        /// <summary>
        /// Foreign key referencing the Customer to which the Order belongs 
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Navigation property representing the associated Customer
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        /// <summary>
        /// TotalAmount of the Order 
        /// </summary>
        public string? TotalAmount { get; set; }
        /// <summary>
        /// Foreign key referencing the OrderStatus to which the Order belongs 
        /// </summary>
        public Guid? OrderStatusId { get; set; }

        /// <summary>
        /// Navigation property representing the associated OrderStatus
        /// </summary>
        [ForeignKey("OrderStatusId")]
        public OrderStatus? OrderStatus { get; set; }
        /// <summary>
        /// Collection navigation property representing associated 
        /// </summary>
        public ICollection<OrderLine>? OrderLines { get; set; }
    }
}