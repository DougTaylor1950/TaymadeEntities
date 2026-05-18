using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonMVVM.Models
{
    public class MapDrive
    {

        public int Id { get; set; }

        public string? DriveName { get; set; }

        public string? Type { get; set; }

        public string? MapPath { get; set; }

        [NotMapped]
        public List<MapFolder> Folders { get; set; }

        [NotMapped]
        public MapFolder CurrentMapFolder { get; set; }
    }

    public class MapFolder : ModelBase
    {
        public string? FolderName { get; set; }

        public List<MissingFile>? MissingFiles
        {
            get => missingFiles;
            set
            {
                missingFiles = value;
                FileCount = MissingFiles.Count;
                this.RaisePropertyChanged("FileCount");
            }
        }

        private int? fileCount = 0;
        private List<MissingFile>? missingFiles;

        public int? FileCount
        {
            get
            {
                if (MissingFiles != null && fileCount != MissingFiles.Count)
                {
                    fileCount = MissingFiles.Count;
                    this.RaisePropertyChanged("FileCount");
                }
                return fileCount;
            }
            set
            {

                this.RaiseAndSetIfChanged(ref fileCount, value);
            }
                
        }
    }
}
