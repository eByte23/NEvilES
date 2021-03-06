using System;
using NEvilES.Testing;
using Xunit;

namespace NEvilES.Tests
{
    using Sample;

    public class BDD_StyleTests : BaseAggregateTest<Customer.Aggregate>
    {
        [Fact]
        public void CanCreateNewSample()
        {
            var streamId = Guid.NewGuid();
            var cmd = new Customer.Create() { StreamId = streamId, Name = "Testing" };
            Test(Given(),
                When(x => x.Handle(cmd, new Customer.Validate())),
                Then(new Customer.Created{StreamId = streamId, Name = cmd.Name}));
        }

        //[Fact]
        //public void CanCorrectName()
        //{
        //    // TODO this should fail, as we are testing on Customer not Person Aggregate!
        //    var streamId = Guid.NewGuid();
        //    Test(Given(
        //            new Person.Created(streamId, new PersonalDetails("John", "Smith")),
        //            new Person.NameCorrected {StreamId = streamId, Name = "CraigGardiner"}),
        //        When(x => x.Handle(new Person.CorrectName {StreamId = streamId, Name = "Craig Gardiner"})),
        //        Then(new Person.NameCorrectedV2 {StreamId = streamId, FirstName = "Craig", LastName = "Gardiner"}));
        //}

        //[Fact]
        //public void NoNameFails_Create()
        //{
        //    var cmd = new Customer.Create {StreamId = Guid.NewGuid(), Person = new PersonalDetails("Test", "God")};
        //    Test(Given(),
        //        When(x => x.Handle(cmd)),
        //        ThenFailWith<DomainAggregateException>());
        //}
    }
}