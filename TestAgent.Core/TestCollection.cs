using System.Collections.Generic;
using System.Linq;

namespace TestAgent.Core
{
    public class TestCollection
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

        public string Name { get; set; }

        public bool IsSelected
        {
            get { return _tests.All(t => t.IsSelected) && _collections.All(c => c.IsSelected); }
            set
            {
                foreach (var testCollection in _collections)
                {
                    testCollection.IsSelected = value;
                }

                foreach (var testDefinition in _tests)
                {
                    testDefinition.IsSelected = value;
                }
            }
        }

        public ITestDefinition[] Tests { get { return _tests.ToArray(); } }
        public TestCollection[] Collections { get { return _collections.ToArray(); } }
    }
}
