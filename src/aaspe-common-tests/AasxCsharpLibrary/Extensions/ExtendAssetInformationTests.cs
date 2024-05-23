using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions
{
    public class ExtendAssetInformationTests
    {
        private readonly IFixture _fixture;

        public ExtendAssetInformationTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void ToCaptionInfo_ShouldReturnValidTuple()
        {
            // Arrange
            var assetInformation = _fixture.Create<AssetInformation>();
            assetInformation.GlobalAssetId = "TestGlobalAssetId";

            // Act
            var result = assetInformation.ToCaptionInfo();

            // Assert
            result.Item1.Should().Be("AssetInformation");
            result.Item2.Should().Be("TestGlobalAssetId");
        }

        [Fact]
        public void ConvertFromV10_ShouldConvertCorrectly()
        {
            // Arrange
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var sourceAsset = _fixture.Create<AasxCompatibilityModels.AdminShellV10.Asset>();
            var assetInformation = _fixture.Create<AssetInformation>();

            // Act
            var convertedAssetInfo = assetInformation.ConvertFromV10(sourceAsset);

            // Assert
            convertedAssetInfo.AssetKind.Should().Be(sourceAsset.kind.IsType ? AssetKind.Type : AssetKind.Instance);
            convertedAssetInfo.GlobalAssetId.Should().Be(sourceAsset.identification.id);
        }

        [Fact]
        public void ConvertFromV20_ShouldConvertCorrectly()
        {
            // Arrange
            var sourceAsset = _fixture.Create<AasxCompatibilityModels.AdminShellV20.Asset>();
            var assetInformation = _fixture.Create<AssetInformation>();

            // Act
            var convertedAssetInfo = assetInformation.ConvertFromV20(sourceAsset);

            // Assert
            convertedAssetInfo.AssetKind.Should().Be(sourceAsset.kind.IsType ? AssetKind.Type : AssetKind.Instance);
            convertedAssetInfo.GlobalAssetId.Should().Be(sourceAsset.identification.id);
        }
    }
}
