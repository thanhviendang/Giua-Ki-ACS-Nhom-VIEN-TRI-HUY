using ASC.Web.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models
{
    public interface IIdentitySeed
    {
        Task Seed(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<ApplicationSettings> options);
    }
}
