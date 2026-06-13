using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Interfaces
{
    public interface IPhrasesRepository : IDisposable
    {
        #region Public Methods

        void Delete(int id);

        PhraseEntry? GetById(int id);

        void Insert(PhraseEntry phrase);

        bool Save();

        void Update(PhraseEntry phrase);

        void Add(PhraseEntry phrase);

        void Save(PhraseEntry phrase);

        IEnumerable<PhraseEntry>? GetPhrasesByPhraseHeaderId(int Id);

        IEnumerable<PhraseEntry>? GetSubPhrasesByPhraseID(string compkey);

        IEnumerable<PhraseEntry>? GetSubPhrasesByPhraseID(int id, string phraseId);

        IEnumerable<PhraseHeader>? GetPhraseHeaders();

        #endregion Public Methods
    }
}