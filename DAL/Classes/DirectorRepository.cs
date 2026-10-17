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

        public bool AddDirector(Director director)
        {
            using var _context = new DBContext.SandboxEntities();
            Director? temp = _context.Directors.Find(director.Id);
            if (temp == null)
            {
                _context.Directors.Add(director);
                return _context.SaveChanges() > 0;
            }
            else return false;  // already inserted!
        }


        public bool UpdateDirector(Director director)
        {
            using var _context = new DBContext.SandboxEntities();
            Director? temp = _context.Directors.Find(director.Id);
            if (temp == null)
                _context.Directors.Add(director);
            return _context.SaveChanges() > 0;
        }

        public Director? GetDirectorById(int id)
        {
            using var _context = new DBContext.SandboxEntities();
            Director? temp = _context.Directors.Find(id);

            return temp;
        }

        public bool DeleteDirector(Director director)
        {
            using var _context = new DBContext.SandboxEntities();
            Director? temp = _context.Directors.Find(director.Id);
            if (temp == null) return false;
            _context.Directors.Remove(director);
            return _context.SaveChanges() > 0;

        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Director? GetById(int id)
        {
            using var _context = new DBContext.SandboxEntities();
            return _context.Directors.Find(id);
        }

        public Director? GetByName(string directorName)
        {
            using var _context = new DBContext.SandboxEntities();
            Director director = _context.Directors.Where(a => a.Name.ToLower().Contains(directorName.ToLower())).FirstOrDefault();
            return director;
        }

        public List<Director>? GetDirectors()
        {
            using var _context = new DBContext.SandboxEntities();
            return _context.Directors.ToList();
        }

        public Director? GetOrCreate(string? directorName)
        {
            using var _context = new DBContext.SandboxEntities();
            Director director = _context.Directors.FirstOrDefault(a => a.Name == directorName);
            if (director == null)
            {
                director = new Director { Name = directorName };
                _context.Directors.Add(director);
                _context.SaveChanges();
            }
            return director;
        }

        public bool Insert(Director director)
        {
            using var _context = new DBContext.SandboxEntities();
            _context.Directors.Add(director);
            return _context.SaveChanges() > 0;
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

        public List<Director>? GetDirectorsByName(string findDirectorText)
        {
            using var _context = new DBContext.SandboxEntities();
            List<Director>? director = _context.Directors.
                Where(a => a.Name.ToLower().Contains(findDirectorText.ToLower())).ToList();
            return director;
        }

        public async Task<bool> UpdateDirectorAsync(Director director)
        {
            using var _context = new DBContext.SandboxEntities();

            Director? tempDirector = await _context.Directors.FindAsync(director.Id);
            if (tempDirector == null)
            {
                await _context.Directors.AddAsync(director);
            }
            else
            {
                tempDirector.Name = director.Name;
                tempDirector.Info = director.Info;
                tempDirector.ImagePath = director.ImagePath;
            }
            int result = await _context.SaveChangesAsync();
            bool success = result > 0;

            return success;
        }

        public bool Save(Director director)
        {
            using var _context = new DBContext.SandboxEntities();
            //_context.Directors.Update(director);
            return _context.SaveChanges() > 0;
        }


        //public async Task<bool> UpdateAsync(Director director)
        //{
        //    _context.Directors.Update(director);
        //    return await SaveAsync();
        //}

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