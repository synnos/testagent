using System;
using System.IO;
using NUnit.Framework;

namespace TestAgent.Core.Tests
{
    [TestFixture]
    public class FilePackagerTests
    {
        [Test]
        public void CompressFiles_EmptyOutputFileSpecified_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FilePackager.CompressFiles(string.Empty,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile1.txt"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile2.txt")));
        }

        [Test]
        public void CompressFiles_NullOutputFileSpecified_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FilePackager.CompressFiles(null,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile1.txt"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile2.txt")));
        }

        [Test]
        public void CompressFiles_NoFilesToCompressSpecified_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FilePackager.CompressFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip")));
        }

        [Test]
        public void CompressFiles_FilesToCompressDontExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FilePackager.CompressFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFileNonExistent.txt")));
        }

        [Test]
        public void CompressFiles_SomeFilesToCompressDontExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => FilePackager.CompressFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFileNonExistent.txt"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile2.txt")));
        }

        [Test]
        public void CompressFiles_EverythingFine_CreatesOutputFile()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip")))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip"));
            }

            FilePackager.CompressFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile1.txt"), 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DummyFile2.txt"));

            Assert.True(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zip")));
        }
    }
}
