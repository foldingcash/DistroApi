namespace StatsDownload.Core.Tests
{
    using System.Collections.Generic;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestStatsFileParserProvider
    {
        private const string GoodStatsFile = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team
PS3EdOlkkola	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	70335";

        private const string MalformedHeader = @"Tue Dec 26 10:20:01 PST 2017
a malformed header not exactly matching the good stats file header
PS3EdOlkkola	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	70335";

        private const string MalformedUserRecord = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team
PS3EdOlkkola	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
a bad user record exists
anonymous	not an ulong	64221589	0
anonymous	13937689581	not an int	0
anonymous	13937689581	64221589	not an int
TheWasp	13660834951	734045	70335";

        private IAdditionalUserDataParserService additionalUserDataParserServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenInvoked_ParsesAdditionalUserData()
        {
            List<UserData> usersData = systemUnderTest.Parse(GoodStatsFile);

            foreach (UserData actual in usersData)
            {
                additionalUserDataParserServiceMock.Received(1).Parse(actual);
            }
        }

        [Test]
        public void Parse_WhenInvoked_ReturnsListOfUsersData()
        {
            List<UserData> actual = systemUnderTest.Parse(GoodStatsFile);

            Assert.That(actual.Count, Is.EqualTo(5));
        }

        [Test]
        public void Parse_WhenInvoked_ReturnsParsedUserData()
        {
            List<UserData> usersData = systemUnderTest.Parse(GoodStatsFile);
            UserData actual = usersData[2];

            Assert.That(actual.Name, Is.EqualTo("msi_TW"));
            Assert.That(actual.TotalPoints, Is.EqualTo(15889476570));
            Assert.That(actual.TotalWorkUnits, Is.EqualTo(359312));
            Assert.That(actual.TeamNumber, Is.EqualTo(31403));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(MalformedHeader)]
        public void Parse_WhenInvokedWithFileWithMalformedHeader_ReturnsEmptyUserDataList(string fileData)
        {
            List<UserData> actual = systemUnderTest.Parse(fileData);

            Assert.That(actual.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_WhenInvokedWithMalformedUserRecord_SkipsMalformedUserRecord()
        {
            List<UserData> actual = systemUnderTest.Parse(MalformedUserRecord);

            Assert.That(actual.Count, Is.EqualTo(4));
        }

        [SetUp]
        public void SetUp()
        {
            additionalUserDataParserServiceMock = Substitute.For<IAdditionalUserDataParserService>();

            systemUnderTest = new StatsFileParserProvider(additionalUserDataParserServiceMock);
        }
    }
}