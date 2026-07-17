using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaymadeEntities.DBContext;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TaymadeEntities.Models
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        DbContext Create();
    }

    public class SandboxDBContextFactory 
    {
        public SandboxEntities Create()
        {
            return new SandboxEntities();
        }
    }
}
