using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using todo_domain_entities;
using todo_domain_entities.Repository;

namespace todo_aspnetmvc_ui.Views.Shared
{
    public class SideBarMenuViewComponent : ViewComponent
    {
        private readonly IToDoRepository repo;

        public SideBarMenuViewComponent(IToDoRepository repo)
        {
            this.repo = repo;
        }

        public IViewComponentResult Invoke()
        {
            if (RouteData?.Values["id"] == null)
                ViewBag.SelectedCategory = 1;
            else
                ViewBag.SelectedCategory = Convert.ToInt32(RouteData?.Values["id"]);

            return View(repo.GetToDoLists());
        }
    }
}
