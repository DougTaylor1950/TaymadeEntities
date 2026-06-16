using System;

namespace TaymadeEntities.Models
{
    public class MVMLogs
    {
        #region Public Constructors

        public MVMLogs(string? logger, string? message, string? level)
        {
            Logger = logger;
            Message = message;
            Level = level;

            CreatedOn = DateTime.UtcNow;
        }

        public MVMLogs(Exception ex, string? logger, string? level, string? url = "")
        {
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            Exception = ex.ToString();

            Logger = logger;
            Level = level;

            CreatedOn = DateTime.UtcNow;
            Url = url;
        }

        #endregion Public Constructors

        #region Public Properties

        public System.DateTime? CreatedOn { get; set; }
        public string? Exception { get; set; }
        public int Id { get; set; }

        public string? Level { get; set; }
        public string? Logger { get; set; }

        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Url { get; set; }



        #endregion Public Properties

        #region Internal Methods

        public void Delete()
        {
            DataController.MaintenaceController.DeleteLog(this.Id);
        }

        public void Insert()
        {
            DataController.MaintenaceController.InsertLog(this);
        }

        #endregion Internal Methods
    }
}