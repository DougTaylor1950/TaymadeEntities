using System;
using System.Collections.Generic;
using System.Text;
using TaymadeEntities.DAL.Classes;
using TaymadeEntities.DAL.Interfaces;
using TaymadeEntities.Models;
using TaymadeEntities.Support;

namespace TaymadeEntities.Controllers
{
    public class ActorController : IDisposable
    {
        private bool disposedValue;
        private IActorRepository actorRepository;

        public ActorController()
        {
            this.actorRepository = new ActorRepository(new DBContext.SandboxEntities());
        }

        public ActorController(IActorRepository actorRepository)
        {
            this.actorRepository = actorRepository;
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
        // ~ActorController()
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

        internal Actor? GetActorByName(string name)
        {
            return actorRepository.GetActorByName(name);
        }

        internal Actor? GetOrCreateActor(string name)
        {
            var actor = actorRepository.GetActorByName(name);
            if (actor == null)
            {
                actor = new Actor { Name = name };
                actorRepository.AddActor(actor);
            }
            return actor;
        }

        internal Actor? GetActorByTMDBID(int iD)
        {
            return actorRepository.GetActorByTMID(iD);
        }

        internal void Save(Actor actor)
        {
            actorRepository.Save(actor);
        }

        internal void AddActor(Actor actor)
        {
            actorRepository.AddActor(actor);
        }

        internal void SetDetailsFromCastMember(Actor actor, CastMember person)
        {
            actorRepository.SetDetailsFromCastMember(actor, person);
        }
    }
}
