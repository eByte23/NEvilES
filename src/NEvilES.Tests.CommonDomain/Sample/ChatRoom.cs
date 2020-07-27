using System;
using System.Collections.Generic;
using NEvilES.Abstractions;

namespace NEvilES.Tests.CommonDomain.Sample
{
    public class ChatRoom
    {
        public class Create : ICommand
        {
            public Create()
            {
                InitialUsers = new HashSet<Guid>();
            }

            public Guid StreamId { get; set; }
            public string Name { get; set; }
            public HashSet<Guid> InitialUsers { get; set; }
        }

        public class Created : Create, IEvent
        {

        }

        public class IncludeUserInRoom : ICommand
        {
            public Guid StreamId { get; set; }
            public Guid UserId { get; set; }
        }

        public class UserIncludedInRoom : IncludeUserInRoom, IEvent
        {

        }

        public class RemoveUserFromRoom : ICommand
        {
            public Guid StreamId { get; set; }
            public Guid UserId { get; set; }
        }

        public class UserRemovedFromRoom : RemoveUserFromRoom, IEvent
        {

        }


        public class Aggregate : AggregateBase,
            IHandleAggregateCommand<Create>,
            IHandleAggregateCommand<IncludeUserInRoom>,
            IHandleAggregateCommand<RemoveUserFromRoom>

        {
            public ICommandResponse Handle(Create command)
            {
                Raise<Created>(command);
                return new CommandCompleted(command.StreamId,nameof(Create));
            }

            public ICommandResponse Handle(IncludeUserInRoom command)
            {
                Raise<UserIncludedInRoom>(command);
                return new CommandCompleted(command.StreamId,nameof(IncludeUserInRoom));
            }

            public ICommandResponse Handle(RemoveUserFromRoom command)
            {
                Raise<UserRemovedFromRoom>(command);
                return new CommandCompleted(command.StreamId,nameof(RemoveUserFromRoom));
            }



            //------------------------------------------------

            private string Name;
            private HashSet<Guid> UsersInRoom;


            private void Apply(Created e)
            {
                Id = e.StreamId;
                Name = e.Name;
                UsersInRoom = e.InitialUsers;
            }
            private void Apply(UserIncludedInRoom e)
            {
                UsersInRoom.Add(e.UserId);
            }

            private void Apply(UserRemovedFromRoom e)
            {
                UsersInRoom.Remove(e.UserId);
            }
        }

    }
}