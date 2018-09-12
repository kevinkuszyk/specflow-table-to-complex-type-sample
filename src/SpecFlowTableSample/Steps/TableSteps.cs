using System.Collections.Generic;
using FluentAssertions;
using SpecFlowTableSample.Extensions;
using SpecFlowTableSample.Models;
using TechTalk.SpecFlow;

namespace SpecFlowTableSample.Steps
{
    [Binding]
    public class TableSteps
    {
        [Given(@"I have the following customers")]
        public void GivenIHaveTheFollowingCustomers(Table table)
        {
            var customers = table.VerifyPropertiesAndCreateSet<Customer>();

            ScenarioContext.Current.SaveCustomers(customers);
        }

        [Then(@"I should be able to retrieve the saved customers")]
        public void ThenIShouldBeAbleToRetrieveTheSavedCustomers()
        {
            var savedCustomers = ScenarioContext.Current.GetCustomers();

            var expectedCustomers = new List<Customer>
            {
                new Customer
                {
                    FirstName = "Kevin",
                    LastName = "Kuszyk",
                    Address = new Address
                    {
                        Address1 = "Address 1",
                        Address2 = "Address 2",
                        Town = "Town",
                        PostCode = "LS1 1AB"
                    }
                },
                new Customer
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Address = new Address
                    {
                        Address1 = "A house",
                        Address2 = "Somewhere",
                        Town = "Else",
                        PostCode = "EL1 1SE"
                    }
                }
            };

            savedCustomers.Should().BeEquivalentTo(expectedCustomers);
        }
    }
}
