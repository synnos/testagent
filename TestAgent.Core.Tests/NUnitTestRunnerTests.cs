using System;
using System.IO;
using NUnit.Framework;

namespace TestAgent.Core.Tests
{
    [TestFixture]
    public class NUnitTestRunnerTests
    {
        readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Temp");
        
        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void CopyDependencies_CopiesAllNecessaryFiles()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }

            Directory.CreateDirectory(_directory);

            var testRunner = new NUnitTestRunner();
            testRunner.CopyDependencies(_directory);

            Assert.True(File.Exists(Path.Combine(_directory, "nunit.framework.dll")));
            Assert.True(File.Exists(Path.Combine(_directory, "nunit.core.dll")));
            Assert.True(File.Exists(Path.Combine(_directory, "nunit.core.interfaces.dll")));
        }

        [Test]
        public void CopyDependencies_EmptyOutputDirectory_ThrowsException()
        {
            var testRunner = new NUnitTestRunner();

            Assert.Throws<ArgumentException>(() => testRunner.CopyDependencies(string.Empty));
        }

        [Test]
        public void CopyDependencies_NullOutputDirectory_ThrowsException()
        {
            var testRunner = new NUnitTestRunner();

            Assert.Throws<ArgumentException>(() => testRunner.CopyDependencies(null));
        }

        [Test]
        public void State_DefaultValueIsIdle()
        {
            var testRunner = new NUnitTestRunner();
            Assert.AreEqual(TestRunnerState.Idle, testRunner.State);
        }
    }
}
