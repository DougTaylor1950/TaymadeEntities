using System;
using System.Collections.Generic;
using System.Text;

namespace TaymadeEntities.Support
{
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

        public event EventHandler? FileFound;

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

        public int Interval { get; set; } = 60;
        public TimeUnits IntervalUnit { get; set; } = TimeUnits.Minutes;

        public string? Path { get; set; }

        #endregion Public Properties

        #region Private Properties

        private TimeSpan period { get; set; } = TimeSpan.FromMinutes(1);

        #endregion Private Properties

        #region Public Methods

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

        /// <summary>
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
                            FileFound?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

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
}