using Microsoft.AspNetCore.Mvc;
using System.Linq;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Views.Shared
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly ToDoDbContext _context;

        public NavigationMenuViewComponent(ToDoDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View(_context.ToDoLists.OrderBy(x => x.Id));
        }
    }
}
