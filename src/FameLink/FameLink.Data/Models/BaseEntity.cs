using System;
using System.ComponentModel.DataAnnotations;

namespace FameLink.Data.Models
{
    public class BaseEntity
    {
        [Key]
        [MaxLength(450)]
        public Guid Id { get; set; }
    }
}
