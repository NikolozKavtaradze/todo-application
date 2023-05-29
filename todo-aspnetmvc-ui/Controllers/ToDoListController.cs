using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Models;
using todo_domain_entities;
using todo_domain_entities.AggregateModels;
using todo_domain_entities.Repository;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly IToDoRepository repository;

        public ToDoListController(IToDoRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index(int? id,string filter = null)
        {
            id ??= repository.GetToDoLists().FirstOrDefault()?.Id;
            SelectedList.Id = id ?? 0;

            ViewBag.ListIsPresent = repository.GetToDoLists().Any();

            var filters = new Filters(filter);

            ViewBag.Filters = filters;
            ViewBag.Categories = repository.GetCategories();
            ViewBag.Statuses = repository.GetStatuses();
            ViewBag.DueFilters = Filters.DueFilterValue;

            var todoItems = repository.GetTodoItems(SelectedList.Id);
                

            if (filters.HasCategory)
            {
                todoItems = todoItems.Where(t => t.CategoryId == filters.CategoryId).ToList();
            }
            if (filters.HasStatus)
            {
                todoItems = todoItems.Where(t => t.StatusId == filters.StatusId).ToList();
            }
            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    todoItems = todoItems.Where(x => x.DueDate < today).ToList();
                }
                else if (filters.IsFuture)
                {
                    todoItems = todoItems.Where(x => x.DueDate > today).ToList();
                }
                else if (filters.IsToday)
                {
                    todoItems = todoItems.Where(x => x.DueDate == today).ToList();

                }
            }

            var tasks = todoItems.OrderBy(t => t.DueDate).ToList();

            return View(tasks);
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            ViewBag.Categories = repository.GetCategories();
            ViewBag.Statuses = repository.GetStatuses();
            var task = new ToDoItemViewModel
            {
                StatusId = "ongoing"
            };
            return View(task);
        }

        [HttpPost]
        public IActionResult AddTask(ToDoItemViewModel todoItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = repository.GetCategories();
                ViewBag.Statuses = repository.GetStatuses();
                return View(todoItem);
            }

            var task = new ToDoItem()
            {
                Description = todoItem.Description,
                CategoryId = todoItem.CategoryId,
                DueDate = todoItem.DueDate,
                StatusId = todoItem.StatusId,
                ToDoListId = SelectedList.Id,
            };

            repository.AddTask(task);
            return RedirectToAction("Index", new {id = SelectedList.Id});

        }
        [HttpGet]
        public IActionResult EditTask(int id)
        {
            ViewBag.Categories = repository.GetCategories();
            ViewBag.Statuses = repository.GetStatuses();
            var task = repository.GetTodoItems(SelectedList.Id).Find(x => x.Id == id);

            var todoItem = new ToDoItemViewModel()
            {
                Description = task.Description,
                StatusId = task.StatusId,
                CategoryId = task.CategoryId,
                DueDate= task.DueDate,
                Id = task.Id
            };

            return View(todoItem);
        }

        [HttpPost]
        public IActionResult EditTask(ToDoItemViewModel todoItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = repository.GetCategories().ToList();
                ViewBag.Statuses = repository.GetStatuses().ToList();
                return View(todoItem);
            }

            var task = repository.GetTodoItems(SelectedList.Id).Find(x => x.Id == todoItem.Id);

            task.Description = todoItem.Description;
            task.DueDate = todoItem.DueDate;
            task.StatusId = todoItem.StatusId;
            task.CategoryId = todoItem.CategoryId;
            repository.UpdateTask(task);

            return RedirectToAction("Index", new { id = SelectedList.Id });
        }

        [HttpGet]
        public IActionResult DeleteTask(int id)
        {
            repository.DeleteTask(id);

            return RedirectToAction("Index", new { id = SelectedList.Id });
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            
            string filterId = string.Join('-', filter);

            return RedirectToAction("Index", new {id = SelectedList.Id,filter = filterId });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string filterId, ToDoItem selected)
        {
            repository.MarkComplete(selected.Id);
            return RedirectToAction("Index", new {id = SelectedList.Id, filter = filterId });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string filterId)
        {
            repository.DeleteComplete(SelectedList.Id);

            return RedirectToAction("Index",new {id = SelectedList.Id, filter = filterId});
        }

        public IActionResult CreateList() => View();

        [HttpPost]
        public IActionResult CreateList(ToDoList list)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            repository.CreateList(list);
            var todoList = repository.GetToDoLists().LastOrDefault();

            SelectedList.Id = todoList.Id;
            return RedirectToAction("Index", new {id = SelectedList.Id});
        }


        public IActionResult EditList(int id)
        {
            ToDoList list = repository.GetToDoLists().Find(x => x.Id == id);

            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult EditList(ToDoList list)
        {
            if(!ModelState.IsValid)
            {
                return View(list);
            }

            repository.UpdateList(list);

            return RedirectToAction("Index", new {id = SelectedList.Id});
        }

        public IActionResult DeleteList(int id)
        {
            repository.DeleteList(id);

            return RedirectToAction("Index", new {id = SelectedList.Id});
        }
    }
}
