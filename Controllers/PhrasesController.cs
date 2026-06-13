using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class PhrasesController : IDisposable
    {

        private bool disposedValue;
        private IPhrasesRepository phrasesRepository;

        public PhrasesController()
        {
            this.phrasesRepository = new PhrasesRepository(new DBContext.SandboxEntities());
        }

        public PhrasesController(IPhrasesRepository repository)
        {
            this.phrasesRepository = repository;
        }

        public bool Save()
        {
            return phrasesRepository.Save();
        }

        public void Save(Models.PhraseEntry phrase)
        {
            phrasesRepository.Save(phrase);
        }

        public void Update(Models.PhraseEntry phrase)
        {
            phrasesRepository.Update(phrase);
        }

        public void Delete(int id)
        {
            phrasesRepository.Delete(id);
        }

        public PhraseEntry? GetById(int id)
        {
            return phrasesRepository.GetById(id);
        }

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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~phraseController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal List<PhraseEntry>? GetPhrasesByPhraseHeaderId(int v)
        {
            return phrasesRepository.GetPhrasesByPhraseHeaderId(v)?.ToList();
        }

        internal List<PhraseEntry>? GetSubPhraseEntries(string phraseId)
        {
            return phrasesRepository.GetSubPhrasesByPhraseID(phraseId)?.ToList();
        }

        internal void Add(PhraseEntry tempPhrase)
        {
            phrasesRepository.Add(tempPhrase);
        }

        //internal void Update(PhraseEntry Phrase)
        //{
        //    phrasesRepository.Update(phrase);
        //}
    }
}
