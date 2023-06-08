using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using todo_domain_entities.AggregateModels;

namespace todo_domain_entities
{
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "home", Name = "Home" },
                new Category { CategoryId = "work", Name = "Work"},
                new Category { CategoryId = "ex", Name = "Exercise"},
                new Category { CategoryId = "shop", Name = "Shopping"}
                );
            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = "notstarted", Name = "Not Started"},
                new Status { StatusId = "ongoing", Name = "In Progress"},
                new Status { StatusId = "finished", Name = "Completed"}
                );
            modelBuilder.Entity<ToDoList>().HasData(
                new ToDoList { Id = 1, Name = "Tasks"},
                new ToDoList { Id = 2, Name = "My Day"},
                new ToDoList { Id = 3, Name = "Important" },
                new ToDoList { Id = 4, Name = "Planned" }
                );
        }
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
