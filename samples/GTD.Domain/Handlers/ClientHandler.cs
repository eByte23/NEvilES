using System;
using NEvilES;
using GTD.Web.Commands;
using GTD.Web.Domain;

namespace GTD.Web.Commands.Handlers
{
    public class ClientHandler : IHandleAggregateCommand<NewClient>
    {
        public void Handle(NewClient c)
        {
            var client = new Client(c.StreamId, c.By, c.On, c.Name);
        }
    }
}