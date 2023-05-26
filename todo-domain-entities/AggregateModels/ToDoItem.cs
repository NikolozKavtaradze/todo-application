using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace todo_domain_entities.AggregateModels
{
    public class ToDoItem
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public string StatusId { get; set; }

        public Status Status { get; set; }

        public int ToDoListId { get; set; }

        public ToDoList ToDoList { get; set; }

        public bool Overdue => StatusId == "ongoing" && DueDate < DateTime.Today;

    }
}
