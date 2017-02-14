using System;
using NEvilES;
using NEvilES.SampleMvc.Commands;

namespace NEvilES.SampleMvc.Commands.Handlers
{
    public class NotesHandler : IHandleAggregateCommand<NewNote>
    {
        public void Handle(NewNote command)
        {

        }
    }
}