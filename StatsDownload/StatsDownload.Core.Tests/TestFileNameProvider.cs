namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileNameProvider
    {
        private IDateTimeService dateTimeServiceMock;

        private IFileNameService systemUnderTest;

        [Test]
        public void GetRandomFileNamePath_WhenInvoked_ReturnsGuidAppendedToDirectory()
        {
            string actual = systemUnderTest.GetRandomFileNamePath(@"C:\Temp");

            Assert.That(actual, Is.EqualTo($@"C:\Temp\{DateTime.MaxValue.ToFileTime()}.daily_user_summary.txt.bz2"));
        }

        [SetUp]
        public void SetUp()
        {
            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(DateTime.MaxValue);

            systemUnderTest = new FileNameProvider(dateTimeServiceMock);
        }
    }
}