using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using todo_domain_entities.AggregateModels;

namespace todo_aspnetmvc_ui.Models
{
    public class ToDoItemViewModel
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Please enter a description!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a dueDate!")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Please select a category!")]
        public string CategoryId { get; set; } = string.Empty;

        [ValidateNever]
        public Category Category { get; set; } = null!;

        [Required(ErrorMessage = "Please select a status!")]
        public string StatusId { get; set; } = string.Empty;

        [ValidateNever]
        public Status Status { get; set; }

    }
}
