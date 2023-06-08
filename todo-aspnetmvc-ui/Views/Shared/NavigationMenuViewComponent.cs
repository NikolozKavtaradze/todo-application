using Microsoft.AspNetCore.Mvc;
using System.Linq;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Views.Shared
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public NavigationMenuViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
