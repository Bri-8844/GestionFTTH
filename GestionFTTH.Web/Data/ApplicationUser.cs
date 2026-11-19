

using Microsoft.AspNetCore.Identity;

namespace GestionFTTH.Web.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? TrabajadorId { get; set; }
    }
}
