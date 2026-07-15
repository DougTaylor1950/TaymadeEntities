using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TaymadeEntities.Support
{
    public class FileFoundEventArgs : EventArgs
    {
        #region Public Constructors

        public FileFoundEventArgs(string filePath)
        {
            FilePath = filePath;
        }

        #endregion Public Constructors

        #region Public Properties

        public System.IO.FileInfo? FileInfo { get; set; }
        public string FilePath { get; }

        #endregion Public Properties
    }

    /// <summary>
    /// Represents a file monitor that can detect changes to a specified file.
    /// Defaults to checking every 60 seconds, but can be set to check every X seconds, minutes or hours.
    /// Defaults are overridable by calling the SetInterval method.
    /// Can be cancelled by calling the Cancel method.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <author>
    /// Doug Taylor - Taymade Software Services
    /// </author>
    /// <remarks>
    ///   <created> 04/07/2026 21:49 </created>
    /// </remarks>
    public sealed class FileMonitor : IDisposable
    {
        #region Private Fields

        private readonly CancellationTokenSource _cts = new();

        #endregion Private Fields

        #region Public Events

        public event EventHandler<FileFoundEventArgs>? FileFound;

        public event EventHandler<FileFoundEventArgs>? NewFileFound;

        #endregion Public Events

        #region Public Enums

        public enum TimeUnits
        {
            Seconds,
            Minutes,
            Hours
        }

        #endregion Public Enums

        #region Public Properties

        public int Interval { get; set; } = 30;
        public TimeUnits IntervalUnit { get; set; } = TimeUnits.Seconds;

        public string? Path { get; set; }

        #endregion Public Properties

        #region Private Properties

        private TimeSpan period { get; set; } = TimeSpan.FromMinutes(1);

        #endregion Private Properties

        #region Public Methods

        public static System.IO.FileInfo? GetLatestFile(string folder)
        {
            // System.IO.FileInfo? info = null;

            var fileList = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

            System.IO.FileInfo info = (from file in fileList
                                       let fileInfo = new System.IO.FileInfo(file)
                                       where fileInfo.CreationTime > DateTime.Today - TimeSpan.FromDays(20)
                                       orderby fileInfo.CreationTime descending
                                       select fileInfo
                        ).First();

            return info;
        }

        // All process to be cancelled from the calling application
        public void Cancel()
        {
            _cts.Cancel();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        public async Task GetLatestFileAsync(string folder)
        {
            using var timer = new PeriodicTimer(period);

            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    System.IO.FileInfo? info = FileMonitorSupport.GetLatestFile(folder);

                    if (info != null)
                    {
                        NewFileFound?.Invoke(this, new FileFoundEventArgs(info.FullName));
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// waits for changes to specified file and raises the FileFound event when a change is detected.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 04/07/2026 04/07/2026 </created>
        /// </remarks>
        public async Task RunAsync(string path)
        {
            Path = path;

            using var timer = new PeriodicTimer(period);

            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    // get File Info and see if it has changed since last check
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(Path);
                    if (fileInfo.Exists)
                    {
                        // default is 60 seconds, but can be set to check every X seconds, minutes or hours.
                        if (fileInfo.LastWriteTime > DateTime.Now.AddSeconds(-Interval))
                        {
                            // could be expanded to look for changes in other file info properties
                            var eventArgs = new FileFoundEventArgs(fileInfo.FullName)
                            {
                                FileInfo = fileInfo
                            };
                            eventArgs.FileInfo = fileInfo;
                            FileFound?.Invoke(this, eventArgs);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Sets the interval for checking the file.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="unit">The unit.</param>
        /// <exception cref="ArgumentOutOfRangeException">unit - null</exception>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 06/07/2026 06/07/2026 </created>
        /// </remarks>
        public void SetInterval(int interval, TimeUnits unit)
        {
            Interval = interval;
            IntervalUnit = unit;
            switch (unit)
            {
                case TimeUnits.Seconds:
                    period = TimeSpan.FromSeconds(interval);
                    break;

                case TimeUnits.Minutes:
                    period = TimeSpan.FromMinutes(interval);
                    interval = interval * 60; // Convert minutes to seconds for the Interval property
                    break;

                case TimeUnits.Hours:
                    period = TimeSpan.FromHours(interval);
                    interval = interval * 3600; // Convert hours to seconds for the Interval property
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }

        #endregion Public Methods

    }

    public class FileMonitorSupport
    {
        #region Public Methods

        /// <summary>
        /// Gets the latest file in the specified folder.
        /// Limited at the moment to files created today, but can be changed to any date range.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        /// <author>
        /// Doug Taylor - Taymade Software Services
        /// </author>
        /// <remarks>
        ///   <created> 06/07/2026 06/07/2026 </created>
        /// </remarks>
        public static System.IO.FileInfo? GetLatestFile(string folder)
        {
            var fileList = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

            System.IO.FileInfo? info = (from file in fileList
                                        where !string.IsNullOrWhiteSpace(file)
                                        let fileInfo = new System.IO.FileInfo(file)
                                        where fileInfo.CreationTime > DateTime.Today
                                        orderby fileInfo.CreationTime descending
                                        select fileInfo
                        ).FirstOrDefault();

            return info;
        }

        #endregion Public Methods
    }
}