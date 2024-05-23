using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendAssetAdministrationShellTests
{
    private readonly IFixture _fixture;

    public ExtendAssetAdministrationShellTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void FindAllReferences_ShouldReturnLocatedReferences()
    {
        // Arrange
        var assetAdministrationShell = _fixture.Create<IAssetAdministrationShell>();
        assetAdministrationShell.Submodels = new List<IReference>
        {
            _fixture.Create<IReference>(),
            _fixture.Create<IReference>(),
            _fixture.Create<IReference>()
        };

        // Act
        var result = assetAdministrationShell.FindAllReferences();

        // Assert
        result.Should().HaveCount(assetAdministrationShell.Submodels.Count);
        foreach (var locatedReference in result)
        {
            locatedReference.Should().BeOfType<LocatedReference>();
        }
    }

    [Fact]
    public void AddSubmodelReference_ShouldAddSubmodelReference()
    {
        // Arrange
        var assetAdministrationShell = _fixture.Create<IAssetAdministrationShell>();
        var newSubmodelReference = _fixture.Create<IReference>();

        // Act
        assetAdministrationShell.AddSubmodelReference(newSubmodelReference);

        // Assert
        assetAdministrationShell.Submodels.Should().Contain(newSubmodelReference);
    }
    
    [Fact]
    public void GetFriendlyName_ShouldReturnValidFriendlyName()
    {
        // Arrange
        var assetAdministrationShell = _fixture.Build<AssetAdministrationShell>().With(x => x.IdShort, "Test_AAS_1").Create();

        // Act
        var friendlyName = assetAdministrationShell.GetFriendlyName();

        // Assert
        friendlyName.Should().Be("Test_AAS_1");
    }
}