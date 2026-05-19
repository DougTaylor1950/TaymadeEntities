//-----------------------------------------------------------------------
// <copyright file="nfo.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>29/09/2022 13:41:19 29/09/2022 13:41:19 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using TaymadeEntities.Models;
    using ReactiveUI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;


    /// <summary>
    /// Defines the <see cref="Nfo" />.
    /// </summary>
    public class Nfo : ModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the myActorList.
        /// </summary>
        private ActorList? myActorList;

        /// <summary>
        /// Defines the myActors.
        /// </summary>
        private List<XElement>? myActors;

        /// <summary>
        /// Defines the myDirector.
        /// </summary>
        private string myDirector;

        /// <summary>
        /// Defines the myGenres.
        /// </summary>
        private IEnumerable<string>? myGenres;

        /// <summary>
        /// Defines the myGroup.
        /// </summary>
        private string myGroup = string.Empty;

        /// <summary>
        /// Defines the myIMDBid.
        /// </summary>
        private string myIMDBid = string.Empty;

        /// <summary>
        /// Defines the myNfoPath.
        /// </summary>
        private string myNfoPath = string.Empty;

        /// <summary>
        /// Defines the myPlot.
        /// </summary>
        private string myPlot;

        /// <summary>
        /// Defines the myStudio.
        /// </summary>
        private IEnumerable<string>? myStudio;

        /// <summary>
        /// Defines the myTagls.
        /// </summary>
        private List<XElement>? myTagls;

        /// <summary>
        /// Defines the myTags.
        /// </summary>
        private IEnumerable<string>? myTags;

        /// <summary>
        /// Defines the myTitle.
        /// </summary>
        private string myTitle;

        /// <summary>
        /// Defines the myXML.
        /// </summary>
        private XElement? myXML;

        /// <summary>
        /// Defines the myYear.
        /// </summary>
        private string myYear;
        private int? myTIMDBid;
        private NfoData? nFOData;
        private string language;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Nfo"/> class.
        /// </summary>
        /// <param name="nfopath">The nfopath<see cref="string"/>.</param>
        public Nfo(string nfopath)
        {
            NFOPath = nfopath;
            if (System.IO.File.Exists(nfopath))
            {
                try
                {
                    XML = XElement.Load(nfopath);
                }
                catch (System.Exception)
                {

                }

                
            }
            if (XML != null)
            {
                myTitle = GetValue("title");
                myYear = GetValue("year");
                myPlot = GetValue("plot");
                Studio = XML.Elements("studio").Select(x => x.Value);

                Tag = XML.Elements("tag").Select(x => x.Value);
                TagList = XML.Elements("tag").ToList();
                string tmp = Tags;
                Actors = XML.Elements("actor").ToList();
                Genre = XML.Elements("genre").Select(x => x.Value);
                myIMDBid = GetValue("id");
                myTIMDBid = GetValueInt("tmdbid");
                myDirector = GetValue("director");
            }
            else
            {
                XML = new XElement("movie");
                XML.Add(new XElement("title"));
                myTitle = "";
                XML.Add(new XElement("year"));
                myYear = "";
                XML.Add(new XElement("plot"));
                myPlot = "";
                XML.Add(new XElement("id"));
                myIMDBid = "";
                XML.Add(new XElement("director"));
                myDirector = "";
                if (System.IO.File.Exists(nfopath))
                    XML.Save(nfopath);
                Studio = XML.Elements("studio").Select(x => x.Value);
                Tag = XML.Elements("tag").Select(x => x.Value);
                Actors = XML.Elements("actor").ToList();
                TagList = XML.Elements("tag").ToList();
                if (TagList == null) { TagList = new List<XElement>(); }
                Genre = XML.Elements("genre").Select(x => x.Value);
            }
        }

        public Nfo(NfoData nfoData)
        {
            NFOData = nfoData;
            if (nfoData != null)
            {
                XML = nfoData.ToXML();
                Title = nfoData.Title;
                Year = nfoData.Year;
                Plot = nfoData.Plot;
                //if (nfoData.Companies != null) Studio = nfoData.Companies.DistinctBy(x => x.Name).Select(x => x.Name);
                Studio = XML.Elements("studio").DistinctBy(x => x.Value).Select(x => x.Value);
                Language = GetValue("language");
                Tag = XML.Elements("tag").Select(x => x.Value);
                TagList = XML.Elements("tag").ToList();
                string tmp = Tags;
                Actors = XML.Elements("actor").ToList();
                Genre = XML.Elements("genre").Select(x => x.Value);
                IMDBid = GetValue("id");
                TIMDBid = GetValueInt("tmdbid");
                Director = GetValue("director");
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ActorList.
        /// </summary>
        public ActorList ActorList
        {
            get
            {
                if ((myActorList == null || myActorList.Count == 0) && Actors != null && Actors.Count > 0)
                {
                    ActorList = new ActorList(Actors);
                }
                return myActorList;
            }

            set => this.RaiseAndSetIfChanged(ref myActorList, value);
        }

        /// <summary>
        /// Gets or sets the Actors.
        /// </summary>
        public List<XElement> Actors { get => myActors; set => myActors = value; }

        /// <summary>
        /// Gets or sets the Director.
        /// </summary>
        public string Director { get => myDirector; set => this.RaiseAndSetIfChanged(ref myDirector, value); }

        /// <summary>
        /// Gets or sets the Genre.
        /// </summary>
        public IEnumerable<string> Genre { get => myGenres; set => this.RaiseAndSetIfChanged(ref myGenres, value); }

        /// <summary>
        /// Gets the Genres.
        /// </summary>
        public string Genres => string.Join(",", Genre.ToArray());

        /// <summary>
        /// Gets or sets the IMDBid.
        /// </summary>
        public string IMDBid { get => myIMDBid; set => myIMDBid = value; }

        public int? TIMDBid { get => myTIMDBid; set => this.RaiseAndSetIfChanged(ref myTIMDBid, value); }

        /// <summary>
        /// Gets or sets the MovieGroup.
        /// </summary>
        public string MovieGroup { get => myGroup; set => myGroup = value; }

        public NfoData? NFOData { get => nFOData; set => nFOData = value; }

        /// <summary>
        /// Gets or sets the NFOPath.
        /// </summary>
        public string NFOPath { get => myNfoPath; set => myNfoPath = value; }

        /// <summary>
        /// Gets or sets the Plot.
        /// </summary>
        public string Plot { get => myPlot; set => myPlot = value; }

        /// <summary>
        /// Gets or sets the Studio.
        /// </summary>
        public IEnumerable<string?> Studio { get => myStudio; set => this.RaiseAndSetIfChanged(ref myStudio, value); }
        public string Language { get => language; set => this.RaiseAndSetIfChanged(ref language, value); }

        /// <summary>
        /// Gets the Studios.
        /// </summary>
        public string Studios => string.Join(",", Studio.ToArray());

        /// <summary>
        /// Gets or sets the Tag.
        /// </summary>
        public IEnumerable<string> Tag { get => myTags; set => this.RaiseAndSetIfChanged(ref myTags, value); }

        /// <summary>
        /// Gets or sets the TagList.
        /// </summary>
        public List<XElement>? TagList { get => myTagls; set => myTagls = value; }

        /// <summary>
        /// Gets the Tags.
        /// </summary>
        public string Tags
        {
            get => string.Join(",", Tag.ToArray());

            set
            {
                IEnumerable<string> list = value.Split(',').ToArray();
                Tag = list;

                TagList.Clear();
                foreach (var item in list)
                {
                    TagList.Add(new XElement("tag", item));
                }

            }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get => myTitle; set => this.RaiseAndSetIfChanged(ref myTitle, value); }

        /// <summary>
        /// Gets or sets the XML.
        /// </summary>
        public XElement XML { get => myXML; set => myXML = value; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public string Year { get => myYear; set => this.RaiseAndSetIfChanged(ref myYear, value); }

        /// <summary>
        /// Gets the Yearint.
        /// </summary>
        private int Yearint
        {
            get
            {
                int retValue = 0;
                int.TryParse(Year, out retValue);
                return retValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Save.
        /// </summary>
        public void Save()
        {
            if (NFOData == null)
            {
                XML = new XElement("movie");
                XML.Add(new XElement("title", myTitle));
                //myTitle = "";
                XML.Add(new XElement("year", myYear));
                //myYear = "";
                XML.Add(new XElement("plot", myPlot));
                //myPlot = "";
                XML.Add(new XElement("id", myIMDBid));

                XML.Add(new XElement("timdbid", myTIMDBid));

                //myIMDBid = "";
                XML.Add(new XElement("director", myDirector));
                //myDirector = "";

                if (TagList != null)
                    foreach (XElement tag in TagList)
                    {
                        XML.Add(tag);
                    }

                if (ActorList != null)
                {
                    foreach (Actor actor in ActorList)
                    {
                        XElement xActor = new XElement("actor");
                        XML.Add(xActor);
                        xActor.Add(new XElement("name", actor.Name));
                        xActor.Add(new XElement("role", actor.TmpRole));
                        xActor.Add(new XElement("thumb", actor.Thumb));
                        xActor.Add(new XElement("gender", actor.GenderDisplay));
                    }
                }
                XML.Save(NFOPath);
            }
            else
            {
                NFOData.Save(NFOPath);
            }
        }

        /// <summary>
        /// The ToHTML.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string ToHTML()
        {
            string html = "";
            //html = Form1.ResolveStyleSheet(ToXMLElement().ToString(), "FilmInfo.xslt", 0);
            return html;
        }

        /// <summary>
        /// The ToXMLElement.
        /// </summary>
        /// <returns>The <see cref="XElement"/>.</returns>
        public XElement ToXMLElement()
        {
            XElement Element = new XElement("NFO");

            Element.Add(new XElement("Title", Title));

            Element.Add(new XElement("Studios", Studios));
            Element.Add(new XElement("Genres", Genres));
            Element.Add(new XElement("Tags", Tags));
            Element.Add(new XElement("Plot", Plot));
            Element.Add(new XElement("Year", Yearint));
            Element.Add(new XElement("Director", Director));
            if (ActorList != null)
            {
                Element.Add(ActorList.ToXMLElement());
            }
            return Element;
        }

        /// <summary>
        /// The ToXMLElementFull.
        /// </summary>
        /// <returns>The <see cref="XElement"/>.</returns>
        public XElement ToXMLElementFull()
        {
            XElement Element = ToXMLElement();
            return Element;
        }

        public void SetValuesFromMovie(Movies movie)
        {
            if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(movie.MovieName)) Title = movie.MovieName;
            if (string.IsNullOrEmpty(Plot) && !string.IsNullOrEmpty(movie.Info)) Plot = movie.Info;

            if (TIMDBid == null && movie.TMDBID != null) TIMDBid = movie.TMDBID;

            if (ActorList == null) ActorList = new ActorList();

            if (movie.Casts != null && movie.Casts.Count > 0 )
            {
                foreach (Cast castMember in movie.Casts)
                {
                    if (castMember.Actor != null)
                    {
                        Actor actor = ActorList.Find(x => x.TMDBID == castMember.Actor.TMDBID);

                        if (actor == null ) // not present
                        {
                            actor = castMember.Actor;
                            ActorList.Add(actor);
                        }

                        if (string.IsNullOrEmpty(actor.TmpRole) && !string.IsNullOrEmpty(castMember.Role))
                            actor.TmpRole = castMember.Role;
                    }
                }
            }
        }

        /// <summary>
        /// The GetValue.
        /// </summary>
        /// <param name="elemName">The elemName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetValue(string elemName)
        {
            string retvalue = "";
            XElement? title = XML.Element(elemName);
            if (title != null) retvalue = title.Value;
            return retvalue;
        }

        private int? GetValueInt(string elemName)
        {
            int? retvalue = null;
            string temp = string.Empty;
            XElement? title = XML.Element(elemName);
            if (title != null) temp = title.Value;

            if (!string.IsNullOrEmpty(temp))
            {
                retvalue = int.Parse(temp);
            }

            return retvalue;
        }

        #endregion
    }
}
