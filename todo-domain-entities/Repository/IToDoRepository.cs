using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using todo_domain_entities.AggregateModels;

namespace todo_domain_entities.Repository
{
    public interface IToDoRepository
    {
        List<ToDoList> GetToDoLists();
        List<ToDoItem> GetTodoItems(int listId);
        List<Category> GetCategories();
        List<Status> GetStatuses();

        void CreateList(ToDoList list);
        void UpdateList(ToDoList list);
        void DeleteList(int id);

        void DeleteComplete(int listId);
        void MarkComplete(int id);

        void AddTask(ToDoItem task);
        void UpdateTask(ToDoItem task);

        void DeleteTask(int id);

        void Save();



    }
}
