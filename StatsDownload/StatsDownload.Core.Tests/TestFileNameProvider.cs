namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileNameProvider
    {
        private IGuidService guidServiceMock;

        private IFileNameService systemUnderTest;

        [Test]
        public void GetRandomFileNamePath_WhenInvoked_ReturnsGuidAppendedToDirectory()
        {
            string actual = systemUnderTest.GetRandomFileNamePath(@"C:\Temp");

            Assert.That(actual, Is.EqualTo($@"C:\Temp\{Guid.Empty}.daily_user_summary.txt.bz2"));
        }

        [SetUp]
        public void SetUp()
        {
            guidServiceMock = Substitute.For<IGuidService>();
            guidServiceMock.NextGuid().Returns(Guid.Empty);

            systemUnderTest = new FileNameProvider(guidServiceMock);
        }
    }
}