using DocumentFormat.OpenXml.EMMA;
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
    public class ActorRepository : IActorRepository, IDisposable
    {
        private bool disposedValue;

        private readonly DBContext.SandboxEntities _context;

        public ActorRepository(SandboxEntities context)
        {
            _context = context;
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
        // ~ActorRepository()
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

        public void DeleteActor(int id)
        {
            Actor actorToDelete = _context.Actors.Find(id);
            if (actorToDelete != null)
            {
                _context.Actors.Remove(actorToDelete);
                Save();
            }
        }

        public IEnumerable<Movies>? GetActorMovies(int actorId)
        {
            List<Movies> tempList = new List<Movies>();

            IEnumerable<Cast> actorCasts = _context.Casts.Where(c => c.ActorId == actorId).ToList();

            List<int?> actorMovieIds = actorCasts.Select(c => c.MovieID).ToList();


            var result = from x in DataController.SandboxEntities.Movies
                         where actorMovieIds.Contains(x.Id)
                         select x;
            // should be a list of movies
            tempList = result.ToList();
            result = null;

            return tempList;
        }

        public IEnumerable<Actor> GetActors()
        {
            return _context.Actors.ToList().OrderBy(a=> a.Name) ?? Enumerable.Empty<Actor>();
        }

        public IEnumerable<Actor> GetActorsByName(string findText)
        {
            return _context.Actors.Where(a => a.Name.ToLower().Contains(findText.ToLower()))
                .OrderBy(a => a.Name).ToList() ?? Enumerable.Empty<Actor>();
        }
        public Actor? GetActorById(int id)
        {
            return _context.Actors.Find(id);
        }

        public void InsertActor(Actor actor)
        {
            _context.Actors.Add(actor);
            Save();
        }

        public Actor? GetOrCreateActor(string actorName)
        {
            Actor actor = _context.Actors.FirstOrDefault(a => a.Name == actorName);
            if (actor == null)
            {
                actor = new Actor { Name = actorName };
                InsertActor(actor);
            }
            return actor;
        }

        public void UpdateActor(Actor actor)
        {
            _context.Actors.Update(actor);
            Save();
        }

        public bool Save()
        {
            bool success = _context.SaveChanges() > 0;
            return success;
        }

        public Actor? GetActorByName(string actorName)
        {
            // only return the first actor with the given name, if there are multiple actors with the same name, this will need to be updated to return null or throw an exception

            Actor returnedActor = null;
            List<Actor> temp = _context.Actors.Where(a => a.Name.ToLower() == actorName.ToLower()).ToList();

            if (temp.Count == 1)
            {
                returnedActor = temp[0];
            }
            else if (temp.Count > 1)
            {
                throw new Exception($"Multiple actors found with the name {actorName}. Please provide a unique name or use the actor's ID to retrieve the correct actor.");
            }

            return returnedActor;
        }

        public void AddActor(Actor actor)
        {
            _context.Actors.Add(actor);
            Save();
        }

        public Actor? GetActorByTMID(int tmdbId)
        {
            return _context.Actors.FirstOrDefault(a => a.TMDBID == tmdbId);
        }

        public void Save(Actor actor)
        {
            _context.Actors.Update(actor);
            Save();
        }

        public void SetDetailsFromCastMember(Actor actor, CastMember person)
        {
            if (person != null)
            {
                if (!string.IsNullOrEmpty(person.Name)) actor.Name = person.Name;
                if (person.Gender > 0) actor.Gender = person.Gender;
                if (person.BirthDate > DateTime.MinValue) actor.DOB = person.BirthDate;
                if (person.Adult) actor.Adult = person.Adult;
                if (person.DeathDate > DateTime.MinValue) actor.DeathDay = person.DeathDate;
                if (person.KnownAs != null && person.KnownAs.Length > 0) actor.Aliases = string.Join(",", person.KnownAs);
                //if (!string.IsNullOrEmpty(person.) && string.IsNullOrEmpty(Info)) Info = person.Biography;

                //if (TMDBID == null && int.TryParse(person.ID, out int id))
                //{
                actor.TMDBID = person.ID;
                //}

                const string defaultimage = @"\id-0.jpg";


                if (string.IsNullOrEmpty(actor.IMDB) && !string.IsNullOrEmpty(person.IMDB)) actor.IMDB = person.IMDB;

                if (!string.IsNullOrEmpty(person.PlaceOfBirth) && string.IsNullOrEmpty(actor.PlaceOfBirth)) actor.PlaceOfBirth = person.PlaceOfBirth;

            }
            }
        }
}
