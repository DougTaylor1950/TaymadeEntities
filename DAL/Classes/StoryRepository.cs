using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Classes
{
    public class StoryRepository : IStoryRepository, IDisposable
    {
        #region Private Fields

        // private DBContext.SandboxEntities _context;
        private bool disposedValue;
        private SandboxEntities _context;

        #endregion Private Fields

        #region Public Constructors

        public StoryRepository(SandboxEntities context)
        {
            // _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool Add(Story Story)
        {
            Story.StorySeries = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.Story.Add(Story);

                return _context.SaveChanges() > 0;
            }
        }

        public bool AddAuthor(Author author)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                Author? temp = _context.Author.Find(author.Id);
                if (temp == null)
                {
                    _context.Author.Add(author);

                    return _context.SaveChanges() > 0;
                }
                else return false;
            }
        }

        public bool? AddStoryCast(StoryCast storyCast)
        {
            StoryCast? temp = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                temp = _context.CreateStoryCast(storyCast.StoryId,
                    storyCast.CastId, storyCast.Codes, storyCast.Character, storyCast.Age);
            }
            int result = _context.SaveChanges();
            if (temp != null && temp.Id > 0) storyCast.Id = temp.Id;
            return (result > 0);
        }

        public bool AddStoryDictionary(StoryDictionary? dictionary)
        {
            bool success = false;
            if (dictionary != null && dictionary.StoryId > 0)
            {
                // see if the story is present in the repository 
                Story? story = this.GetById(dictionary.StoryId);
                using (var _context = new DBContext.SandboxEntities())
                {
                    _context.StoryDictionary.Add(dictionary);

                    // cannot save changes as the story may not have been added
                    if (story != null)
                    {
                        int result = _context.SaveChanges();
                        success = result > 0;
                    }
                    else
                        success = true;
                }
            }

            return success;
        }

        public bool UpdateStorySeries(StorySeries storySeries)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.StorySeries.Update(storySeries);
            }
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<StorySeries> GetStorySeriesList()
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                return _context.StorySeries.ToList().OrderBy(s => s.Name);
            }
        }

        public StoryAuthor? AddStoryAuthor(int id1, int id2)
        {
            StoryAuthor? storyAuthor = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                storyAuthor = _context.InsertStoryAuthor(id1, id2);
            }
            return storyAuthor;
        }

        public List<StoryAuthor>? GetStoryAuthorsByStoryId(int id)
        {
            List<StoryAuthor>? storyAuthors = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                storyAuthors = _context.StoryAuthor.Where(sa => sa.StoryId == id).ToList();
            }
            return storyAuthors;
        }

        public StoryAuthor? GetAuthorById(int id)
        {
            StoryAuthor? storyAuthor = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                storyAuthor = _context.StoryAuthor.Find(id);
            }
            return storyAuthor;
        }

        public IEnumerable<Story>? GetStoriesByAuthorId(int id)
        {
            List<Story>? tempList = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                tempList = _context.Story.Where(s => s.IDAuthor == id).OrderBy(s => s.Title).ToList();
            }
            return tempList;
        }
        public bool AddStoryTransInfo(StoryTransInfo storyTransInfo)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.StoryTransInfo.Add(storyTransInfo);
            }
            return _context.SaveChanges() > 0;
        }
        public bool AddWordHeading(WordHeadings item)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.WordHeadings.Add(item);

                return _context.SaveChanges() > 0;
            }
        }

        public StoryCast? CreateStoryCast(int StoryId, int CastId, string Codes, string Character, string Age)
        {
            StoryCast? storyCast = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                storyCast = _context.CreateStoryCast(StoryId, CastId, Codes, Character, Age);
            }
            return storyCast;
        }

        public void Delete(int id)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                using Story? StoryToDelete = _context.Story.Find(id);
                if (StoryToDelete != null)
                {
                    // stored procedure used to delete items where story is the foriegn key.
                    _context.DeleteStory(id);
                    _context.SaveChanges();
                }
            }
        }

        public bool DeleteStoryCast(int castId)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                StoryCast? temp = _context.StoryCast.Find(castId);
                if (temp != null)
                {
                    _context.StoryCast.Remove(temp);
                    return _context.SaveChanges() > 0;
                }

                return false;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StoryRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public IEnumerable<Author> GetAuthors()
        {
            List<Author> list = new List<Author>();
            using (var _context = new DBContext.SandboxEntities())
                list = _context.Author.OrderBy(a => a.Name).ToList();
            return list;
        }
        public Story? GetById(int id)
        {
            Story? story = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                story = _context.Story.Find(id);
            }
            return story;
        }

        public Story? GetOrCreate(string StoryName)
        {
            Story? story = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                story = _context.Story.FirstOrDefault(a => a.Path == StoryName);
                if (story == null)
                {
                    story = new Story { Path = StoryName };
                    Insert(story);
                }
                return story;
            }
        }

        public IEnumerable<Story> GetStories()
        {
            List<Story> list = null;
            using (var _context = new DBContext.SandboxEntities())
            {
                list = _context.Story.Include(s => s.WordHeadingList)
                .Include(s => s.StorySeries)
                .OrderByDescending(s => s.Id).ToList();
            }
            return list;
        }

        public StoryCast? GetStoryCastById(int castId)
        {
            StoryCast? storyCast = null;
            using (var _context = new DBContext.SandboxEntities())
                storyCast = _context.StoryCast.Where(s => s.Pk == castId).ToList().FirstOrDefault();
            return storyCast;
        }

        public IEnumerable<StoryCast>? GetStoryCastList(int storyId)
        {
            List<StoryCast> list = new List<StoryCast>();
            using (var _context = new DBContext.SandboxEntities())
                list = _context.StoryCast.Where(s => s.StoryId == storyId).OrderBy(s => s.CastId).ToList();
            return list;
        }

        public StoryDictionary? GetStoryDictionaryByStoryId(int storyId)
        {
            StoryDictionary? storyDictionary = null;
            using (var _context = new DBContext.SandboxEntities())
                storyDictionary = _context.StoryDictionary.Where(s => s.StoryId == storyId).FirstOrDefault();
            return storyDictionary;
        }

        public StoryDictionary? GetStoryDictionary(int id)
        {
            StoryDictionary? storyDictionary = null;
            using (var _context = new DBContext.SandboxEntities())
                storyDictionary = _context.StoryDictionary.Where(s => s.Id == id).FirstOrDefault();
            return storyDictionary;
        }


        public StoryProperties? GetStoryProperties()
        {
            StoryProperties? storyProperties = null;
            using (var _context = new DBContext.SandboxEntities())
                storyProperties = _context.StoryProperties.ToList().FirstOrDefault();
            return storyProperties;
        }

        public StoryTransInfo? GetStoryTransInfo()
        {
            StoryTransInfo? storyTransInfo = null;
            using (var _context = new DBContext.SandboxEntities())
                storyTransInfo = _context.StoryTransInfo.FirstOrDefault();
            return storyTransInfo;
        }

        public IEnumerable<WordHeadings> GetWordHeadingsByStoryId(int storyId)
        {
            List<WordHeadings>? list = null;
            using (var _context = new DBContext.SandboxEntities())
                list = _context.WordHeadings.Where(w => w.StoryId == storyId).ToList();
            return list;
        }

        public void Insert(Story story)
        {
            using (var _context = new DBContext.SandboxEntities())
                _context.Story.Add(story);
            if (story.SeriesId == 0) story.SeriesId = 1;
            _context.SaveChanges();
        }

        public Task<bool> InsertStoryAsync(Story story)
        {
            using (var _context = new DBContext.SandboxEntities())
                _context.Story.Add(story);
            if (story.SeriesId == 0) story.SeriesId = 1;
            return SaveAsync();
        }

        //public bool Save()
        //{
        //    try
        //    {
        //        using bool success = _context.SaveChanges() > 0;
        //        return success;
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        foreach (var entry in ex.Entries)
        //        {
        //            Debug.Write(entry.Entity.GetType().Name + " : ");
        //            Debug.WriteLine(entry.State);
        //        }

        //        //foreach (var e in _context.ChangeTracker.Entries())
        //        //{
        //        //    Debug.WriteLine($"{e.Entity.GetType().Name}  {e.State}");

        //        //    foreach (var p in e.Properties)
        //        //    {
        //        //        if (p.IsModified && e.Entity.GetType().Name == "StoryDictionary")
        //        //            Debug.WriteLine($"    {p.Metadata.Name}");
        //        //    }
        //        //}
        //        return false; 
        //       // throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.ToString();
        //        return false;
        //    }
        //}

        public bool Save(Story story)
        {
            if (story.SeriesId == 0) story.SeriesId = 1;
            using (var _context = new DBContext.SandboxEntities())
                _context.Story.Update(story);
            return _context.SaveChanges() > 0;

        }

        public bool SaveStoryProperties(StoryProperties item)
        {
            //bool success = false;
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.StoryProperties.Update(item);
                return _context.SaveChanges() > 0;
            }


        }



        public bool SaveStoryTransInfo(StoryTransInfo? item)
        {
            bool success = false;
            if (item != null)
            {
                //item = _context.StoryTransInfo.Find(item.Id);
                using (var _context = new DBContext.SandboxEntities())
                {
                    _context.StoryTransInfo.Update(item);



                    int result = _context.SaveChanges();

                    success = (result > 0);
                }

            }
            return success;
        }

        public bool SaveWordHeading(WordHeadings item)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                _context.WordHeadings.Update(item);

                int result = _context.SaveChanges();

                return (result > 0);
            }
        }

        public bool Update(Story story)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                Story? temp = _context.Story.Find(story.Id);
                if (temp == null)
                {
                    _context.Story.Add(story);
                }
                else
                {
                    temp.MoveFromStory(story);
                }

                return _context.SaveChanges() > 0;
            }
        }

        public async Task<bool> UpdateAsync(Story story)
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                Story? temp = _context.Story.Find(story.Id);
                if (temp == null)
                {
                    _context.Story.Add(story);
                }
                else
                {
                    temp.MoveFromStory(story);
                }

                return await _context.SaveChangesAsync() > 0;
            }
        }

        

        

        public bool UpdateStoryDictionary(StoryDictionary? storyDictionary)
        {
            bool success = false;
            if (storyDictionary != null)
            {
                using (var _context = new DBContext.SandboxEntities())
                {
                    _context.StoryDictionary.Update(storyDictionary);

                    int result = _context.SaveChanges();

                    success = result > 0;
                }
            }

            return success;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task<bool> SaveAsync()
        {
            try
            {
                bool success = await _context.SaveChangesAsync() > 0;
                return success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        bool IStoryRepository.Save()
        {
            using (var _context = new DBContext.SandboxEntities())
            {
                try
                {
                    bool success = _context.SaveChanges() > 0;
                    return success;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        #endregion Private Methods
    }
}