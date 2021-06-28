using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Palettes {

  public static class PaletteGenerator {

    public static Texture2D GeneratePalette(Texture2D texture) {
      var paletteColors = ExtractPaletteColors(texture);
      return CreateTextureFromPaletteColors(paletteColors);
    }

    private static Color32[] ExtractPaletteColors(Texture2D texture) {
      return texture.GetPixels32()
        .Distinct()
        .ToArray();
    }

    private static Texture2D CreateTextureFromPaletteColors(Color32[] paletteColors) {
      var texture = new Texture2D(paletteColors.Length, 1);
      texture.SetPixels32(paletteColors);
      return texture;
    }

    public static Texture2D GenerateDepalettized(Texture2D texture, Texture2D palette) {
      var depalettizedPixels = GetDepalettizedPixels(texture, palette);
      var depalettized = new Texture2D(texture.width, texture.height);
      depalettized.SetPixels32(depalettizedPixels);
      return depalettized;
    }

    public static Color32[] GetDepalettizedPixels(Texture2D texture, Texture2D palette) {
      IEnumerable<Color32> texturePixels = texture.GetPixels32();
      var paletteColors = palette.GetPixels32();
      for (byte i = 0; i < paletteColors.Length; i++) {
        var paletteColor = paletteColors[i];
        var indexColor = new Color32(i, i, i, i);
        texturePixels = texturePixels.ReplaceAll(paletteColor, indexColor);
      }
      return texturePixels.ToArray();
    }

    private static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> self, T original, T replacement) {
      return self != null
        ? self.Select(item => item.Equals(original) ? replacement : item)
        : throw new ArgumentNullException();
    }

  }

}
