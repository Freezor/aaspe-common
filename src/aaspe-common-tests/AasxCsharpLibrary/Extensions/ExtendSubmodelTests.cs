﻿using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AasxCompatibilityModels;
using AdminShellNS;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendSubmodel))]
public class ExtendSubmodelTests
{
    private readonly IFixture _fixture;

    public ExtendSubmodelTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _fixture.Customizations.Add(
            new TypeRelay(
                typeof(IAdministrativeInformation),
                typeof(AdministrativeInformation)));
        
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void RecurseOnReferable_NullSubmodel_DoesThrow()
    {
        // Act & Assert
        Action act = () => ((Submodel) null).RecurseOnReferable(null, null);
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Remove_NullSubmodel_DoesNotThrow()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();

        // Act & Assert
        Action act = () => submodel.Remove(null);
        act.Should().NotThrow();
    }

    [Fact]
    public void AddChild_NullSubmodelElement_ReturnsNull()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();

        // Act
        var result = submodel.AddChild(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddChild_SubmodelElement_AddsChildToSubmodel()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var childElement = _fixture.Create<ISubmodelElement>();

        // Act
        var result = submodel.AddChild(childElement);

        // Assert
        result.Should().Be(childElement);
        submodel.SubmodelElements.Should().Contain(childElement);
    }

    [Fact]
    public void ToCaptionInfo_ReturnsTupleWithCaptionAndInfo()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.IdShort, "TestSubmodel")
            .With(s => s.Id, "TestId")
            .Create();

        // Creating a mock instance of IAdministrativeInformation using Moq
        var administration = new Mock<IAdministrativeInformation>();
        administration.SetupGet(a => a.Version).Returns("1");
        administration.SetupGet(a => a.Revision).Returns("2");

        submodel.Administration = administration.Object;

        // Act
        var result = submodel.ToCaptionInfo();

        // Assert
        result.Item1.Should().Be("\"TestSubmodel\" V1.2");
        result.Item2.Should().Be("[TestId]");
    }

    [Fact]
    public void FindAllReferences_ReturnsLocatedReferences()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var referenceElement = _fixture.Build<ReferenceElement>()
            .With(re => re.Value, _fixture.Create<Reference>())
            .Create();
        var relationshipElement = _fixture.Build<RelationshipElement>()
            .With(rl => rl.First, _fixture.Create<Reference>())
            .With(rl => rl.Second, _fixture.Create<Reference>())
            .Create();
        submodel.AddChild(referenceElement);
        submodel.AddChild(relationshipElement);

        // Act
        var result = submodel.FindAllReferences();

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Validate_ReturnsEarlyWhenResultsIsNull()
    {
        // Arrange
        Submodel submodel = null;
        AasValidationRecordList results = null;

        // Act
        Action action = () => submodel.Validate(results);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Validate_PerformsValidationCorrectly()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var results = new AasValidationRecordList();

        // Act
        submodel.Validate(results);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void ConvertFromV10_HandlesNullParameters()
    {
        // Arrange
        Submodel submodel = null;
        AdminShellV10.Submodel sourceSubmodel = null;

        // Act
        var result = submodel.ConvertFromV10(sourceSubmodel);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertFromV10_ConvertsFromV10ToV30Properly()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV10.Submodel>();

        // Act
        var result = submodel.ConvertFromV10(sourceSubmodel);

        // Assert
        result.Should().NotBeNull();
        result.IdShort.Should().Be(sourceSubmodel.idShort);
    }

    [Fact]
    public void ConvertFromV10_PerformsShallowCopyWhenShallowCopyIsTrue()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV10.Submodel>();

        // Act
        var result = submodel.ConvertFromV10(sourceSubmodel, shallowCopy: true);

        // Assert
        result.Should().NotBeNull();
        result.SubmodelElements.Should().NotBeNull();
    }

    [Fact]
    public void ConvertFromV10_MocksDependenciesCorrectly()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV10.Submodel>();

        // Act
        var result = submodel.ConvertFromV10(sourceSubmodel);

        // Assert
        result.Should().NotBeNull();
    }
}