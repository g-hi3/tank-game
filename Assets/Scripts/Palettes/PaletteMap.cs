using UnityEngine;

namespace Palettes {
  
  [CreateAssetMenu(menuName = "Palettes/Palette Map")]
  public class PaletteMap : ScriptableObject {

    public Texture2D texture;
    public Texture2D depalettizedTexture;
    [SerializeField] private PaletteConfiguration[] palettes;
    
  }
}