using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core;
using Wororo.Utilities;

namespace NetworkDriveLauncher.UnitTests
{
    public class Tests
    {
        private PlainTextIndexBuilder indexBuilder;
        private PlainTextIndexConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            configuration = new PlainTextIndexConfiguration(config);
            indexBuilder = new PlainTextIndexBuilder(configuration);
        }

        [Test]
        public void TestConfigurationFile()
        {
            //Assert
            Assert.NotNull(configuration);
            Assert.NotNull(configuration.OutputFilename);
            Assert.NotNull(configuration.Depth);
            Assert.NotNull(configuration.OverwriteIndex);
            Assert.NotNull(configuration.RootDirectories);

            Assert.IsTrue(configuration.OutputFilename.IsNotEmpty());
            Assert.IsNotEmpty(configuration.RootDirectories);
            Assert.IsTrue(configuration.RootDirectories.FirstOrDefault().IsNotEmpty());
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
            var developmentDirectory = configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 1, 3);

            //Act
            var directories = indexBuilder.GetDirectories().ToList();
            
            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestBuildIndexWithDepth1And3Directories()
        {
            //Arrange
            var developmentDirectory = configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 3, 1);

            //Act
            var directories = indexBuilder.GetDirectories().ToList();
            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestBuildIndexWithDepth3And4Directories()
        {
            //Arrange
            var developmentDirectory = configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDirectories(developmentDirectory, 4, 3);

            //Act
            var directories = indexBuilder.GetDirectories().ToList();

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