using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IStoryRepository : IDisposable
    {
        #region Public Methods
        void Add(Story Story);

        bool? AddStoryCast(StoryCast storyCast);

        bool AddWordHeading(WordHeadings item);

        bool AddStoryDictionary(StoryDictionary? dictionary);

        StoryCast? CreateStoryCast(int StoryId, int CastId, string Codes, string Character, string Age);
        void Delete(int id);


        bool DeleteStoryCast(int castId);

        Story? GetById(int id);


        Story? GetOrCreate(string StoryPath);

        IEnumerable<Story> GetStories();

        StoryCast? GetStoryCastById(int castId);

        IEnumerable<StoryCast>? GetStoryCastList(int storyId);

        StoryDictionary? GetStoryDictionaryByStoryId(int storyId);

        StoryProperties? GetStoryProperties();

        StoryTransInfo? GetStoryTransInfo();

        IEnumerable<WordHeadings> GetWordHeadingsByStoryId(int storyId);

        void Insert(Story Story);
        bool Save();
        bool Save(Story Story);

        bool SaveStoryProperties(StoryProperties item);

        bool SaveStoryTransInfo(StoryTransInfo item);

        bool SaveWordHeading(WordHeadings item);
        bool Update(int storyId);

        bool UpdateStoryDictionary(StoryDictionary? storyDictionary);
        #endregion Public Methods
    }
}
