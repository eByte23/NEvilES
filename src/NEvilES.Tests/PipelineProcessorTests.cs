using System;
using System.Linq;
using NEvilES.Pipeline;
using NEvilES.Tests.Sample;
using StructureMap;
using Xunit;

namespace NEvilES.Tests
{
    public class PipelineProcessorTests : IClassFixture<SharedFixtureContext>
    {
        private ICommandProcessor commandProcessor;
        private IRepository repository;
        private IContainer container;

        public PipelineProcessorTests(SharedFixtureContext context)
        {
            container = context.Container.GetNestedContainer();
            commandProcessor = container.GetInstance<ICommandProcessor>();
            repository = container.GetInstance<IRepository>();
        }


        [Fact]
        public void CommandWithDifferentEventHandlerOnAggregate()
        {
            var streamId = Guid.NewGuid();

            var expected = commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") });
            Assert.Equal(streamId, expected.FilterEvents<Person.Created>().First().StreamId);
        }

        [Fact]
        public void CommandWithDifferentEventHandlerOnAggregateWithException()
        {
            var streamId = Guid.NewGuid();

            Assert.Throws<DomainAggregateException>(() =>
                commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "God") }));
        }

        [Fact]
        public void CommandWithHandlerDependencies()
        {
            var streamId = Guid.NewGuid();

            var netAmount = 60000M;
            commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") });
            var expected = commandProcessor.Process(new Employee.PayPerson { StreamId = streamId, NetAmount = netAmount });
            var payPerson = expected.FilterEvents<Employee.PaidPerson>().First();
            Assert.Equal(streamId, payPerson.StreamId);
            Assert.True(payPerson.Tax < netAmount);
        }

        [Fact]
        public void CommandWithHandlerDependenciesResultingInAggregateStateChange()
        {
            var streamId = Guid.NewGuid();

            var bonus = 6000M;
            commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") });
            var expected = commandProcessor.Process(new Employee.PayBonus { StreamId = streamId, Amount = bonus });
            var payPerson = expected.FilterEvents<Employee.PaidBonus>().First();
            Assert.Equal(streamId, payPerson.StreamId);
            Assert.Equal(bonus, payPerson.Amount);

            var agg = repository.Get<Employee.Aggregate>(streamId);
            Assert.Equal(bonus, agg.Bonus);
        }

        [Fact]
        public void ProcessStatelessEvent()
        {
            var streamId = Guid.NewGuid();

            commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") });

            var expected = commandProcessor.Process(new Person.StatelessBirthdateChanged { StreamId = streamId, Birthdate = DateTime.Now });
            Assert.Equal(streamId, expected.FilterEvents<Person.StatelessBirthdateChanged>().First().StreamId);
        }

        [Fact]
        public void BadProcessStatelessEvent_Throws()
        {
            var streamId = Guid.NewGuid();

            Assert.Throws<Exception>(() =>
                commandProcessor.Process(new Customer.BadStatelessEvent { StreamId = streamId }));
        }

        [Fact]
        public void WithProjector()
        {
            var streamId = Guid.NewGuid();

            var results = commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") });
            var projectedItem = results.FindProjectedItem<PersonalDetails>();
            Assert.True(projectedItem.FirstName == "John");

            results = commandProcessor.Process(new Person.CorrectName { StreamId = streamId, Name = "New Name" });
            projectedItem = results.FindProjectedItem<PersonalDetails>();
            Assert.True(projectedItem.FirstName == "New");
        }

        [Fact]
        public void WithExternalValidator_Failure()
        {
            var readModel = container.GetInstance<IReadModel>();
            readModel.People.Add(Guid.NewGuid(), new PersonalDetails("John", "Smith"));

            var streamId = Guid.NewGuid();

            Assert.Throws<CommandValidationException>(() =>
                commandProcessor.Process(new Employee.Create { StreamId = streamId, Person = new PersonalDetails("John", "Smith") }));
        }

        [Fact]
        public void OneCommandToManyAggregates()
        {
            var streamId = Guid.NewGuid();

            var command = new Person.SendInvite(streamId, new PersonalDetails("John", "Smith"), "john@gmail.com");
            var expected = commandProcessor.Process(command);

            Assert.True(expected.UpdatedAggregates.Count == 2);

            var projectedItem = expected.FindProjectedItem<PersonalDetails>();
            Assert.True(projectedItem.FirstName == command.Person.FirstName);

            var person = expected.FilterEvents<Person.Created>().First();
            Assert.True(person.Person.LastName == command.Person.LastName);
            var email = expected.FilterEvents<Email.PersonInvited>().First();
            Assert.True(email.StreamId != streamId);
            Assert.True(email.EmailAddress == command.Email);
        }


        [Fact]
        public void Projector_RaiseCommandCastAsEvent_GivenCommandInherentsFromEvent()
        {
            var streamId = Guid.NewGuid();

            var bonus = new Employee.PayBonus { StreamId = streamId, Amount = 10000M };
            var results = commandProcessor.Process(bonus);
            var projectedItem = (decimal)results.ReadModelItems[0];
            Assert.True(projectedItem == bonus.Amount);
        }

        [Fact]
        public void Projector_RaiseStatelessEvent()
        {
            var streamId = Guid.NewGuid();

            var email = new Customer.SendEmail { StreamId = streamId, Text = "Testing" };
            var results = commandProcessor.Process(email);
            var projectedItem = results.ReadModelItems[0];
            Assert.True((string)projectedItem == email.Text);
        }
    }
}