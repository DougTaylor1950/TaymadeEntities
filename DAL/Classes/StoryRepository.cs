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

        private readonly DBContext.SandboxEntities _context;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public StoryRepository(SandboxEntities context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool Add(Story Story)
        {
            Story.StorySeries = null;
            _context.Story.Add(Story);
            return Save();
        }

        public bool AddAuthor(Author author)
        {
            Author? temp = _context.Author.Find(author.Id);
            if (temp == null)
            {
                _context.Author.Add(author);
                return Save();
            }
            else return false;
        }

        public bool? AddStoryCast(StoryCast storyCast)
        {
            StoryCast? temp =
            _context.CreateStoryCast(storyCast.StoryId,
                storyCast.CastId, storyCast.Codes, storyCast.Character, storyCast.Age);
            int result = _context.SaveChanges();
            if (temp.Id > 0) storyCast.Id = temp.Id;
            return (result > 0);
        }

        public bool AddStoryDictionary(StoryDictionary? dictionary)
        {
            bool success = false;
            if (dictionary != null && dictionary.StoryId > 0)
            {
                // see if the story is present in the repository 
                Story? story = this.GetById(dictionary.StoryId);
                _context.StoryDictionary.Add(dictionary);
                // cannot save changes as the story may not have been added
                if (story != null)
                {
                    int result = _context.SaveChanges();
                    success = result > 0;
                } else 
                success = true;
            }

            return success;
        }

        public bool AddStoryTransInfo(StoryTransInfo storyTransInfo)
        {
            _context.StoryTransInfo.Add(storyTransInfo);
            return Save();
        }
        public bool AddWordHeading(WordHeadings item)
        {
            _context.WordHeadings.Add(item);
            return Save();
        }

        public StoryCast? CreateStoryCast(int StoryId, int CastId, string Codes, string Character, string Age)
        {
            return _context.CreateStoryCast(StoryId, CastId, Codes, Character, Age);
        }

        public void Delete(int id)
        {
            Story StoryToDelete = _context.Story.Find(id);
            if (StoryToDelete != null)
            {
                // stored procedure used to delete items where story is the foriegn key.
                _context.DeleteStory(id);
                Save();
            }
        }

        public bool DeleteStoryCast(int castId)
        {
            StoryCast? temp = _context.StoryCast.Find(castId);
            if (temp != null)
            {
                _context.StoryCast.Remove(temp);
                return Save();
            }
            return false;
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
            return _context.Author.OrderBy(a => a.Name).ToList();
        }
        public Story? GetById(int id)
        {
            return _context.Story.Find(id);
        }

        public Story? GetOrCreate(string StoryName)
        {
            Story Story = _context.Story.FirstOrDefault(a => a.Path == StoryName);
            if (Story == null)
            {
                Story = new Story { Path = StoryName };
                Insert(Story);
            }
            return Story;
        }

        public IEnumerable<Story> GetStories()
        {
            return _context.Story.Include(s => s.WordHeadingList)
            .Include(s => s.StorySeries)
            .OrderByDescending(s => s.Id).ToList();
        }

        public StoryCast? GetStoryCastById(int castId)
        {
            return _context.StoryCast.Where(s => s.Pk == castId).ToList().FirstOrDefault();
        }

        public IEnumerable<StoryCast>? GetStoryCastList(int storyId)
        {
            return _context.StoryCast.Where(s => s.StoryId == storyId).OrderBy(s => s.CastId);
        }

        public StoryDictionary? GetStoryDictionaryByStoryId(int storyId)
        {
            return _context.StoryDictionary.Where(s => s.StoryId == storyId).FirstOrDefault();
        }

        public StoryDictionary? GetStoryDictionary(int id)
        {
            return _context.StoryDictionary.Where(s => s.Id == id).FirstOrDefault();
        }
        

        public StoryProperties? GetStoryProperties()
        {
            return _context.StoryProperties.ToList().FirstOrDefault();
        }

        public StoryTransInfo? GetStoryTransInfo()
        {
            return _context.StoryTransInfo.FirstOrDefault();
        }

        public IEnumerable<WordHeadings> GetWordHeadingsByStoryId(int storyId)
        {
            return _context.WordHeadings.Where(w => w.StoryId == storyId);
        }

        public void Insert(Story story)
        {
            _context.Story.Add(story);
            if (story.SeriesId == 0) story.SeriesId = 1;
            Save();
        }

        public Task<bool> InsertStoryAsync(Story story)
        {
            _context.Story.Add(story);
            if (story.SeriesId == 0) story.SeriesId = 1;
            return SaveAsync();
        }

        public bool Save()
        {
            try
            {
                bool success = _context.SaveChanges() > 0;
                return success;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    Debug.Write(entry.Entity.GetType().Name + " : ");
                    Debug.WriteLine(entry.State);
                }

                //foreach (var e in _context.ChangeTracker.Entries())
                //{
                //    Debug.WriteLine($"{e.Entity.GetType().Name}  {e.State}");

                //    foreach (var p in e.Properties)
                //    {
                //        if (p.IsModified && e.Entity.GetType().Name == "StoryDictionary")
                //            Debug.WriteLine($"    {p.Metadata.Name}");
                //    }
                //}
                return false; 
               // throw;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }

        public bool Save(Story story)
        {
            if (story.SeriesId == 0) story.SeriesId = 1;
            _context.Story.Update(story);
            return Save();
        }

        public bool SaveStoryProperties(StoryProperties item)
        {
            //bool success = false;
            _context.StoryProperties.Update(item);
            return Save();
                       
        }

        public bool SaveStoryTransInfo(StoryTransInfo? item)
        {
            bool success = false;
            if (item != null)
            {
                //item = _context.StoryTransInfo.Find(item.Id);
                _context.StoryTransInfo.Update(item);
                //if (item.CurrentStoryId > 0)
                //{
                //    Story? story = _context.Story.Find(item.CurrentStoryId);
                //    if (story != null)
                //    {
                //        _context.Story.Update(story);
                //        _context.SaveChanges();
                //    }
                //}
                int result = _context.SaveChanges();
                success = (result > 0);
            }
            return success;
        }

        public bool SaveWordHeading(WordHeadings item)
        {
            _context.WordHeadings.Update(item);
            int result = _context.SaveChanges();
            return (result > 0);
        }

        public bool Update(int storyId)
        {
            Story? temp = _context.Story.Find(storyId);
            if (temp != null)
            {
                _context.Story.Update(temp);
            }
            return Save();
        }

        public async Task<bool> UpdateAsync(Story story)
        {
            _context.Story.Update(story);
            return await SaveAsync();
        }

        public bool UpdateStoryDictionary(StoryDictionary? storyDictionary)
        {
            bool success = false;
            if (storyDictionary != null)
            {
                _context.StoryDictionary.Update(storyDictionary);
                int result = _context.SaveChanges();
                success = result > 0;
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

        #endregion Private Methods
    }
}