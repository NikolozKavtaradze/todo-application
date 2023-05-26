using System;
using System.Collections.Generic;
using System.Text;

namespace todo_domain_entities.AggregateModels
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<ToDoItem> ToDoItems { get; set; }

    }
}
