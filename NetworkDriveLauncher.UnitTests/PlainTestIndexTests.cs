using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core.Index;
using Wororo.Utilities;

namespace NetworkDriveLauncher.UnitTests
{
    public class PlainTestIndexTests
    {
        private PlainTextIndex _index;
        private PlainTextIndexConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            _configuration = new PlainTextIndexConfiguration(config);
            _index = new PlainTextIndex(_configuration);
        }

        [Test]
        public void TestConfigurationFile()
        {
            //Assert
            Assert.NotNull(_configuration);
            Assert.NotNull(_configuration.OutputFilename);
            Assert.NotNull(_configuration.Depth);
            Assert.NotNull(_configuration.OverwriteIndex);
            Assert.NotNull(_configuration.RootDirectories);

            Assert.IsTrue(_configuration.OutputFilename.IsNotEmpty());
            Assert.IsNotEmpty(_configuration.RootDirectories);
            Assert.IsTrue(_configuration.RootDirectories.FirstOrDefault().IsNotEmpty());
        }

        [Test]
        public void TestCreateDirectories()
        {
            //Arrange
            var rootDirectory = "TestCreateDirectory";
            rootDirectory.DeleteIfExists();

            //Act
            UnitTestsHelper.CreateDirectories(rootDirectory, 4, 3);

            //Assert
            Assert.IsTrue(Directory.Exists(rootDirectory));
            Assert.IsTrue(Directory.Exists($"{rootDirectory}\\{0}"));
            Assert.IsTrue(Directory.Exists($"{rootDirectory}\\{0}\\{0}"));
            Assert.IsTrue(Directory.Exists($"{rootDirectory}\\{0}\\{0}\\{0}"));
            Assert.IsTrue(Directory.Exists($"{rootDirectory}\\{3}\\{3}\\{3}"));
            Assert.IsFalse(Directory.Exists($"{rootDirectory}\\{3}\\{3}\\{3}\\{3}"));
            Assert.IsFalse(Directory.Exists($"{rootDirectory}\\{0}\\{0}\\{4}"));
        }

        [Test]
        public void TestBuildIndexWithDepth3And1Directory()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 1, 3);

            //Act
            var directories = _index.GetDirectories().ToList();
            
            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestBuildIndexWithDepth1And3Directories()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 3, 1);

            //Act
            var directories = _index.GetDirectories().ToList();
            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestBuildIndexWithDepth3And4Directories()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 4, 3);

            //Act
            var directories = _index.GetDirectories().ToList();

            //Assert
            Assert.That(directories.Count, Is.EqualTo(84));
            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}0")));
            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}0\\0")));
            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}0\\0\\0")));

            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}3")));
            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}3\\3")));
            Assert.IsTrue(directories.Any(x => x.EndsWith($"{developmentDirectory}3\\3\\3")));

            Assert.IsFalse(directories.Any(x => x.EndsWith($"{developmentDirectory}4")));
            Assert.IsFalse(directories.Any(x => x.EndsWith($"{developmentDirectory}0\\0\\0\\0")));
        }

    }
}