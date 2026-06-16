using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public void Add(Story Story)
        {
            Story.StorySeries = null;
            _context.Story.Add(Story);
            Save();
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
            if (dictionary != null)
            {
                _context.StoryDictionary.Add(dictionary);
                int result = _context.SaveChanges();
                success = result > 0;
            }

            return success;
        }

        public bool AddWordHeading(WordHeadings item)
        {
            _context.WordHeadings.Add(item);
            return SaveWordHeading(item);
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
                int result = _context.SaveChanges();
                return (result > 0);
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
            return _context.StoryCast.Where(s => s.Id == castId).FirstOrDefault();
        }

        public IEnumerable<StoryCast>? GetStoryCastList(int storyId)
        {
            return _context.StoryCast.Where(s => s.StoryId == storyId).OrderBy(s => s.CastId);
        }

        public StoryDictionary? GetStoryDictionaryByStoryId(int storyId)
        {
            return _context.StoryDictionary.Where(s => s.StoryId == storyId).FirstOrDefault();
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

        public bool Save()
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

        public bool Save(Story story)
        {
            if (story.SeriesId == 0) story.SeriesId = 1;
            _context.Story.Update(story);
            return Save();
        }

        public bool SaveStoryProperties(StoryProperties item)
        {
            bool success = false;
            _context.StoryProperties.Update(item);
            int result = _context.SaveChanges();
            success = (result > 0);
            return success;
        }

        public bool SaveStoryTransInfo(StoryTransInfo item)
        {
            bool success = false;
            _context.StoryTransInfo.Update(item);
            int result = _context.SaveChanges(true);
            success = (result > 0);
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

    }
}
