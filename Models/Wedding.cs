using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace wedding_planner.Models
{
    // this is to validate future dates     
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now)
                return new ValidationResult("Date must be in the future");
            return ValidationResult.Success;
        }
    }


    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Required]
        public string WedderOne { get; set; }
        [Required]
        public string WedderTwo { get; set; }
        [Required]
        [FutureDate(ErrorMessage = "Date must be a future date")]
        public DateTime Date { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        // Foreign Key to Wedding Creator
        public int UserId { get; set; } // this is how it connected to User Model
        public User Creator { get; set; }  /// not sure what this is ?
        // association (Many to Many)
        public List<Association> Associations { get; set; }  // why did we use this? and what questiosn we need to ask to put this here when starting from scratch?

    }


}