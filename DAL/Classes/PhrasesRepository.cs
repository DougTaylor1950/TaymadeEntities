using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.DAL.Classes
{
    public class PhrasesRepository : IPhrasesRepository, IDisposable
    {
        #region Private Fields

        private readonly DBContext.SandboxEntities _context;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public PhrasesRepository(SandboxEntities context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(PhraseEntry phrase)
        {
            _context.Add(phrase);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            PhraseEntry? phraseEntry = _context.PhraseEntry.Find(id);
            if (phraseEntry != null)
            {
                _context.Remove(phraseEntry);
                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public PhraseEntry? GetById(int id)
        {
            PhraseEntry? phraseEntry = _context.PhraseEntry.Find(id);
            return phraseEntry;
        }

        public List<PhraseHeader>? GetPhraseHeaders()
        {
            return _context.PhraseHeader.ToList();
        }

        IEnumerable<PhraseHeader>? IPhrasesRepository.GetPhraseHeaders()
        {
            return GetPhraseHeaders();
        }

        public List<PhraseEntry>? GetPhrasesByPhraseHeaderId(int Id)
        {
            List<PhraseEntry>? phraseEntries = _context.PhraseEntry.Where(p => p.PhraseID == Id).ToList();
            return phraseEntries;
        }

        IEnumerable<PhraseEntry>? IPhrasesRepository.GetPhrasesByPhraseHeaderId(int Id)
        {
            return GetPhrasesByPhraseHeaderId(Id);
        }
       
        public IEnumerable<PhraseEntry>? GetSubPhrasesByPhraseID(string compkey)
        {
            return GetSubPhrasesByPhraseID(9, compkey);
        }

        public IEnumerable<PhraseEntry>? GetSubPhrasesByPhraseID(int id, string phraseId)
        {
            // check to see if phraseId contains a '.' if so only use part before '.'
            if (phraseId.Contains("."))
            {
                int index = phraseId.IndexOf(".");
                if (index > 0)
                {
                    phraseId = phraseId.Substring(0, index);
                }
            }
            List<PhraseEntry>? phraseEntries = _context.PhraseEntry.Where(p => p.PhraseID == id && p.Id.Contains(phraseId)).ToList();
            return phraseEntries;
        }

        public void Insert(PhraseEntry phrase)
        {
            _context.Add(phrase);
            _context.SaveChanges();
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ActorRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        public void Save(PhraseEntry phrase)
        {
            _context.Update(phrase);
            _context.SaveChanges();
        }

        public void Update(PhraseEntry phrase)
        {
            _context.Update(phrase);
            _context.SaveChanges();
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