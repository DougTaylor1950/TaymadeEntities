using CliWrap;
using CliWrap.EventStream;
using ShimSkiaSharp.Editing;
using SupportCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TaymadeEntities.Support
{
    public class VLCSupport
    {
        private VLCSupport? Instance { get; set; }
        public static string? ProcessOutput { get; private set; }
        public static int ExitCode { get; private set; } = 1;

        private VLCSupport()
        {
        }

        public static async Task<int> DoCliWrapPlay(string param)
        {
            string os = Support.GetOS();
            string filepath = string.Empty;
            if (os == "WinNT")
            {
                filepath = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
            }
            else
            {
                filepath = "/snap/bin/vlc";
            }

            int errorCode = 0;

            if (!string.IsNullOrEmpty(param))
            {
                char firstChar = param[0];
                if (firstChar != '"')
                {
                    // wrap param in double quotes
                    param = '"' + param + '"';
                }
                var cmd = Cli.Wrap(filepath)
                    .WithArguments(param);

                try
                {
                    await foreach (var cmdEvent in cmd.ListenAsync(System.Text.Encoding.Default))
                    {
                        switch (cmdEvent)
                        {
                            case StartedCommandEvent started:
                                Console.WriteLine($"Process started; ID: {started.ProcessId}");
                                break;

                            case StandardOutputCommandEvent stdOut:
                                //_output.WriteLine($"Out> {stdOut.Text}");
                                // process received data
                                string output = stdOut.Text;
                                Debug.WriteLine(output);
                                if (output.Contains("Percent="))
                                {
                                    //ProcessOutput = output;
                                }
                                else if (output.Contains("New Bookmark"))
                                {
                                    //DoReloadBookmarks();
                                    //CurrentBookmark = CurrentMovieModel.Bookmarks.Last();
                                }
                                else if (output.Contains("Bookmark Image"))
                                {
                                    int pos = output.IndexOf("Id=");
                                    if (pos >= 0)
                                    {
                                        string id = output.Substring(pos + 3);
                                        pos = id.IndexOf("|");
                                        if (pos >= 0)
                                        {
                                            string path = id.Substring(pos + 6);
                                            id = id.Substring(0, pos);

                                            if (int.TryParse(id, out int bmId))
                                            {
                                                //if (CurrentBookmark.Id == bmId)
                                                //{
                                                //    CurrentBookmark.ImagePath = path;
                                                //    CurrentBookmark.SetImageBMP();
                                                //}
                                            }
                                        }
                                    }
                                }
                                else
                                    ProcessOutput = output;

                                break;

                            case StandardErrorCommandEvent stdErr:
                                string eoutput = stdErr.Text;
                                Debug.WriteLine(eoutput);
                                CliWrapProgressEventArgs cliWrapProgress = new CliWrapProgressEventArgs(0, null)
                                {
                                    Progress = ProcessOutput,
                                    TaskName = "PlayVLC"
                                };                            //OnCliWrapProgress(cliWrapProgress);
                                break;

                            case ExitedCommandEvent exited:
                                ExitCode = exited.ExitCode;
                                errorCode = ExitCode;
                                ProcessOutput = $"Process exited; Code: " + exited.ExitCode.ToString();
                                //CliWrapCompletedEventArgs eventArgs = new CliWrapCompletedEventArgs(null, false, null)
                                //{
                                //    Result = ExitCode,
                                //    TaskName = "PlayVLC",
                                //    MovieName = MovieName
                                //};

                                //OnCliWrapComplete(eventArgs);
                                //DoReloadBookmarks();
                                //CurrentMovieModel.SetPercentUnmarked();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Support.GenerateInfoAndLogMessage("FFMpeg", "Movie", 0, ex.ToString());
                    ProcessOutput = $"Process errored ; " + ex.Message + " see log file";
                    //CliWrapErrorEventArgs cliWrapProgress = new CliWrapErrorEventArgs(ex, null, action);

                    // OnCliWrapError(cliWrapProgress);
                    errorCode = -1;  // indicate there has been an error.
                                     //throw;
                }

                return errorCode;
            }
            else return -1;
        }

        public VLCSupport GetInstance()
        {
            if (Instance == null)
                Instance = new VLCSupport();
            return Instance;
        }
    }
}