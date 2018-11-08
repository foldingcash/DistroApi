namespace StatsDownload.Core.Tests
{
    using Implementations;
    using Interfaces;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsFileDateTimeFormatsAndOffsetProvider
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new StatsFileDateTimeFormatsAndOffsetProvider();
        }

        private IStatsFileDateTimeFormatsAndOffsetService systemUnderTest;

        [Test]
        public void GetStatsFileDateTimeFormatsAndOffset_WhenInvoked_ReturnsDateTimeFormatsAndOffsetsConstants()
        {
            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.IsSupersetOf(actual, Constants.StatsFile.DateTimeFormatsAndOffset);
        }
    }
}