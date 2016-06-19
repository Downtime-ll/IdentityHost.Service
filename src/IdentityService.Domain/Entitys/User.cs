using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityService.Domain.Entitys
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public Sex Sex { get; set; }
    }

    public enum Sex
    {
        Man,
        Woman
    }
}
