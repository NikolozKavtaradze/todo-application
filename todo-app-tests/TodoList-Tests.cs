using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using todo_domain_entities;
using todo_domain_entities.AggregateModels;
using todo_domain_entities.Repository;

namespace todo_app_tests
{
    [TestFixture]
    public class TodoListTests
    {
        private ToDoRepository _repository;
        private ToDoDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ToDoDbContext(options);
            _repository = new ToDoRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void CreateList_WhenCalled_ShouldAddListToContext()
        {
            // Arrange
            var list = new ToDoList
            {
                Id = 1,
                Name = "My List",
                ToDoItems = new List<ToDoItem>()
            };

            // Act
            _repository.CreateList(list);

            // Assert
            Assert.Contains(list, _context.ToDoLists.ToList());
        }
        [Test]
        public void UpdateList_WhenCalled_ShouldUpdateListInContext()
        {
            // Arrange
            var list = new ToDoList { Id = 1, Name = "List 1", ToDoItems = new List<ToDoItem>() };


            // Act
            list.Name = "Updated List";
            _repository.UpdateList(list);

            // Assert
            var updatedList = _repository.GetToDoLists().Find(x => x.Id == 1);
            Assert.AreEqual("Updated List", updatedList.Name);
        }

        [Test]
        public void DeleteList_WhenListExists_ShouldRemoveListFromContext()
        {
            // Arrange
            var list = new ToDoList { Id = 7, Name = "List 2", ToDoItems = new List<ToDoItem>() };
            _repository.CreateList(list);


            // Act
            _repository.DeleteList(7);

            // Assert
            var deletedList = _repository.GetTodoItems(1).Find(x => x.Id == 7);
            Assert.IsNull(deletedList);
        }

        [Test]
        public void DeleteComplete_WhenListAndCompleteTasksExist_ShouldRemoveCompleteTasksFromContext()
        {
            // Arrange
            var list = new ToDoList { Id = 10, Name = "List 10", ToDoItems = new List<ToDoItem>() };
            var task1 = new ToDoItem { Id = 7, Description = "Task 1", StatusId = "finished", ToDoListId = 10 };
            var task2 = new ToDoItem { Id = 8, Description = "Task 2", StatusId = "ongoing", ToDoListId = 10 };
            _repository.CreateList(list);
            _repository.AddTask(task1);
            _repository.AddTask(task2);


            // Act
            _repository.DeleteComplete(10);

            // Assert
            var deletedTask = _repository.GetTodoItems(10).Find(x => x.Id == 7);
            var remainingTask = _repository.GetTodoItems(10).Find(x => x.Id == 8);
            Assert.IsNull(deletedTask);
            Assert.IsNotNull(remainingTask);
        }

        [Test]
        public void GetToDoLists_WhenListsExist_ShouldReturnListOfToDoLists()
        {
            // Arrange
            var lists = new List<ToDoList>
            {
                new ToDoList { Id = 2, Name = "List 1", ToDoItems = new List<ToDoItem>() },
                new ToDoList { Id = 3, Name = "List 2", ToDoItems = new List<ToDoItem>() },
                new ToDoList { Id = 4, Name = "List 3", ToDoItems = new List<ToDoItem>() }
            };

            _repository.CreateList(lists[0]);
            _repository.CreateList(lists[1]);
            _repository.CreateList(lists[2]);

            _repository.Save();


            // Act
            var result = _repository.GetToDoLists().Where(x => x.Id > 1 && x.Id < 5).ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(lists.All(x => result.Contains(x)));
        }

        [Test]
        public void GetCategories_WhenCategoriesExist_ShouldReturnListOfCategories()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { CategoryId = "home", Name = "Home" },
            new Category { CategoryId = "work", Name = "Work" },
            new Category { CategoryId = "ex", Name = "Exercise" },
            new Category { CategoryId = "shop", Name = "Shopping" }
        };

            _context.Categories.AddRange(categories);

            _context.SaveChanges();

            // Act
            var result = _repository.GetCategories();

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(categories.All(x => result.Contains(x)));
        }

        [Test]
        public void GetStatuses_WhenStatusesExist_ShouldReturnListOfStatuses()
        {
            // Arrange
            var statuses = new List<Status>
            {
                new Status { StatusId = "notstarted", Name = "Not Started" },
                new Status { StatusId = "ongoing", Name = "In Progress" },
                new Status { StatusId = "finished", Name = "Completed" }
            };
            _context.Statuses.AddRange(statuses);
            _context.SaveChanges();

            // Act
            var result = _repository.GetStatuses();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(statuses.All(x => result.Contains(x)));
        }

        [Test]
        public void AddTask_WhenCalled_ShouldAddTaskToContext()
        {
            // Arrange
            var task = new ToDoItem { Id = 1, Description = "Task 1", StatusId = "notstarted", ToDoListId = 1 };

            // Act
            _repository.AddTask(task);

            // Assert
            Assert.Contains(task, _context.ToDoItems.ToList());
        }

        [Test]
        public void UpdateTask_WhenCalled_ShouldUpdateTaskInContext()
        {
            // Arrange
            var task = new ToDoItem { Id = 1, Description = "Task 1", StatusId = "notstarted", ToDoListId = 1 };

            // Act
            task.Description = "Updated Task";
            _repository.UpdateTask(task);

            // Assert
            var updatedTask = _repository.GetTodoItems(1).Find(x => x.Id == 1);
            Assert.AreEqual("Updated Task", updatedTask.Description);
        }

        [Test]
        public void DeleteTask_WhenTaskExists_ShouldRemoveTaskFromContext()
        {
            // Arrange
            var task = new ToDoItem { Id = 3, Description = "Task 3", StatusId = "notstarted", ToDoListId = 1 };
            _repository.AddTask(task);
            

            // Act
            _repository.DeleteTask(3);

            // Assert
            var deletedTask = _repository.GetTodoItems(1).Find(x => x.Id == 3);
            Assert.IsNull(deletedTask);
        }

    }
}