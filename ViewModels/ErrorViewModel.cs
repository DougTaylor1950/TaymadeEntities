using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaymadeEntities.Models;
using ReactiveUI;
using TaymadeEntities.Models;
using DataController = TaymadeEntities.Models.DataController;

namespace TaymadeEntities.ViewModels
{
    public class ErrorViewModel : DialogModelBase
    {
        #region Private Fields

        private MVMLogs? currentLog;
        private ObservableCollection<MVMLogs>? errorLogs;

        #endregion Private Fields

        #region Public Constructors

        public ErrorViewModel()
        {
           
            ErrorLogs = new(DataController.SandboxEntities.MVMLogs.OrderByDescending(e=>e.CreatedOn).ToList());
        }

        #endregion Public Constructors

        #region Public Properties

        public MVMLogs? CurrentLog 
        { 
            get => currentLog;
            set => this.RaiseAndSetIfChanged(ref currentLog, value); 
        }

        public ObservableCollection<MVMLogs>? ErrorLogs
        { 
            get => errorLogs; 
            set => this.RaiseAndSetIfChanged(ref errorLogs, value);
        }

        public int ErrorCount {
            get
            {
                int returnValue = 0;
                if (ErrorLogs != null)
                    returnValue = ErrorLogs.Count;
                return returnValue;
            }
          }

        #endregion Public Properties

        public void DeleteError()
        {
            if (this.CurrentLog != null)
            {
                CurrentLog.Delete();
                this.ErrorLogs.Remove(CurrentLog);

                if (this.ErrorLogs.Count > 0) CurrentLog = this.ErrorLogs.FirstOrDefault();
            }
        }
    }
}