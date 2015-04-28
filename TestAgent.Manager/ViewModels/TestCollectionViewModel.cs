using System.Collections.Generic;
using System.Linq;
using TestAgent.Core;

namespace TestAgent.Manager
{
    public class TestCollectionViewModel : TestDefinitionViewModel
    {
        private TestCollection _testCollection;

        public TestCollectionViewModel(TestCollection testCollection)
            : base(testCollection, null)
        {
            _testCollection = testCollection;

            var results =
                        new List<TestDefinitionViewModel>(testCollection.Tests.Length +
                                                          testCollection.Collections.Length);
            results.AddRange(testCollection.Tests.Select(t => new TestDefinitionViewModel(t, this)));
            results.AddRange(testCollection.Collections.Select(c => new TestCollectionViewModel(c)));
            Items = results.OrderBy(d => d.Name).ToArray();
        }

        public TestDefinitionViewModel[] Items { get; private set; }

        public override bool IsSelected
        {
            get { return Items.All(t => t.IsSelected); }
            set
            {
                foreach (var testDefinitionViewModel in Items)
                {
                    testDefinitionViewModel.IsSelected = value;
                }
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public string[] AllSelectedTests
        {
            get
            {
                var tests = new List<string>();
                
                foreach (var test in Items)
                {
                    if (test is TestCollectionViewModel)
                    {
                        var collection = (TestCollectionViewModel) test;
                        tests.AddRange(collection.AllSelectedTests);
                    }
                    else
                    {
                        if (test.IsSelected)
                        {
                            tests.Add(test.Fullname);
                        }
                    }
                }

                return tests.ToArray();
            }
        }

        public void UpdateIsSelected()
        {
            NotifyOfPropertyChange(() => IsSelected);
        }
    }
}