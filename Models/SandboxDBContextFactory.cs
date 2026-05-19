using Microsoft.EntityFrameworkCore;
using SupportCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TaymadeEntities.Models
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        DbContext Create();
    }

    public class SandboxDBContextFactory 
    {
        public sandboxEntities Create()
        {
            return new sandboxEntities();
        }
    }
}
