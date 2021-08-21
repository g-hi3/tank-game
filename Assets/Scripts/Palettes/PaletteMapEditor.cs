using System.IO;
using UnityEditor;
using UnityEngine;

namespace Palettes {
  
  [CustomEditor(typeof(PaletteMap))]
  public class PaletteMapEditor : Editor {

    private PaletteMap PaletteMap => target as PaletteMap;
    
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
      var originalTexture = PaletteMap.texture;
      if (GUILayout.Button("Generate Original Palette")) {
        var paletteTexture = PaletteGenerator.GeneratePalette(originalTexture);
        var originalPath = AssetDatabase.GetAssetPath(originalTexture);
        var palettePath = Path.Combine(originalPath, $"../{originalTexture.name}_originalPalette.png");
        var pngBytes = paletteTexture.EncodeToPNG();
        File.WriteAllBytes(palettePath, pngBytes);
        AssetDatabase.Refresh();
      }
      if (PaletteMap.depalettizedTexture != null
          || !GUILayout.Button("Generate Depalettized Texture")) {
        return;
      }

      PaletteMap.depalettizedTexture = CreateDepalettizedTexture(originalTexture);
    }

    private static Texture2D CreateDepalettizedTexture(Texture2D texture) {
      var originalPath = AssetDatabase.GetAssetPath(texture);
      var palettePath = originalPath.Replace(".png", "_originalPalette.png");
      var palette = AssetDatabase.LoadAssetAtPath<Texture2D>(palettePath);
      var depalettizedTexture = PaletteGenerator.GenerateDepalettized(texture, palette);
      var depalettizedPath = Path.Combine(originalPath, $"../{texture.name}_depalettized.png");
      var pngBytes = depalettizedTexture.EncodeToPNG();
      File.WriteAllBytes(depalettizedPath, pngBytes);
      AssetDatabase.Refresh();
      return AssetDatabase.LoadAssetAtPath<Texture2D>(depalettizedPath);
    }
    
  }

}