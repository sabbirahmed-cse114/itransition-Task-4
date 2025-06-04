using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFour.Data;

namespace TaskFour.Models
{
    public class UserActionFilter
    {
        private readonly RequestDelegate _nextAction;

        public UserActionFilter(RequestDelegate nextAction)
        {
            _nextAction = nextAction;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var email = context.User.Identity.Name;
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    var deletedUser = await dbContext.DeletedUsers.FirstOrDefaultAsync(d => d.Id.ToString() == context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (deletedUser != null)
                    {
                        dbContext.DeletedUsers.Remove(deletedUser);
                        await dbContext.SaveChangesAsync();
                    }
                    await context.SignOutAsync();
                    context.Response.Redirect("/Account/Login");
                    return;
                }
                if (user.IsBlocked)
                {
                    await context.SignOutAsync();
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }
            await _nextAction(context);
        }
    }
}
