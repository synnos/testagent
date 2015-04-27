using System.Collections.Generic;
using System.Linq;
using NUnit.Core;

namespace TestAgent.Core
{
    public class NUnitTestLoader : ITestLoader
    {
        public TestCollection[] LoadTests(string filename)
        {
            var results = new List<TestCollection>();
            var package = new TestPackage(filename);

            CoreExtensions.Host.InitializeService();

            var builder = new TestSuiteBuilder();

            var suite = builder.Build(package);

            foreach (object test in suite.Tests)
            {
                var testSuite = test as TestSuite;
                if (testSuite == null) { continue; }
                results.AddRange(GetSteps(testSuite));
            }

            CoreExtensions.Host.UnloadService();

            return results.ToArray();
        }
        
        private static TestCollection[] GetSteps(TestSuite testSuite)
        {
            var results = new List<TestCollection>();
            
            foreach (var testFixture in testSuite.Tests)
            {
                var fixture = testFixture as TestFixture;
                if (fixture != null)
                {
                    results.AddRange(GetSteps(fixture));
                }
                else
                {
                    var suite = testFixture as TestSuite;
                    if (suite != null)
                    {
                        results.AddRange(GetSteps(suite));
                    }
                }
            }

            return results.ToArray();
        }

        private static TestCollection[] GetSteps(TestFixture fixture)
        {
            var results = new List<TestCollection>();

            var collection = new TestCollection();
            collection.Name = fixture.TestName.Name;
            results.Add(collection);

            foreach (var test in fixture.Tests)
            {
                var method = test as TestMethod;
                if (method != null)
                {
                    if (collection.Tests.All(t => t.Fullname != method.TestName.FullName))
                    {
                        var testDefinition = new TestDefinition();
                        testDefinition.Fullname = method.TestName.FullName;
                        testDefinition.Name = method.TestName.Name;
                        collection.Add(testDefinition);
                    }
                }
                else
                {
                    var suite = test as ParameterizedMethodSuite;
                    if (suite != null)
                    {
                        var methodSuite = suite;
                        results.Add(GetSteps(methodSuite));
                    }
                }
            }

            return results.ToArray();
        }

        private static TestCollection GetSteps(ParameterizedMethodSuite methodSuite)
        {
            var collection = new TestCollection();
            collection.Name = methodSuite.TestName.Name;

            foreach (var test1 in methodSuite.Tests)
            {
                var method = test1 as TestMethod;
                if (method != null)
                {
                    if (collection.Tests.All(test => test.Fullname != method.TestName.FullName))
                    {
                        var testDefinition = new TestDefinition();
                        testDefinition.Fullname = method.TestName.FullName;
                        testDefinition.Name = method.TestName.Name;
                        collection.Add(testDefinition);
                    }
                }
            }

            return collection;
        }
    }
}