using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class StoryTransInfo
    {
        #region Public Constructors

        public StoryTransInfo()
        {
            // Default constructor

           
        }

        #endregion Public Constructors

        #region Public Properties

        public int CurrentStoryId { get; set; }
        public int Id { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void Load()
        {
            StoryTransInfo? st = DataController.SandboxEntities.StoryTransInfo.Where(st => st.Id == 1).FirstOrDefault();

            if (st != null)
            {
                Id = st.Id;
                CurrentStoryId = st.CurrentStoryId;
            }
            else
            {
                // If no translation info exists, initialize with default values
                Id = 1; // Default ID
                CurrentStoryId = 0; // Default story ID
            }
        }

        public void Insert()
        {
            // Insert the current story translation info into the database
            DataController.SandboxEntities.StoryTransInfo.Add(this);
            DataController.SandboxEntities.SaveChanges();
        }   
        public void Update()
        {
            // Update the current story translation info in the database

            DataController.SandboxEntities.UpdataStoryTransInfo(this);
        }

        #endregion Public Methods
    }
}
