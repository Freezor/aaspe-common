using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendEntityTests))]
public class ExtendEntityTests
{
    private readonly Fixture _fixture;

    public ExtendEntityTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Add_ShouldAddSubmodelElementToEntity()
    {
        // Arrange
        var entity = _fixture.Create<Entity>();
        var submodelElement = _fixture.Create<ISubmodelElement>();

        // Act
        entity.Add(submodelElement);

        // Assert
        entity.Statements.Should().Contain(submodelElement);
    }

    [Fact]
    public void Remove_ShouldRemoveSubmodelElementFromEntity()
    {
        // Arrange
        var entity = _fixture.Create<Entity>();
        var submodelElement = _fixture.Create<ISubmodelElement>();
        entity.Add(submodelElement);

        // Act
        entity.Remove(submodelElement);

        // Assert
        entity.Statements.Should().NotContain(submodelElement);
    }

    [Fact]
    public void AddChild_ShouldAddChildSubmodelElementToEntity()
    {
        // Arrange
        var entity = _fixture.Create<Entity>();
        var childSubmodelElement = _fixture.Create<ISubmodelElement>();

        // Act
        var result = entity.AddChild(childSubmodelElement);

        // Assert
        entity.Statements.Should().Contain(childSubmodelElement);
        result.Should().Be(childSubmodelElement);
    }

    [Fact]
    public void ConvertFromV20_ShouldConvertEntityFromV20()
    {
        // Arrange
        var entity = _fixture.Create<Entity>();
        var sourceEntity = _fixture.Create<AasxCompatibilityModels.AdminShellV20.Entity>();

        // Act
        var convertedEntity = entity.ConvertFromV20(sourceEntity);

        // Assert
        convertedEntity.Should().NotBeNull();
    }
}