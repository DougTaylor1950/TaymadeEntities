using TaymadeEntities.Support;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaymadeEntities.Models
{
    public class DownloadProperties
    {
        #region Public Properties

        public int Id { get; set; }

        public int? LastUnboundIndex { get; set; }
        public int? SortDirection { get; set; }

        public int? SortedColumn { get; set; }

        #endregion Public Properties

        #region Constructors
        public DownloadProperties()
        {
            Id = 0;
            LastUnboundIndex = null;
            SortDirection = null;
            SortedColumn = null;
        }

        #endregion Constructors

        // add update method
        public bool Update()
        {
            // Update logic here
            // update back to database
            bool success = true;
            try
            {
                if (Id == 0)
                {
                    success = Insert();
                }

                EntityState state = DataController.SandboxEntities.Entry(this).State;

                var local = DataController.SandboxEntities.Set<DownloadProperties>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                // check if local is not null
                if (local != null)
                {
                    // detach
                    DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                }
                // set Modified flag in your entry
                DataController.SandboxEntities.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DataController.SandboxEntities.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                string msg = "error Saving movie : " + Id.ToString() + " : " ;

                MVMLogs logs = new MVMLogs(ex, "database", "Error");
                success = false;
            }

            return success;
        }

        private bool Insert()
        {
            // Insert logic here
            bool success = true;
            try
            {
                DataController.SandboxEntities.DownloadProperties.Add(this);
                DataController.SandboxEntities.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                string msg = "error Inserting movie : " + Id.ToString() + " : ";
                MVMLogs logs = new MVMLogs(ex, "database", "Error");
                success = false;
            }

            return success;
        }
    }
}
