//-----------------------------------------------------------------------
// <copyright file="ActorPartial.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>16/07/2020 11:10:51 16/07/2020 11:10:51 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Models
{
    using TaymadeEntities.Support;
    using Microsoft.EntityFrameworkCore;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Actor" />.
    /// </summary>
    [MetadataType(typeof(ActorMetadata))]
    public partial class Actor : ModelBase
    {
        #region Fields

        /// <summary>
        /// Defines the age.
        /// </summary>
        private int? age;

        /// <summary>
        /// Defines the bMPVisible.
        /// </summary>
        private bool? bMPVisible;

        /// <summary>
        /// Defines the genderDisplay.
        /// </summary>
        private string? genderDisplay;

        /// <summary>
        /// Defines the genderValue.
        /// </summary>
        private PhraseEntry? genderValue;

        /// <summary>
        /// Defines the imageBMP.
        /// </summary>
        private Avalonia.Media.Imaging.Bitmap? imageBMP;

        /// <summary>
        /// Defines the movies.
        /// </summary>
        private List<Movies> movies;

        /// <summary>
        /// Defines the nameList.
        /// </summary>
        private string[] nameList;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ActorAge.
        /// </summary>
        public int? ActorAge => GetAge();

        /// <summary>
        /// Gets or sets a value indicating whether BMPVisible.
        /// </summary>
        [NotMapped]
        public bool BMPVisible
        {
            get
            {
                if (bMPVisible != null)
                    return bMPVisible.Value;
                else return true;
            }
            set => this.RaiseAndSetIfChanged(ref bMPVisible, value);
        }

        /// <summary>
        /// Gets the DiedAged.
        /// </summary>
        public int? DiedAged
        {
            get
            {
                if (DOB != null && DeathDay != null)
                {
                    double days = DeathDay.Value.Subtract(DOB.Value).TotalDays;
                    int years = (int)(days / 365.25);
                    return years;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the GenderDisplay
        /// Gets the GenderDisplay...
        /// </summary>
        [NotMapped]
        public string GenderDisplay
        {
            get
            {
                genderDisplay = SetGenderDisplay();

                return genderDisplay;
            }

            set => this.RaiseAndSetIfChanged(ref genderDisplay, value);
        }

        /// <summary>
        /// Gets or sets the GenderValue.
        /// </summary>
        [NotMapped]
        public PhraseEntry? GenderValue
        {
            get
            {
                if (genderValue != null && Gender != null)
                {
                    genderValue = DataController.GenderList.Find(x => x.Order == Gender);

                }

                return genderValue;
            }
            set => this.RaiseAndSetIfChanged(ref genderValue, value);
        }

        /// <summary>
        /// Gets or sets the ImageBMP.
        /// </summary>
        [NotMapped]
        public Avalonia.Media.Imaging.Bitmap? ImageBMP
        {
            get
            {
                SetImageBMP();
                return imageBMP;
            }
            set => this.RaiseAndSetIfChanged(ref imageBMP, value);
        }

        /// <summary>
        /// Gets or sets the Movies.
        /// </summary>
        [NotMapped]
        public List<Movies> Movies
        {
            get
            {
                if (movies == null && Casts != null)
                {
                    movies = [];

                    foreach (var item in Casts)
                    {
                        Movies? movie = DataController.SandboxEntities.Movies.Find(item.MovieID);
                        if (movie != null)
                            movies.Add(movie);
                    }
                }
                return movies;
            }

            set => this.RaiseAndSetIfChanged(ref movies, value);
        }

        /// <summary>
        /// Gets the Names.
        /// </summary>
        public string[] Names
        {
            get
            {
                nameList = Name.Split([' ']);
                if (string.IsNullOrEmpty(SortName))
                {
                    SortName = string.Empty;
                    for (int i = nameList.Length; i > 0; i--)
                    {
                        if (!string.IsNullOrEmpty(SortName)) SortName += ", ";
                        SortName += nameList[i - 1];
                    }
                }
                return nameList;
            }
        }

        /// <summary>
        /// Gets or sets the TmpRole.
        /// </summary>
        [NotMapped]
        public string? TmpRole { get; internal set; }
        public Cast? Parent { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// The GetAge.
        /// </summary>
        /// <returns>The <see cref="int?"/>.</returns>
        public int? GetAge()
        {
            if (DOB != null)
            {
                double years = DateTime.Today.Subtract(DOB.Value).TotalDays / 365;
                age = (int)years;
            }
            return age;
        }

        /// <summary>
        /// <br />.
        /// </summary>
        public void GetDetailsFromTMDB()
        {
            if (TMDBID != null && TMDBID > 0)
            {
                Person person = TmdbSupport.GetPerson(TMDBID.Value);

                if (person != null)
                {
                    SetDetailsFromPerson(person);
                }
            }
        }

        /// <summary>
        /// The SetDetailsFromCastMember.
        /// </summary>
        /// <param name="person">The person<see cref="CastMember"/>.</param>
        public void SetDetailsFromCastMember(CastMember person)
        {
            if (person != null)
            {
                if (!string.IsNullOrEmpty(person.Name)) Name = person.Name;
                if (person.Gender > 0) Gender = person.Gender;
                if (person.BirthDate > DateTime.MinValue) DOB = person.BirthDate;
                if (person.Adult) Adult = person.Adult;
                if (person.DeathDate > DateTime.MinValue) DeathDay = person.DeathDate;
                if (person.KnownAs != null && person.KnownAs.Length > 0) Aliases = string.Join(",", person.KnownAs);
                //if (!string.IsNullOrEmpty(person.) && string.IsNullOrEmpty(Info)) Info = person.Biography;

                //if (TMDBID == null && int.TryParse(person.ID, out int id))
                //{
                TMDBID = person.ID;
                //}

                const string defaultimage = @"\id-0.jpg";


                if (string.IsNullOrEmpty(IMDB) && !string.IsNullOrEmpty(person.IMDB)) IMDB = person.IMDB;

                if (!string.IsNullOrEmpty(person.PlaceOfBirth) && string.IsNullOrEmpty(PlaceOfBirth)) PlaceOfBirth = person.PlaceOfBirth;

                if (!string.IsNullOrEmpty(person.Profile_path) && string.IsNullOrEmpty(Thumb)) Thumb = person.Profile_path;

                // Id may not be known at this stage.
                if (Id > 0 && !string.IsNullOrEmpty(person.Profile_path) && (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(ImagePath) || imagePath.Contains(defaultimage)))
                {
                    Avalonia.Media.Imaging.Bitmap temp = TmdbSupport.GetImageFromProfile(person.Profile_path);

                    if (temp != null)
                    {
                        ImagePath = @"k:\TD1\MovieImages\ActorImages\id-" + Id.ToString().Trim() + ".jpg";

                        if (!System.IO.File.Exists(imagePath))
                        {
                            temp.Save(Support.FixImagePath(ImagePath));
                        }
                        SetImageBMP();
                    }
                    //this.Save();
                }
            }
        }

        /// <summary>
        /// The SetDetailsFromPerson.
        /// </summary>
        /// <param name="person">The person<see cref="Person"/>.</param>
        public void SetDetailsFromPerson(Person person)
        {
            if (person != null)
            {
                if (!string.IsNullOrEmpty(person.Name) && (string.IsNullOrEmpty(Name) || Name == "Blank")) Name = person.Name;
                if (person.Gender > 0) Gender = person.Gender;
                if (person.DateOfBirth > DateTime.MinValue) DOB = person.DateOfBirth;
                if (person.Adult) Adult = person.Adult;
                if (person.DateOfDeath > DateTime.MinValue) DeathDay = person.DateOfDeath;
                if (person.AlsoKnownAs != null && person.AlsoKnownAs.Length > 0) Aliases = string.Join(",", person.AlsoKnownAs);
                if (!string.IsNullOrEmpty(person.Biography) && string.IsNullOrEmpty(Info)) Info = person.Biography;

                if (TMDBID == null && int.TryParse(person.PersonId, out int id))
                {
                    TMDBID = id;
                }



                if (string.IsNullOrEmpty(IMDB) && !string.IsNullOrEmpty(person.IMDBID)) IMDB = person.IMDBID;

                if (!string.IsNullOrEmpty(person.PlaceOfBirth) && string.IsNullOrEmpty(PlaceOfBirth)) PlaceOfBirth = person.PlaceOfBirth;

                // must have a valid id to save image

                if (!string.IsNullOrEmpty(person.ProfilePath) && (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(ImagePath)) && Id > 0)
                {
                    Avalonia.Media.Imaging.Bitmap temp = TmdbSupport.GetImageFromProfile(person.ProfilePath);


                    if (temp != null && Id > 0)
                    {
                        ImagePath = @"k:\TD1\MovieImages\ActorImages\id-" + Id.ToString().Trim() + ".jpg";

                        if (!System.IO.File.Exists(Support.FixImagePath(imagePath)))
                        {
                            temp.Save(Support.FixImagePath(ImagePath));
                        }
                        SetImageBMP();
                    }
                    if (Id > 0)
                        Save();
                    else
                        Insert();
                }
            }
        }

        /// <summary>
        /// The SetGenderDisplay.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string SetGenderDisplay()
        {
            string gender = "";
            if (Gender != null)
            {

                if (Gender == 0) gender = "Unknown";
                if (Gender == 1) gender = "Female";
                if (Gender == 2) gender = "Male";
            }
            this.RaiseAndSetIfChanged(ref genderDisplay, gender, nameof(GenderDisplay));
            return gender;
        }

        /// <summary>
        /// The SetMovies.
        /// </summary>
        public void SetMovies()
        {
            if (Movies == null)
            {
                Casts ??= [.. DataController.SandboxEntities.Casts.AsNoTracking().Where(x => x.ActorId == Id)];

                List<int?> movieIds = [.. Casts.Select(x => x.MovieID)];
                Movies = [.. (from movies in DataController.SandboxEntities.Movies where movieIds.Contains(movies.Id) select movies)];
            }
        }

        /// <summary>
        /// The GetCasts.
        /// </summary>
        internal void GetCasts()
        {
            if (Casts?.Count == 0)
            {
                Casts = [.. DataController.SandboxEntities.Casts.AsNoTracking().Where(a => a.ActorId == Id)];
            }
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        internal void Insert()
        {
            //string temp = SortName;
            DataController.SandboxEntities.Add(this);
            DataController.SandboxEntities.SaveChanges();
        }


        internal void LogMessage(string action)
        {
            Support.GenerateInfoAndLogMessage(action, "Actor", Id, Name);
        }
        /// <summary>
        /// The Save.
        /// </summary>
        internal void Save()
        {
            if (Dirty)
                try
                {
                    var local = DataController.SandboxEntities.Set<Actor>().Local.FirstOrDefault(entry => entry.Id.Equals(Id));

                    // check if local is not null
                    if (local != null)
                    {
                        // detach
                        DataController.SandboxEntities.Entry(local).State = EntityState.Detached;
                    }
                    // set Modified flag in your entry
                    DataController.SandboxEntities.Entry(this).State = EntityState.Modified;

                    // save
                    DataController.SandboxEntities.SaveChanges();

                    LogMessage("Saved " + ChangedFields);

                    Dirty = false;
                    ChangedFields = string.Empty;

                }
                catch (Exception ex)
                {
                    string msg = "error Saving Actor : " + Id.ToString() + " : " + Name;
                    Support.Logger.Error(ex, msg);
                }
        }

        /// <summary>
        /// The NullImageBMP.
        /// </summary>
        private void NullImageBMP()
        {
            imageBMP = null;
            BMPVisible = false;
        }

        /// <summary>
        /// The SetImageBMP.
        /// </summary>
        private void SetImageBMP()
        {
            try
            {
                if (imageBMP == null)
                {
                    if (!string.IsNullOrEmpty(Support.FixImagePath(imagePath)))
                    {
                        string fileName = Support.FixImagePath(imagePath);
                        if (System.IO.File.Exists(fileName) && imageBMP == null)
                        {
                            ImageBMP = Support.GetBMP(fileName);
                        }
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }

            BMPVisible = (imageBMP != null);
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ActorGroup" />.
    /// </summary>
    public class ActorGroup
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Key.
        /// </summary>
        public string? Key { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ActorIndexModel" />.
    /// </summary>
    public class ActorIndexModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ActorGroups.
        /// </summary>
        public List<ActorGroup>? ActorGroups { get; set; }

        /// <summary>
        /// Gets or sets the Alphas.
        /// </summary>
        public List<string>? Alphas { get; set; }

        /// <summary>
        /// Gets or sets the SelectedGroup.
        /// </summary>
        public ActorGroup? SelectedGroup { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="ActorMetadata" />.
    /// </summary>
    public class ActorMetadata
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Adult.
        /// </summary>
        [Display(Name = "Adult Films")]
        public Nullable<bool> Adult { get; set; }

        /// <summary>
        /// Gets or sets the Aliases.
        /// </summary>
        [Display(Name = "Also known as")]
        [MaxLength(150, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? Aliases { get; set; }

        /// <summary>
        /// Gets or sets the Casts.
        /// </summary>
        [Display(Name = "Cast member in")]
        public virtual ICollection<Cast>? Casts { get; set; }

        /// <summary>
        /// Gets or sets the DeathDay.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Died")]
        public Nullable<System.DateTime> DeathDay { get; set; }

        /// <summary>
        /// Gets or sets the dob.
        /// </summary>
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DOB { get; set; }

        /// <summary>
        /// Gets or sets the FilmGroup.
        /// </summary>
        [Display(Name = "Film Type")]
        public string? FilmGroup { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        [Display(Name = "Gender")]
        public Nullable<int> Gender { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Display(Name = "Actor Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ImagePath.
        /// </summary>
        [Display(Name = "Actor Image Path")]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the IMDB.
        /// </summary>
        [Display(Name = "IMDB Id")]
        public string? IMDB { get; set; }

        /// <summary>
        /// Gets or sets the Info.
        /// </summary>
        [Display(Name = "Actor Info")]
        [MaxLength(400, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? Info { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [Display(Name = "Actor Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the PlaceOfBirth.
        /// </summary>
        [Display(Name = "Place of Birth")]
        public string? PlaceOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the SortName.
        /// </summary>
        [Display(Name = "Sort Name")]
        public string? SortName { get; set; }

        /// <summary>
        /// Gets or sets the TMDBID.
        /// </summary>
        [Display(Name = "TMDB Id")]
        public Nullable<int> TMDBID { get; set; }

        /// <summary>
        /// Gets or sets the WIKIPageID.
        /// </summary>
        [Display(Name = "Wikipedia")]
        public string? WIKIPageID { get; set; }

        #endregion
    }
}
