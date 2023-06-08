using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using todo_domain_entities.AggregateModels;

namespace todo_domain_entities.Repository
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoDbContext _context;

        public ToDoRepository(ToDoDbContext context)
        {
            _context = context;
        }

        public void CreateList(ToDoList list)
        {
            _context.ToDoLists.Add(list);
            Save();
        }
        public void UpdateList(ToDoList list)
        {
            _context.Entry<ToDoList>(list).State = EntityState.Modified;
            Save();

        }
        public void DeleteList(int id)
        {
            ToDoList list = _context.ToDoLists.Find(id);
            if (list != null)
            {
                var tasks = _context.ToDoItems.Where(x => x.ToDoListId == id);
                _context.ToDoItems.RemoveRange(tasks);
                _context.ToDoLists.Remove(list);
                Save();
            }
        }

        public void DeleteComplete(int listId)
        {
            var tasksToDelete = _context.ToDoItems.Where(t => t.ToDoListId == listId && t.StatusId == "finished").ToList();

            _context.ToDoItems.RemoveRange(tasksToDelete);
            Save();
        }

        public void AddTask(ToDoItem task)
        {
            _context.ToDoItems.Add(task);
            Save();
        }
        public void UpdateTask(ToDoItem task)
        {
            _context.Entry(task).State = EntityState.Modified;
            Save();
        }

        public void DeleteTask(int id)
        {
            var task = _context.ToDoItems.Find(id);
            _context.Remove(task);
            Save();
        }


        public void MarkComplete(int id)
        {
            var selected = _context.ToDoItems.Find(id);

            if (selected != null)
            {
                selected.StatusId = "finished";
                Save();
            }
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public List<Status> GetStatuses()
        {
            return _context.Statuses.ToList();

        }

        public List<ToDoItem> GetTodoItems(int listId)
        {
            var todoItems = _context.ToDoItems
             .Include(x => x.Category)
             .Include(x => x.Status)
             .Include(x => x.ToDoList)
             .Where(x => x.ToDoListId == listId).ToList();

            return todoItems;
        }

        public List<ToDoList> GetToDoLists()
        {
            return _context.ToDoLists.Include(x => x.ToDoItems).ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
