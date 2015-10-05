using System.Collections.Generic;

namespace TestAgent.Core
{
    /// <summary>
    /// Class that holds many test definitions as a collection
    /// </summary>
    public class TestCollection : ITestDefinition
    {
        private readonly List<TestCollection> _collections;
        private readonly List<ITestDefinition> _tests;

        public TestCollection()
        {
            _collections = new List<TestCollection>();
            _tests = new List<ITestDefinition>();
        }

        public void Add(ITestDefinition test)
        {
            _tests.Add(test);
        }

        public void Add(TestCollection collection)
        {
            _collections.Add(collection);
        }

        public void Remove(ITestDefinition test)
        {
            if (_tests.Contains(test))
            {
                _tests.Remove(test);
            }
        }

        public void Remove(TestCollection collection)
        {
            if (_collections.Contains(collection))
            {
                _collections.Remove(collection);
            }
        }

        public string Fullname { get { return Name; } set { Name = value; }}

        public string Name { get; set; }

        public ITestDefinition[] Tests { get { return _tests.ToArray(); } }
        public TestCollection[] Collections { get { return _collections.ToArray(); } }

    }
}
