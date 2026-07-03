using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
                
        }

        public static bool Insert()
        {
            // Insert the current story translation info into the database
            return DataController.StoryController.AddStoryTransfer(_instance);
           
        }
        public static void Update()
        {
            // Update the current story translation info in the database
            if (_instance != null && _instance.Id == 0) Insert();
           
            DataController.StoryController.SaveStoryTransInfo(_instance);
        }

        #endregion Public Methods
    }
}
