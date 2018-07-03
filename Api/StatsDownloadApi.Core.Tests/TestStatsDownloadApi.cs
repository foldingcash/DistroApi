namespace StatsDownloadApi.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsDownloadApi
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new StatsDownloadApi();
        }

        private IStatsDownloadApi systemUnderTest;

        [Test]
        public void GetDistro_WhenInvoked_ReturnsSuccessDistroResponse()
        {
            var actual = systemUnderTest.GetDistro();

            Assert.That(actual.Success, Is.True);
        }
    }
}