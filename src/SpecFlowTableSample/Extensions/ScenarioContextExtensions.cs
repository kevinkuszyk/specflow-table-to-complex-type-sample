using System.Collections.Generic;
using SpecFlowTableSample.Models;
using TechTalk.SpecFlow;

namespace SpecFlowTableSample.Extensions
{
    public static class ScenarioContextExtensions
    {
        public static void SaveCustomers(this ScenarioContext context, IEnumerable<Customer> customers)
        {
            context["customers"] = customers;
        }

        public static IEnumerable<Customer> GetCustomers(this ScenarioContext context)
        {
            return context["customers"] as IEnumerable<Customer>;
        }
    }
}
