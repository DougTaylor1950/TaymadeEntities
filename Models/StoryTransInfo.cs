using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaymadeEntities.Models
{
    public class StoryTransInfo
    {
        #region Public Constructors

        private StoryTransInfo()
        {
            // Default constructor

            _instance = this;
        }

        #endregion Public Constructors

        #region Public Properties

        public int CurrentStoryId { get; set; }
        public int Id { get; set; }

        private static StoryTransInfo? _instance;

        #endregion Public Properties

        #region Public Methods

        public static StoryTransInfo GetInstance()
        {
            if (_instance == null )
            {
                Load();
            }
            return _instance;
        }
        private static void Load()
        {
            _instance = DataController.StoryController.GetStoryTransInfo();
                //DataController.SandboxEntities.StoryTransInfo.Where(st => st.Id == 1).FirstOrDefault();

            //if (st != null)
            //{
            //    StoryTransInfo.Id = st.Id;
            //    StoryTransInfo.CurrentStoryId = st.CurrentStoryId;
            //}
            //else
            //{
            //    // If no translation info exists, initialize with default values
            //    Id = 1; // Default ID
            //    CurrentStoryId = 0; // Default story ID
            //}
        }

        //public void Insert()
        //{
        //    // Insert the current story translation info into the database
        //    DataController.SandboxEntities.StoryTransInfo.Add(this);
        //    DataController.SandboxEntities.SaveChanges();
        //}   
        public static void Update()
        {
            // Update the current story translation info in the database

            DataController.StoryController.SaveStoryTransInfo(_instance);
        }

        #endregion Public Methods
    }
}
