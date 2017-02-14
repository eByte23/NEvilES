using GTD.Web.Events;
using NEvilES;
using System;

namespace GTD.Web.Domain
{
    public abstract class Client
    {
        public class NewClient : ICommand
        {
            public Guid StreamId { get; set; }
            public string Name { get; set; }
        }

        public class ClientCreated : IEvent
        {
            public Guid StreamId { get; set; }
            public string Name { get; set; }
        }

        public class Handlers : AggregateBase,
            IHandleAggregateCommand<NewClient>
        {
            public void Handle(NewClient c)
            {
                RaiseEvent(new ClientCreated
                {
                    StreamId = c.StreamId,
                    Name = c.Name
                });
            }
        }

    }
}
