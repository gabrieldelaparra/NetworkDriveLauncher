using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core.Index;
using Wororo.Utilities;

namespace NetworkDriveLauncher.UnitTests
{
    public class PlainTextIndexTests
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
        public void TestCreateArrayDirectories()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            var sampleDirectories = new[]
            {
                "test\\sample-word\\depth\\",
                "alpha\\beta_gamma\\charlie\\tango\\",
                "test\\beta_gamma\\sample-word.this\\tango depth\\",
            };
            UnitTestsHelper.CreateDirectoriesFromArray(developmentDirectory, sampleDirectories);
            foreach (var directory in sampleDirectories)
            {
                var path = Path.Combine(developmentDirectory, directory);
                Assert.IsTrue(Directory.Exists(path));
            }
            developmentDirectory.DeleteIfExists();
        }

        [Test]
        public void TestCreateDepthDirectories()
        {
            //Arrange
            var rootDirectory = "TestCreateDirectory";
            rootDirectory.DeleteIfExists();

            //Act
            UnitTestsHelper.CreateDepthDirectories(rootDirectory, 4, 3);

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
            UnitTestsHelper.CreateDepthDirectories(developmentDirectory, 1, 3);

            //Act
            var directories = _index.GetDirectories().ToList();

            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));

            developmentDirectory.DeleteIfExists();
        }

        [Test]
        public void TestBuildIndexWithDepth1And3Directories()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDepthDirectories(developmentDirectory, 3, 1);

            //Act
            var directories = _index.GetDirectories().ToList();
            //Assert
            Assert.That(directories.Count, Is.EqualTo(3));

            developmentDirectory.DeleteIfExists();
        }

        [Test]
        public void TestBuildIndexWithDepth3And4Directories()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            UnitTestsHelper.CreateDepthDirectories(developmentDirectory, 4, 3);

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

            developmentDirectory.DeleteIfExists();
        }

        [Test]
        public void TestQueriesWithSampleWords()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            var sampleDirectories = new[]
            {
                "test\\sample-word\\depth\\",
                "alpha\\beta_gamma\\charlie\\tango\\",
                "test_other\\beta_gamma\\sample-word.this\\after depth\\",
            };

            UnitTestsHelper.CreateDirectoriesFromArray(developmentDirectory, sampleDirectories);

            _index.BuildIndexAsync().Wait();

            var queryTerms = new[] { "test" };
            var results = _index.Query(queryTerms).OrderByDescending(x => x.Score).ToArray();
            var expectedResults = new[]
            {
                "test",
                "test_other",
                "test\\sample-word",
                "test_other\\beta_gamma",
                "test\\sample-word\\depth",
                "test_other\\beta_gamma\\sample-word.this",
                "test_other\\beta_gamma\\sample-word.this\\after depth",
            };
            Assert.AreEqual(expectedResults.Length, results.Length);
            for (int i = 0; i < expectedResults.Length; i++)
            {
                var expected = expectedResults[i];
                var actual = results[i].Title;
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TestQueriesWith24554()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            var sampleDirectories = new[]
            {
                "22758-LyonDellBasel Configurateur d'état-ReJae\\",
                "24554-Vilnius-Railway-Elecnor-bec\\",
                "24554-Vilnius-Railway-Elecnor-bec\\3-Design-Documents\\02-Basedesign\\",
                "16853-Duri 150KV-cm\\3-Design-Documents\\02-Basedesign\\",
                "19xxx-DA_Demo_2017\\WMWare_Win_7_x86_SP1 base\\",
            };
            //The last item, "after depth" should not be in the index, since Depth == 3.

            UnitTestsHelper.CreateDirectoriesFromArray(developmentDirectory, sampleDirectories);

            _index.BuildIndexAsync().Wait();

            var queryTerms = new[] { "24554"};
            var results = _index.Query(queryTerms).OrderByDescending(x => x.Score).ToArray();
            var expectedResult = "24554-Vilnius-Railway-Elecnor-bec";
            Assert.AreEqual(expectedResult, results.FirstOrDefault().Title);
            
        }

        [Test]
        public void TestQueriesWith24554Base()
        {
            //Arrange
            var developmentDirectory = _configuration.RootDirectories.FirstOrDefault();
            Assert.IsTrue(developmentDirectory.IsNotEmpty());
            developmentDirectory.DeleteIfExists();
            var sampleDirectories = new[]
            {
                "22758-LyonDellBasel Configurateur d'état-ReJae\\",
                "22758-LyonDellBasel Configurateur d'état-ReJae\\3-Design-Documents\\02-Basedesign\\",
                "24554-Vilnius-Railway-Elecnor-bec\\",
                "24554-Vilnius-Railway-Elecnor-bec\\3-Design-Documents\\02-Basedesign\\",
                "16853-Duri 150KV-cm\\3-Design-Documents\\02-Basedesign\\",
                "19xxx-DA_Demo_2017\\WMWare_Win_7_x86_SP1 base\\",
            };
            //The last item, "after depth" should not be in the index, since Depth == 3.

            UnitTestsHelper.CreateDirectoriesFromArray(developmentDirectory, sampleDirectories);

            _index.BuildIndexAsync().Wait();

            var queryTerms = new[] { "24554", "base" };
            var results = _index.Query(queryTerms).OrderByDescending(x => x.Score).ToArray();
            var expectedResult = "24554-Vilnius-Railway-Elecnor-bec\\3-Design-Documents\\02-Basedesign";
            Assert.AreEqual(expectedResult, results.FirstOrDefault().Title);
        }

    }
}