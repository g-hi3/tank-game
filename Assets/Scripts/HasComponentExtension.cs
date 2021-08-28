using System;
using UnityEngine;

internal static class HasComponentExtension
{
  private static bool HasComponent(this GameObject gameObject, Type componentType)
  {
    return gameObject.TryGetComponent(componentType, out _);
  }

  public static bool HasComponent<T>(this GameObject gameObject)
  {
    return HasComponent(gameObject, typeof(T));
  }

  private static bool HasComponent(this Collider2D collider, Type componentType)
  {
    return collider.TryGetComponent(componentType, out _);
  }
  
  public static bool HasComponent<T>(this Collider2D collider)
  {
    return HasComponent(collider, typeof(T));
  }
}