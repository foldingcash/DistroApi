namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    [TestFixture]
    public class TestStatsFileParserProvider
    {
        [SetUp]
        public void SetUp()
        {
            additionalUserDataParserServiceMock = Substitute.For<IAdditionalUserDataParserService>();

            statsFileDateTimeFormatsAndOffsetServiceMock = Substitute.For<IStatsFileDateTimeFormatsAndOffsetService>();
            statsFileDateTimeFormatsAndOffsetServiceMock
                .GetStatsFileDateTimeFormatsAndOffset().Returns(dateTimeFormatsAndOffset);

            systemUnderTest = NewStatsFileParserProvider(additionalUserDataParserServiceMock,
                statsFileDateTimeFormatsAndOffsetServiceMock);
        }

        private const string EmptyStatsFile = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team";

        private const string GoodHeaderAndUsers = @"name	newcredit	sum(total)	team
	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	4294967295";

        private const string GoodStatsFile = @"Tue Dec 26 10:20:01 PST 2017
name	newcredit	sum(total)	team
	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	4294967295";

        private const string GoodStatsFile2 = @"Tue Dec 26 10:20:01 PST 2017
name	score	wu	team
	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	4294967295";

        private const string GoodStatsFileWithDaylightSavingsTimeZone = @"Tue Dec 26 10:20:01 PDT 2017
name	newcredit	sum(total)	team
	25882218711	458785	224497
war	20508731397	544139	37651
msi_TW	15889476570	359312	31403
anonymous	13937689581	64221589	0
TheWasp	13660834951	734045	4294967295";

        private const string GoodStatsFileWithOnlyNewLine = "Tue Dec 26 10:20:01 PST 2017\n"
                                                            + "name	newcredit	sum(total)	team\n"
                                                            + "PS3EdOlkkola	25882218711	458785	224497\n"
                                                            + "war	20508731397	544139	37651\n"
                                                            + "msi_TW	15889476570	359312	31403\n"
                                                            + "anonymous	13937689581	64221589	0\n"
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

        private readonly (string format, int hourOffset)[] dateTimeFormatsAndOffset =
        {
            ("ddd MMM  d HH:mm:ss GMT yyyy", 0),
            ("ddd MMM dd HH:mm:ss GMT yyyy", 0),
            ("ddd MMM  d HH:mm:ss CDT yyyy", -5),
            ("ddd MMM dd HH:mm:ss CDT yyyy", -5),
            ("ddd MMM  d HH:mm:ss CST yyyy", -6),
            ("ddd MMM dd HH:mm:ss CST yyyy", -6),
            ("ddd MMM  d HH:mm:ss PDT yyyy", -7),
            ("ddd MMM dd HH:mm:ss PDT yyyy", -7),
            ("ddd MMM  d HH:mm:ss PST yyyy", -8),
            ("ddd MMM dd HH:mm:ss PST yyyy", -8)
        };

        private IStatsFileDateTimeFormatsAndOffsetService statsFileDateTimeFormatsAndOffsetServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsFileParserProvider(null, statsFileDateTimeFormatsAndOffsetServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsFileParserProvider(additionalUserDataParserServiceMock, null));
        }

        [Test]
        public void Parse_WhenInvoked_ParsesAdditionalUserData()
        {
            IEnumerable<UserData> usersData = InvokeParse().UsersData;

            foreach (UserData actual in usersData)
            {
                additionalUserDataParserServiceMock.Received(1).Parse(actual);
            }
        }

        [TestCase(GoodStatsFile, 2017, 12, 26, 18, 20, 1)]
        [TestCase(GoodStatsFileWithDaylightSavingsTimeZone, 2017, 12, 26, 17, 20, 1)]
        [TestCase("Tue Dec  5 10:20:01 PST 2017\n" + GoodHeaderAndUsers, 2017, 12, 5, 18, 20, 1)]
        [TestCase("Tue Dec  5 10:20:01 PDT 2017\n" + GoodHeaderAndUsers, 2017, 12, 5, 17, 20, 1)]
        [TestCase("Tue Dec  5 10:20:01 CST 2017\n" + GoodHeaderAndUsers, 2017, 12, 5, 16, 20, 1)]
        [TestCase("Tue Dec  5 10:20:01 CDT 2017\n" + GoodHeaderAndUsers, 2017, 12, 5, 15, 20, 1)]
        [TestCase("Tue Dec  5 10:20:01 GMT 2017\n" + GoodHeaderAndUsers, 2017, 12, 5, 10, 20, 1)]
        public void Parse_WhenInvoked_ReturnsDownloadDateTimeInUTC(string fileData, int year, int month, int day,
                                                                   int hour, int minute, int second)
        {
            DateTime actual = InvokeParse(fileData).DownloadDateTime;

            Assert.That(actual, Is.EqualTo(new DateTime(year, month, day, hour, minute, second)));
        }

        [TestCase(GoodStatsFile, 5)]
        [TestCase(GoodStatsFile2, 5)]
        [TestCase(EmptyStatsFile, 0)]
        [TestCase(GoodStatsFileWithOnlyNewLine, 5)]
        [TestCase(GoodStatsFileWithDaylightSavingsTimeZone, 5)]
        [TestCase("Tue Dec  5 10:20:01 PDT 2017\n" + GoodHeaderAndUsers, 5)]
        [TestCase("Tue Dec  5 10:20:01 PST 2017\n" + GoodHeaderAndUsers, 5)]
        [TestCase("Tue Dec  5 10:20:01 GMT 2017\n" + GoodHeaderAndUsers, 5)]
        public void Parse_WhenInvoked_ReturnsListOfUsersData(string fileData, int expectedCount)
        {
            IEnumerable<UserData> actual = InvokeParse(fileData).UsersData;

            Assert.That(actual.Count(), Is.EqualTo(expectedCount));
        }

        [Test]
        public void Parse_WhenInvoked_ReturnsParsedUserData()
        {
            IEnumerable<UserData> usersData = InvokeParse().UsersData;
            UserData actual = usersData.ElementAt(2);

            Assert.That(actual.LineNumber, Is.EqualTo(5));
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
            var actual = new List<FailedUserData>(InvokeParse(MalformedUserRecord).FailedUsersData);

            Assert.That(actual.Count, Is.EqualTo(4));

            Assert.That(actual[0].LineNumber, Is.EqualTo(6));
            Assert.That(actual[0].RejectionReason, Is.EqualTo(RejectionReason.UnexpectedFormat));
            Assert.That(actual[0].Data, Is.EqualTo("a bad user record exists"));
            Assert.That(actual[0].UserData, Is.Null);

            Assert.That(actual[1].LineNumber, Is.EqualTo(7));
            Assert.That(actual[1].RejectionReason, Is.EqualTo(RejectionReason.FailedParsing));
            Assert.That(actual[1].Data, Is.EqualTo("anonymous	not an ulong	64221589	489"));
            Assert.That(actual[1].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[1].UserData.TotalPoints, Is.EqualTo(0));
            Assert.That(actual[1].UserData.TotalWorkUnits, Is.EqualTo(64221589));
            Assert.That(actual[1].UserData.TeamNumber, Is.EqualTo(489));

            Assert.That(actual[2].LineNumber, Is.EqualTo(8));
            Assert.That(actual[2].RejectionReason, Is.EqualTo(RejectionReason.FailedParsing));
            Assert.That(actual[2].Data, Is.EqualTo("anonymous	13937689581	not an int	123"));
            Assert.That(actual[2].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[2].UserData.TotalPoints, Is.EqualTo(13937689581));
            Assert.That(actual[2].UserData.TotalWorkUnits, Is.EqualTo(0));
            Assert.That(actual[2].UserData.TeamNumber, Is.EqualTo(123));

            Assert.That(actual[3].LineNumber, Is.EqualTo(9));
            Assert.That(actual[3].RejectionReason, Is.EqualTo(RejectionReason.FailedParsing));
            Assert.That(actual[3].Data, Is.EqualTo("anonymous	13937689581	64221589	not an int"));
            Assert.That(actual[3].UserData.Name, Is.EqualTo("anonymous"));
            Assert.That(actual[3].UserData.TotalPoints, Is.EqualTo(13937689581));
            Assert.That(actual[3].UserData.TotalWorkUnits, Is.EqualTo(64221589));
            Assert.That(actual[3].UserData.TeamNumber, Is.EqualTo(0));
        }

        private ParseResults InvokeParse(string fileData = GoodStatsFile)
        {
            return systemUnderTest.Parse(fileData);
        }

        private IStatsFileParserService NewStatsFileParserProvider(
            IAdditionalUserDataParserService additionalUserDataParserService,
            IStatsFileDateTimeFormatsAndOffsetService statsFileDateTimeFormatsAndOffsetService)
        {
            return new StatsFileParserProvider(additionalUserDataParserService,
                statsFileDateTimeFormatsAndOffsetService);
        }
    }
}