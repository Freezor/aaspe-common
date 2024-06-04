using AasCore.Aas3_0;
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

    [Fact]
    public void ConvertFromV20_HandlesNullSourceSubmodel()
    {
        // Arrange
        Submodel submodel = null;
        AdminShellV20.Submodel sourceSubmodel = null;

        // Act
        var result = submodel.ConvertFromV20(sourceSubmodel);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertFromV20_ConvertsFromV20ToV30Properly()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV20.Submodel>();

        // Act
        var result = submodel.ConvertFromV20(sourceSubmodel);

        // Assert
        result.Should().NotBeNull();
        result.IdShort.Should().Be(sourceSubmodel.idShort);
        result.Id.Should().Be(sourceSubmodel.identification.id);
    }

    [Fact]
    public void ConvertFromV20_PerformsShallowCopyWhenShallowCopyIsTrue()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV20.Submodel>();

        // Act
        var result = submodel.ConvertFromV20(sourceSubmodel, shallowCopy: true);

        // Assert
        result.Should().NotBeNull();
        result.SubmodelElements.Should().NotBeNull();
    }

    [Fact]
    public void ConvertFromV20_MocksDependenciesCorrectly()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        var sourceSubmodel = _fixture.Create<AdminShellV20.Submodel>();

        // Act
        var result = submodel.ConvertFromV20(sourceSubmodel);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetModelReference_ReturnsCorrectReference()
    {
        // Arrange
        var submodel = _fixture.Create<Submodel>();
        submodel.Id = "TestId";
        submodel.SemanticId = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {new Key(KeyTypes.GlobalReference, "TestKey")});

        // Act
        var result = submodel.GetModelReference();

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ReferenceTypes.ModelReference);
        result.ReferredSemanticId.Should().Be(submodel.SemanticId);
        result.Keys.Should().ContainSingle().Which.Should().BeEquivalentTo(new Key(KeyTypes.Submodel, "TestId"));
    }

    [Fact]
    public void GetSemanticKey_ReturnsCorrectKeyForInstance()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.Kind, ModellingKind.Instance)
            .With(s => s.Id, "TestId")
            .Create();
        submodel.SemanticId = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {new Key(KeyTypes.GlobalReference, "TestKey")});

        // Act
        var result = submodel.GetSemanticKey();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new Key(KeyTypes.GlobalReference, "TestKey"));
    }

    [Fact]
    public void GetSemanticKey_ReturnsCorrectKeyForTemplate()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.Kind, ModellingKind.Template)
            .With(s => s.Id, "TestId")
            .Create();

        // Act
        var result = submodel.GetSemanticKey();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new Key(KeyTypes.Submodel, "TestId"));
    }

    [Fact]
    public void ConvertFromV20_WithNullSourceSubmodel_ReturnsNull()
    {
        // Arrange
        AdminShellV20.Submodel srcSubmodel = null;
        var sm = _fixture.Create<Submodel>();

        // Act
        var result = sm.ConvertFromV20(srcSubmodel);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertFromV20_WithNullSubmodel_ReturnsNull()
    {
        // Arrange
        var srcSubmodel = new AdminShellV20.Submodel();
        Submodel sm = null;

        // Act
        var result = sm.ConvertFromV20(srcSubmodel);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public void GetSemanticRef_InstanceSubmodel_ReturnsSemanticId()
    {
        // Arrange
        var semanticId = _fixture.Create<IReference>();
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.Kind, ModellingKind.Instance)
            .With(s => s.SemanticId, semanticId)
            .Create();

        // Act
        var result = submodel.GetSemanticRef();

        // Assert
        result.Should().Be(semanticId);
    }

    [Fact]
    public void GetSemanticRef_TemplateSubmodel_ReturnsNewReferenceWithSubmodelId()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.Kind, ModellingKind.Template)
            .With(s => s.Id, "TestId")
            .Create();

        // Act
        var result = submodel.GetSemanticRef();

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ReferenceTypes.ModelReference);
        result.Keys.Should().ContainSingle().Which.Type.Should().Be(KeyTypes.Submodel);
        result.Keys.Should().ContainSingle().Which.Value.Should().Be("TestId");
    }

    [Fact]
    public void SmeForWrite_WithNullSubmodelElements_InitializesAndReturnsNewList()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.SubmodelElements, (List<ISubmodelElement>)null)
            .Create();

        // Act
        var result = submodel.SmeForWrite();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        submodel.SubmodelElements.Should().NotBeNull();
        submodel.SubmodelElements.Should().BeSameAs(result);
    }

    [Fact]
    public void SmeForWrite_WithExistingSubmodelElements_ReturnsSameList()
    {
        // Arrange
        var existingElements = _fixture.Create<List<ISubmodelElement>>();
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.SubmodelElements, existingElements)
            .Create();

        // Act
        var result = submodel.SmeForWrite();

        // Assert
        result.Should().BeSameAs(existingElements);
    }

    [Fact]
    public void RecurseOnSubmodelElements_ExecutesLambdaForAllElements()
    {
        // Arrange
        var submodelElement1 = _fixture.Create<ISubmodelElement>();
        var submodelElement2 = _fixture.Create<ISubmodelElement>();
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.SubmodelElements, new List<ISubmodelElement> { submodelElement1, submodelElement2 })
            .Create();

        var state = new object();
        var lambdaMock = new Mock<Func<object?, List<IReferable>?, IReferable?, bool>>();
        lambdaMock.Setup(l => l(It.IsAny<object>(), It.IsAny<List<IReferable>>(), It.IsAny<IReferable>())).Returns(true);

        // Act
        submodel.RecurseOnSubmodelElements(state, lambdaMock.Object);

        // Assert
        lambdaMock.Verify(l => l(state, It.IsAny<List<IReferable>>(), submodelElement1), Times.Once);
        lambdaMock.Verify(l => l(state, It.IsAny<List<IReferable>>(), submodelElement2), Times.Once);
    }

    [Fact]
    public void FindSubmodelElementByIdShort_SubmodelElementDoesNotExist_ReturnsNull()
    {
        // Arrange
        var submodel = _fixture.Build<Submodel>().With(s => s.SubmodelElements, new List<ISubmodelElement>()).Create();
        var smeIdShort = "nonExistingIdShort";

        // Act
        var result = submodel.FindSubmodelElementByIdShort(smeIdShort);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SetAllParents_SubmodelElementsExist_SetsParentAndTimestamp()
    {
        // Arrange
        var timestamp = DateTime.Now;
        var submodelElement = _fixture.Create<ISubmodelElement>();
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.SubmodelElements, new List<ISubmodelElement> { submodelElement })
            .Create();

        // Act
        submodel.SetAllParents(timestamp);

        // Assert
        submodelElement.Parent.Should().Be(submodel);
        submodelElement.TimeStamp.Should().Be(timestamp);
        submodelElement.TimeStampCreate.Should().Be(timestamp);
    }

    [Fact]
    public void SetAllParents_NoSubmodelElements_DoesNothing()
    {
        // Arrange
        var timestamp = DateTime.Now;
        var submodel = _fixture.Build<Submodel>()
            .With(s => s.SubmodelElements, (List<ISubmodelElement>)null)
            .Create();

        // Act
        submodel.SetAllParents(timestamp);

        // Assert
        // No assertion needed as we just ensure no exceptions are thrown
    }

}