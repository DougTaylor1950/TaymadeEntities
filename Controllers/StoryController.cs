using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class StoryController : IDisposable
    {
        #region Private Fields

        private bool disposedValue;
        private IStoryRepository storyRepository;

        #endregion Private Fields

        #region Public Constructors

        public StoryController()
        {
            this.storyRepository = new StoryRepository(new DBContext.SandboxEntities());
        }

        public StoryController(IStoryRepository repository)
        {
            this.storyRepository = repository;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Delete(int id)
        {
            storyRepository.Delete(id);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Story? GetById(int id)
        {
            return storyRepository.GetById(id);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MovieController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public StoryProperties? GetStoryProperties()
        {
            return storyRepository.GetStoryProperties();
        }
        public IEnumerable<Story> GetStories()
        {
            return storyRepository.GetStories();
        }

        public StoryTransInfo? GetStoryTransInfo()
        {
            return storyRepository.GetStoryTransInfo();
        }


        public bool Save()
        {
            return storyRepository.Save();
        }

        public bool Save(Models.Story story)
        {
            return storyRepository.Save(story);
        }

        public bool SaveStoryProperties(StoryProperties item)
        {
            return storyRepository.SaveStoryProperties(item);
        }

        public bool SaveStoryTransInfo(StoryTransInfo item)
        {
            return storyRepository.SaveStoryTransInfo(item);
        }
        public bool Update(Models.Story story)
        {
            return storyRepository.Update(story.Id);
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

        internal StoryCast? CreateStoryCast(int storyId, int value, string codes, string? character, string? age)
        {
            return storyRepository.CreateStoryCast(storyId, value, codes, character, age);
        }

        internal bool DeleteStoryCast(StoryCast storyCast)
        {
            return storyRepository.DeleteStoryCast(storyCast.Pk);
        }

        internal List<StoryCast>? GetStoryCastByStoryId(int storyId)
        {
            return storyRepository.GetStoryCastList(storyId)?.ToList();
        }

        internal StoryCast? GetStoryCastById(int pkInt)
        {
            return storyRepository.GetStoryCastById(pkInt);
        }

        internal void AddStory(Story story)
        {
            storyRepository.Add(story);
        }

        internal List<WordHeadings>? GetWordHeadingsList(int id)
        {
            return storyRepository.GetWordHeadingsByStoryId(id)?.ToList();
        }

        internal bool AddWordHeading(WordHeadings wordHeadings)
        {
            return storyRepository.AddWordHeading(wordHeadings);
        }

        internal bool UpdateWordHeading(WordHeadings wordHeadings)
        {
            return storyRepository.SaveWordHeading(wordHeadings);
        }

        #endregion Protected Methods
    }
}