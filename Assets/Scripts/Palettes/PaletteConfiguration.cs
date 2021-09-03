using UnityEngine;

namespace Palettes
{
  [CreateAssetMenu(menuName = "Palettes/Palette Configuration")]
  public class PaletteConfiguration : ScriptableObject
  {
    [SerializeField] private Texture2D palette;
    [SerializeField] private Color32[] colors;
  }
}
