using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaymadeEntities.Models
{
    public class ActorList : List<Actor>
    {
        public ActorList()
        {
        }

        public ActorList(List<XElement> actors)
        {
            foreach (XElement item in actors)
            {
                Actor newActor = new Actor(item);
                this.Add(newActor);
            }
        }

        public XElement ToXMLElement()
        {
            XElement element = new XElement("Actors");
            foreach (Actor item in this)
            {
                element.Add(item.ToXMLElement());
            }

            return element;
        }
    }
}
