using NUnit.Framework;
using Palettes;
using UnityEngine;

namespace Assets.Scripts.Palettes.Tests {
  
  [TestFixture]
  public class PaletteGeneratorTest {

    private static readonly Color32 Red = new Color32(255, 0, 0, 255);
    private static readonly Color32 Blue = new Color32(0, 0, 255, 255);
    private static readonly Color32 Orange = new Color32(100, 65, 0, 255);
    private static readonly Color32 Aquamarine = new Color32(50, 100, 83, 255);

    [Test]
    public void GeneratePalette_FromRedTexture2D_ReturnedTextureColors32ContainsRed() {
      // Arrange
      var texture = GetRedTexture2D();

      // Act
      var palette = PaletteGenerator.GeneratePalette(texture);

      // Assert
      var paletteColors = palette.GetPixels32();
      Assert.That(paletteColors, Contains.Item(Red));
    }

    [Test]
    public void GeneratePalette_FromMultiColorTexture2D_ReturnedTextureColors32ContainAllColors() {
      // Arrange
      var colors = new Color32[] { Red, Blue, Orange, Aquamarine };
      var texture = GetMultiColorTexture2D(colors);

      // Act
      var palette = PaletteGenerator.GeneratePalette(texture);

      // Assert
      var paletteColors = palette.GetPixels32();
      foreach (var color in colors) {
        Assert.That(paletteColors, Contains.Item(color));
      }
    }

    [Test]
    public void GeneratePalette_FromRedTexture2D_ReturnedTextureWidthIsOne() {
      // Arrange
      var texture = GetRedTexture2D();

      // Act
      var palette = PaletteGenerator.GeneratePalette(texture);

      // Assert
      Assert.That(palette.width, Is.EqualTo(1));
    }

    [Test]
    public void GeneratePalette_FromMultiColorTexture2D_ReturnedTextureWidthIsNumberOfPaletteColors() {
      // Arrange
      var colors = new Color32[] { Red, Blue, Orange, Aquamarine };
      var texture = GetMultiColorTexture2D(colors);

      // Act
      var palette = PaletteGenerator.GeneratePalette(texture);

      // Assert
      var paletteColors = palette.GetPixels32();
      Assert.That(paletteColors.Length, Is.EqualTo(colors.Length));
    }

    [Test]
    public void GeneratePalette_FromTexture2DWithVerySimilarColors_PaletteContainsAllColors() {
      // Arrange
      var colors = new Color32[] {
        new Color32(15, 117, 88, 50),
        new Color32(15, 117, 88, 49),
        new Color32(15, 117, 89, 50),
        new Color32(15, 116, 88, 50),
        new Color32(14, 117, 88, 50)
      };
      var texture = GetMultiColorTexture2D(colors);

      // Act
      var palette = PaletteGenerator.GeneratePalette(texture);

      // Assert
      var paletteColors = palette.GetPixels32();
      foreach (var color in colors) {
        Assert.That(paletteColors, Contains.Item(color));
      }
    }

    private static Texture2D GetRedTexture2D() {
      var texture = new Texture2D(1, 1);
      texture.SetPixels32(new Color32[] { Red });
      return texture;
    }

    private static Texture2D GetMultiColorTexture2D(Color32[] colors) {
      var texture = new Texture2D(colors.Length, 1);
      texture.SetPixels32(colors);
      return texture;
    }
  }

}
