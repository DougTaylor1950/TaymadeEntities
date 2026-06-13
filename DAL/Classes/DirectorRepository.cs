using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.DBContext;
using TaymadeEntities.Models;

namespace TaymadeEntities.DAL.Classes
{
    public class DirectorRepository : IDirectorRepository, IDisposable
    {
        #region Private Fields

        private readonly DBContext.SandboxEntities _context;
        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        public DirectorRepository(SandboxEntities context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(Director director)
        {
            _context.Directors.Add(director);
            Save();
        }

        public void Delete(int id)
        {
            Director? directorToDelete = _context.Directors.Find(id);
            if (directorToDelete != null)
            {
                _context.Directors.Remove(directorToDelete);
                Save();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Director? GetById(int id)
        {
            return _context.Directors.Find(id);
        }

        public Director? GetByName(string directorName)
        {
            Director director = _context.Directors.Where(a => a.Name.ToLower().Contains(directorName.ToLower())).FirstOrDefault();
            return director;
        }

        public IEnumerable<Director>? GetDirectors()
        {
            return _context.Directors;
        }

        public Director? GetOrCreate(string directorName)
        {
            Director director = _context.Directors.FirstOrDefault(a => a.Name == directorName);
            if (director == null)
            {
                director = new Director { Name = directorName };
                Insert(director);
            }
            return director;
        }

        public bool Insert(Director director)
        {
            _context.Directors.Add(director);
            return Save();
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

        public async Task<bool> SaveAsync()
        {
            int result = await _context.SaveChangesAsync();
            bool success = result > 0;
            return success;
        }

        public bool Save(Director director)
        {
            _context.Directors.Update(director);
            return Save();
        }


        public async Task<bool> UpdateAsync(Director director)
        {
            _context.Directors.Update(director);
            return await SaveAsync();
        }

        public bool Update(Director director)
        {
            _context.Directors.Update(director);
            return Save();
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