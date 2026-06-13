using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;

namespace TaymadeEntities.Controllers
{
    public class DirectorController : IDisposable
    {

        private bool disposedValue;
        private IDirectorRepository directorRepository;

        public DirectorController()
        {
            this.directorRepository = new DirectorRepository(new DBContext.SandboxEntities());
        }

        public DirectorController(IDirectorRepository repository)
        {
            this.directorRepository = repository;
        }

        public bool Save()
        {
            return directorRepository.Save();
        }

        public async Task<bool> SaveAsync()
        {
            return await directorRepository.SaveAsync();
        }

        public bool Save(Models.Director movie)
        {
            return directorRepository.Save(movie);
        }

        public bool Update(Models.Director movie)
        {
            return directorRepository.Update(movie);
        }

        public async Task<bool> UpdateAsync(Models.Director movie)
        {
            return await directorRepository.UpdateAsync(movie);
        }

        public void Delete(int id)
        {
            directorRepository.Delete(id);
        }

        public Director? GetById(int id)
        {
            return directorRepository.GetById(id);
        }

        public List<Director>? GetDirectorList()
        {
            return directorRepository.GetDirectors()?.ToList();
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
        // ~MovieController()
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

        internal void Insert(Director director)
        {
            try
            {
                directorRepository.Insert(director);
            }
            catch (Exception ex)
            {

                
            }
            
        }

        public Director? GetByName(string findDirectorText)
        {
            return directorRepository.GetByName(findDirectorText);
        }
    }
}
