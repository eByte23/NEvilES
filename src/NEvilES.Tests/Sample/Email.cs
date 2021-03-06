using System;

namespace NEvilES.Tests.Sample
{
    public class Email
    {
        public class PersonInvited : IEvent
        {
            public Guid StreamId { get; set; }
            public string EmailAddress { get; set; }
        }

        public class Aggregate : StatelessAggregate,
            IHandleStatelessEvent<PersonInvited>
        {
        }
    }
}