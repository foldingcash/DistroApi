namespace StatsDownload.Core.Tests
{
    using System.Collections.Generic;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestStatsFileParserProvider
    {
        private const string EmptyStatsFile = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team";

        private const string GoodStatsFile = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team
	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	4294967295";

        private const string GoodStatsFileWithOnlyNewLine =
            "Tue Dec 26 10:20:01 PST 2017\n" + "name	newcredit	sum(total)	team\n"
            + "PS3EdOlkkola	25882218711	458785	224497\n" + "war	20508731397	544139	37651\n"
            + "msi_TW	15889476570	359312	31403\n" + "anonymous	13937689581	64221589	0\n"
            + "TheWasp	13660834951	734045	70335";

        private const string MalformedDateTime = @"a malformed date time not exactly matching the good stats file header
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
anonymous	not an ulong	64221589	489
anonymous	13937689581	not an int	123
anonymous	13937689581	64221589	not an int
TheWasp	13660834951	734045	70335";

        private IAdditionalUserDataParserService additionalUserDataParserServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenInvoked_ParsesAdditionalUserData()
        {
            List<UserData> usersData = InvokeParse().UsersData;

            foreach (UserData actual in usersData)
            {
                additionalUserDataParserServiceMock.Received(1).Parse(actual);
            }
        }

        [TestCase(GoodStatsFile, 5)]
        [TestCase(EmptyStatsFile, 0)]
        [TestCase(GoodStatsFileWithOnlyNewLine, 5)]
        public void Parse_WhenInvoked_ReturnsListOfUsersData(string fileData, int expectedCount)
        {
            List<UserData> actual = InvokeParse(fileData).UsersData;

            Assert.That(actual.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void Parse_WhenInvoked_ReturnsParsedUserData()
        {
            List<UserData> usersData = InvokeParse().UsersData;
            UserData actual = usersData[2];

            Assert.That(actual.Name, Is.EqualTo("msi_TW"));
            Assert.That(actual.TotalPoints, Is.EqualTo(15889476570));
            Assert.That(actual.TotalWorkUnits, Is.EqualTo(359312));
            Assert.That(actual.TeamNumber, Is.EqualTo(31403));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(MalformedHeader)]
        [TestCase(MalformedDateTime)]
        public void Parse_WhenInvokedWithFileWithMalformedHeader_ThrowsInvalidStatsFileException(string fileData)
        {
            Assert.Throws<InvalidStatsFileException>(() => InvokeParse(fileData));
        }

        [Test]
        public void Parse_WhenInvokedWithMalformedUserRecord_ReturnsListOfFailedUsersData()
        {
            List<FailedUserData> actual = InvokeParse(MalformedUserRecord).FailedUsersData;

            Assert.That(actual.Count, Is.EqualTo(4));

            Assert.That(actual[0].Data, Is.EqualTo("a bad user record exists"));
            Assert.That(actual[0].UserData, Is.Null);

            Assert.That(actual[1].Data, Is.EqualTo("anonymous	not an ulong	64221589	489"));
            Assert.That(actual[1].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[1].UserData.TotalPoints, Is.EqualTo(0));
            Assert.That(actual[1].UserData.TotalWorkUnits, Is.EqualTo(64221589));
            Assert.That(actual[1].UserData.TeamNumber, Is.EqualTo(489));

            Assert.That(actual[2].Data, Is.EqualTo("anonymous	13937689581	not an int	123"));
            Assert.That(actual[2].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[2].UserData.TotalPoints, Is.EqualTo(13937689581));
            Assert.That(actual[2].UserData.TotalWorkUnits, Is.EqualTo(0));
            Assert.That(actual[2].UserData.TeamNumber, Is.EqualTo(123));

            Assert.That(actual[3].Data, Is.EqualTo("anonymous	13937689581	64221589	not an int"));
            Assert.That(actual[3].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[3].UserData.TotalPoints, Is.EqualTo(13937689581));
            Assert.That(actual[3].UserData.TotalWorkUnits, Is.EqualTo(64221589));
            Assert.That(actual[3].UserData.TeamNumber, Is.EqualTo(0));
        }

        [SetUp]
        public void SetUp()
        {
            additionalUserDataParserServiceMock = Substitute.For<IAdditionalUserDataParserService>();

            systemUnderTest = new StatsFileParserProvider(additionalUserDataParserServiceMock);
        }

        private ParseResults InvokeParse(string fileData = GoodStatsFile)
        {
            return systemUnderTest.Parse(fileData);
        }
    }
}