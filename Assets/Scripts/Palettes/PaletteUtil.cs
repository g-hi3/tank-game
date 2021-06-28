using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Palettes {
  public static class PaletteUtil {

    public static IEnumerable<Color32> ExtractPaletteColors(Texture2D texture) {
      return texture.GetPixels32()
        .Distinct();
    }
    
  }
}