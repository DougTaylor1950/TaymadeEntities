using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Avalonia.Metadata;
using ReactiveUI;

namespace TaymadeEntities.Models
{
    public class PhraseHeader : ModelBase
    {
        #region Private Fields

        private string? description;

        #endregion Private Fields

        public override string ToString()
        {
            return Description ?? string.Empty;
        }   

        #region Public Properties
        [NotMapped]
        public virtual List<PhraseEntry> PhraseEntries { get; set; } = new List<PhraseEntry>();
        
        public new int Id { get; set; }
        public string? Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }

        #endregion Public Properties
    }
}