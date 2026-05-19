using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaymadeEntities.Models
{
    public class Chapter
    {
        private double time;

        public Chapter(string contents)
        {
            if (!string.IsNullOrEmpty(contents))
            {
                GetTimeBase(contents);
                GetTitle(contents);
                GetStart(contents);
                GetEnd(contents);
                double time = Time;
                Found = true;
            }
            else Found = false;
        }

        public Chapter()
        {
        }

        public string? TimeBase { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public string? Title { get; set; }

        public bool Found { get; set; }

        public double Time 
        { 
            get 
            { if (Start > 0 )
                {
                    if (TimeBase != null && TimeBase == "1/1000")
                    {
                        time = Start / 1000;
                    }
                    else time = Start;
                }
                return time;
            }

            set => time = value; }

        internal void GetTimeBase(string contents)
        {
            int pos = contents.IndexOf("TIMEBASE=");

            if (pos >= 0)
            {
                string temp = contents.Substring(pos + 9);
                pos = temp.IndexOf('\n');
                if (pos >= 0)
                {
                    TimeBase = temp.Substring(0, pos);
                }
            }
        }

        internal void GetTitle(string contents)
        {
            int pos = contents.ToUpper().IndexOf("TITLE=");

            if (pos >= 0)
            {
                string temp = contents.Substring(pos + 6);
                pos = temp.IndexOf('\n');
                if (pos >= 0)
                {
                    Title = temp.Substring(0, pos);
                }
            }
        }

        internal void GetStart(string contents)
        {
            int pos = contents.ToUpper().IndexOf("START=");

            if (pos >= 0)
            {
                string temp = contents.Substring(pos + 6);
                pos = temp.IndexOf('\n');
                if (pos >= 0)
                {
                    temp = temp.Substring(0, pos);



                    if (int.TryParse(temp, out int tstart))
                    {
                        Start = tstart;
                    }
                }
            }
        }

        internal void GetEnd(string contents)
        {
            int pos = contents.ToUpper().IndexOf("END=");

            if (pos >= 0)
            {
                string temp = contents.Substring(pos + 4);
                pos = temp.IndexOf('\n');
                if (pos >= 0)
                {
                    temp = temp.Substring(0, pos);



                    if (int.TryParse(temp, out int tstart))
                    {
                        End = tstart;
                    }
                }
            }
        }
    }
}
