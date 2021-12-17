using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;

namespace ASC.Web.Data
{
    public class ApplicationDbContext : IdentityCloudContext
    {
      
        public ApplicationDbContext(IdentityConfiguration config) : base(config) { }
    }
}
