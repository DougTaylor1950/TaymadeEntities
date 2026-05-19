using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaymadeEntities.Support
{
    /// <summary>
    /// </summary>
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 01/03/2026 11:08 </created>
    /// </remarks>
    public class FileDialogs
    {
        #region Public Methods

        /// <summary>
        /// Opens the name of the file.
        /// </summary>
        /// <param name="topLevel">The top level.</param>
        /// <returns></returns>
        public static async Task<string?> OpenFileName(TopLevel topLevel, string directory)
        {
            string? returnFileName = null;

            // Get top level from the current control. Alternatively, you can use Window reference instead.

            IStorageFolder? folder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(new Uri(directory));

            // Start async operation to open the dialog.
            var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = false,
                SuggestedStartLocation = folder
            });

            if (file is not null && file.Count > 0)
            {
                returnFileName = file[0].Name;
                directory = file[0].Path.LocalPath;
            }
            return directory;
        }

        /// <summary>
        /// Opens the name of the folder.
        /// </summary>
        /// <param name="topLevel">The top level.</param>
        /// <returns></returns>
        public static async Task<string?> OpenFolderName(TopLevel topLevel, string? directory = "")
        {
            string? returnFolderName = null;

            // Get top level from the current control. Alternatively, you can use Window reference instead.

            IStorageFolder? folder = null;
            IReadOnlyList<IStorageFolder>? foundFile = null;
            if (!string.IsNullOrEmpty(directory))
            {
                folder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(new Uri(directory));
                // Start async operation to open the dialog.
                var file = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Open Folder",
                    AllowMultiple = false,
                    SuggestedStartLocation = folder
                });
                foundFile = file;
            }
            else
            {
                var file = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Open Folder",
                    AllowMultiple = false
                });
                foundFile = file;
            }

            if (foundFile is not null && foundFile.Count > 0)
            {
                returnFolderName = foundFile[0].Path.LocalPath;
            }
            return returnFolderName;
        }

        #endregion Public Methods
    }
}