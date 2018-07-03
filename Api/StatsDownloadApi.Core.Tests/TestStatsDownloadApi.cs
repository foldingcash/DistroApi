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

        private StatsDownloadApi systemUnderTest;

        [Test]
        public void ReturnTrue_WhenInvoked_ReturnsTrue()
        {
            var actual = systemUnderTest.ReturnTrue();

            Assert.That(actual, Is.True);
        }
    }
}