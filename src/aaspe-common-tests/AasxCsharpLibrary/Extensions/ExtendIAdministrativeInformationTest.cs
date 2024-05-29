using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendIAdministrativeInformationTests
{
    private readonly IFixture _fixture;

    public ExtendIAdministrativeInformationTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void ToStringExtended_ShouldReturnVersionAndRevision_WhenFmtIs2()
    {
        // Arrange
        var mockAdminInfo = _fixture.Freeze<Mock<IAdministrativeInformation>>();
        mockAdminInfo.SetupGet(x => x.Version).Returns("1.0");
        mockAdminInfo.SetupGet(x => x.Revision).Returns("2.0");

        // Act
        var result = mockAdminInfo.Object.ToStringExtended(2);

        // Assert
        result.Should().Be("/1.0/2.0");
    }

    [Fact]
    public void ToStringExtended_ShouldReturnExtendedString_WhenFmtIsNot2()
    {
        // Arrange
        var mockAdminInfo = _fixture.Freeze<Mock<IAdministrativeInformation>>();
        mockAdminInfo.SetupGet(x => x.Version).Returns("1.0");
        mockAdminInfo.SetupGet(x => x.Revision).Returns("2.0");
        mockAdminInfo.SetupGet(x => x.TemplateId).Returns("tmpl123");
        mockAdminInfo.SetupGet(x => x.Creator).Returns((IReference?)null);

        // Act
        var result = mockAdminInfo.Object.ToStringExtended(1);

        // Assert
        result.Should().Be("[ver=1.0, rev=2.0, tmpl=tmpl123, crea=]");
    }

    [Fact]
    public void ToStringExtended_ShouldHandleNullProperties_WhenFmtIsNot2()
    {
        // Arrange
        var mockAdminInfo = _fixture.Freeze<Mock<IAdministrativeInformation>>();
        mockAdminInfo.SetupGet(x => x.Version).Returns("1.0");
        mockAdminInfo.SetupGet(x => x.Revision).Returns("2.0");
        mockAdminInfo.SetupGet(x => x.TemplateId).Returns((string)null);
        mockAdminInfo.SetupGet(x => x.Creator).Returns((IReference)null);

        // Act
        var result = mockAdminInfo.Object.ToStringExtended(1);

        // Assert
        result.Should().Be("[ver=1.0, rev=2.0, tmpl=, crea=]");
    }
}