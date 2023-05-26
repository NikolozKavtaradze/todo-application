using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Models;
using todo_domain_entities;
using todo_domain_entities.AggregateModels;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly ToDoDbContext _context;

        public ToDoListController(ToDoDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index(int? id,string filter = null)
        {
            id ??= _context.ToDoLists.FirstOrDefault()?.Id;
            SelectedList.Id = id ?? 0;

            ViewBag.ListIsPresent = _context.ToDoLists.Any();

            var filters = new Filters(filter);

            ViewBag.Filters = filters;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValue;

            var todoItems = await _context.ToDoItems
                .Include(x => x.Category)
                .Include(x => x.Status)
                .Include(x => x.ToDoList)
                .Where(x => x.ToDoListId == id).ToListAsync();

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
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            var task = new ToDoItemViewModel
            {
                StatusId = "ongoing"
            };
            return View(task);
        }

        [HttpPost]
        public async Task<ActionResult> AddTask(ToDoItemViewModel todoItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Statuses = _context.Statuses.ToList();
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

            _context.ToDoItems.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new {id = SelectedList.Id});

        }
        [HttpGet]
        public IActionResult EditTask(int id)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            var task = _context.ToDoItems.Find(id);

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
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Statuses = _context.Statuses.ToList();
                return View(todoItem);
            }

            var task = _context.ToDoItems.Find(todoItem.Id);

            task.Description = todoItem.Description;
            task.DueDate = todoItem.DueDate;
            task.StatusId = todoItem.StatusId;
            task.CategoryId = todoItem.CategoryId;
            _context.SaveChanges();

            return RedirectToAction("Index", new { id = SelectedList.Id });
        }

        [HttpGet]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var task = _context.ToDoItems.Find(id);

            _context.Remove(task);
            await _context.SaveChangesAsync();

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
            selected = _context.ToDoItems.Find(selected.Id);

            if (selected != null)
            {
                selected.StatusId = "finished";
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new {id = SelectedList.Id, filter = filterId });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string filterId)
        {
            var tasksToDelete = _context.ToDoItems.Where(t => t.ToDoListId == SelectedList.Id && t.StatusId == "finished").ToList();

            _context.ToDoItems.RemoveRange(tasksToDelete);
            _context.SaveChanges();


            return RedirectToAction("Index",new {id = SelectedList.Id, filter = filterId});
        }

        public IActionResult CreateList() => View();

        //POST /todo/create
        [HttpPost]
        public async Task<ActionResult> CreateList(ToDoList list)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            _context.ToDoLists.Add(list);
            await _context.SaveChangesAsync();

            var todoList = _context.ToDoLists.ToList().LastOrDefault();

            SelectedList.Id = todoList.Id;
            return RedirectToAction("Index", new {id = SelectedList.Id});
        }

        //GET /todo/edit/5

        public async Task<ActionResult> EditList(int id)
        {
            ToDoList list = await _context.ToDoLists.FindAsync(id);

            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }

        //POST /todo/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> EditList(ToDoList list)
        {
            if(!ModelState.IsValid)
            {
                return View(list);
            }
            
            _context.Entry<ToDoList>(list).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["Success"] = "The list has been updated!";
            return RedirectToAction("Index");
        }

        //GET /todo/delete/5
        public async Task<ActionResult> DeleteList(int id)
        {
            ToDoList list = await _context.ToDoLists.FindAsync(id);

            if (list == null)
            {
                TempData["Error"] = "the list does not exist";
            }
            else
            {
                var tasks = _context.ToDoItems.Where(x => x.ToDoListId == id);
                _context.ToDoItems.RemoveRange(tasks);
                _context.ToDoLists.Remove(list);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The list has been deleted!";
            }

            return RedirectToAction("Index");
        }
    }
}
