using Demo.Infrastructure;
using Demo.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Components
{
    /**
     * This class controlls the Navigation Menu, injects
     * current session and renders the bar with or without
     * login features
     */
    public class ToolbarViewComponent : ViewComponent
    {
        private User user;

        public ToolbarViewComponent(ISessionRepository session)
        {
            this.user = session.User;
        }

        public IViewComponentResult Invoke() => View(this.user);
    }
}
