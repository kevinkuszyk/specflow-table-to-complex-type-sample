using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SpecFlowTableSample.Extensions
{
    public static class TableExtensions
    {
        public static T CreateComplexInstance<T>(this Table table)
        {
            T result = table.CreateInstance<T>();

            // find sub-properties by looking for "."
            var propNames = table.Rows.OfType<TableRow>()
                .Where(x => x[0].Contains("."))
                .Select(x => Regex.Replace(x[0], @"^(.+?)\..+$", "$1"));

            foreach (var propName in propNames)
            {
                // look for matching property in result object
                var prop = typeof(T).GetProperty(
                    propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (prop != null)
                {
                    // create sub-table with relevant rows (field == propName.something)
                    var subTable = new Table("field", "value");
                    var re = new Regex(string.Format(@"^{0}\.([^\.]*).*$", propName), RegexOptions.IgnoreCase);
                    var subheadings = table.Rows.OfType<TableRow>().Where(x => re.IsMatch(x[0])).ToList();

                    var dsfdsf = subheadings
                        .Select(x => new[] { string.Join(".", x[0].Split('.').Skip(1)), x[1] })
                        .ToList();

                    dsfdsf.ForEach(x => subTable.AddRow(x));

                    // make recursive call to create child object
                    var createInstance = typeof(TableExtensions)
                        .GetMethod(
                            nameof(CreateComplexInstance),
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            CallingConventions.Any,
                            new Type[] { typeof(Table) },
                            null);
                    createInstance = createInstance.MakeGenericMethod(prop.PropertyType);
                    object propValue = createInstance.Invoke(null, new object[] { subTable });

                    // assign child object to result
                    prop.SetValue(result, propValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a specflow table to an instance. But includes complex objects as well (ParentProperty.ChildProperty is the convention)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> CreateComplexSet<T>(this Table table)
        {
            var items = table.CreateSet<T>();

            for (var i = 0; i < table.RowCount; i++)
            {
                var tableRow = table.Rows[i];

                T result = items.ElementAt(i);

                // find sub-properties by looking for "."
                var propNames = tableRow
                    .Where(x => x.Key.Contains("."))
                    .Select(x => Regex.Replace(x.Key, @"^(.+?)\..+$", "$1"));

                foreach (var propName in propNames)
                {
                    // look for matching property in result object
                    var prop = typeof(T).GetProperty(
                        propName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (prop != null)
                    {
                        // create sub-table with relevant rows (field == propName.something)
                        var subTable = new Table("field", "value");
                        var re = new Regex(string.Format(@"^{0}\.([^\.]*)$", propName), RegexOptions.IgnoreCase);

                        tableRow.Where(x => re.IsMatch(x.Key))
                              .Select(x => new[] { re.Replace(x.Key, "$1"), x.Value })
                              .ToList()
                              .ForEach(x => subTable.AddRow(x));

                        // make recursive call to create child object
                        var createInstance = typeof(TableHelperExtensionMethods)
                            .GetMethod(
                                "CreateInstance",
                                BindingFlags.Public | BindingFlags.Static,
                                null,
                                CallingConventions.Any,
                                new Type[] { typeof(Table) },
                                null);
                        createInstance = createInstance.MakeGenericMethod(prop.PropertyType);
                        object propValue = createInstance.Invoke(null, new object[] { subTable });

                        prop.SetValue(result, propValue);
                    }
                }
            }

            return items;
        }


        /// <summary>
        /// Verifies that all properties specified in the table exist on the target type and returns a complex set if they do.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> VerifyPropertiesAndCreateSet<T>(this Table table)
        {
            table.VerifyAllPropertiesExistOnTargetType<T>();
            return table.CreateComplexSet<T>();
        }

        /// <summary>
        /// Verifies that all headings specified in the table exist as properties in the target type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        public static void VerifyAllPropertiesExistOnTargetType<T>(this Table table)
        {
            var headers = table.Header.Select(e => e.Split('.').First());
            var propertiesOnTargetType = typeof(T).GetProperties().Select(e => e.Name).ToList();

            var missingProperties = headers.Where(e => !propertiesOnTargetType.Contains(e)).ToList();

            if (missingProperties.Any())
            {
                var message = $"Type {typeof(T).Name} is missing the following properties specifield in the table: {string.Join(", ", missingProperties)}.";
                throw new ArgumentException(message);
            }
        }
    }
}
