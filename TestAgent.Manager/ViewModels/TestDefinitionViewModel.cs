using Caliburn.Micro;
using TestAgent.Core;

namespace TestAgent.Manager
{
    public class TestDefinitionViewModel : PropertyChangedBase
    {
        private ITestDefinition _testDefinition;
        private TestCollectionViewModel _parentCollection;
        private bool _isSelected;

        public TestDefinitionViewModel(ITestDefinition testDefinition, TestCollectionViewModel parentCollection)
        {
            _testDefinition = testDefinition;
            _parentCollection = parentCollection;
        }

        public string Name
        {
            get { return _testDefinition.Name; }
            set
            {
                if (value == _testDefinition.Name) return;
                _testDefinition.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Fullname
        {
            get { return _testDefinition.Fullname; }
            set
            {
                if (value == _testDefinition.Fullname) return;
                _testDefinition.Fullname = value;
                NotifyOfPropertyChange(() => Fullname);
            }
        }

        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
                _parentCollection.UpdateIsSelected();
            }
        }
    }
}