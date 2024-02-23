using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RahulAI.Model
{
    /// <summary> 
    /// Represents a dictionary entity with essential details
    /// </summary>
    public class Dictionary
    {
        /// <summary>
        /// Primary key for the Dictionary 
        /// </summary>
        [Key]
        public Guid? Id { get; set; }

        /// <summary>
        /// Required field Name of the Dictionary 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Foreign key referencing the Dictionary to which the Dictionary belongs 
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Navigation property representing the associated Dictionary
        /// </summary>
        [ForeignKey("ParentId")]
        public Dictionary? ParentIdDictionary { get; set; }
    }
}