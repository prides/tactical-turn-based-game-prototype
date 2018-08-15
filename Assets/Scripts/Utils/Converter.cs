using System;
using System.Collections.Generic;
using UnityEngine;

public class Converter {
  public static T Convert<T>(SettlersEngine.Point point) {
    return (T)Convert(point, typeof(T));
  }

  public static T Convert<T>(Vector2Int vector2Int) {
    return (T)Convert(vector2Int, typeof(T));
  }

  private static object Convert(SettlersEngine.Point point, Type type) {
    switch (type.FullName) {
      case "Vector2Int":
        return new Vector2Int(point.X, point.Y);
      default:
        Debug.LogError("Can't convert unknown type " + type.FullName);
        return null;
    }
  }

  private static object Convert(Vector2Int vector2Int, Type type) {
    switch (type.FullName) {
      case "SettlersEngine.Point":
        return new SettlersEngine.Point() { X = vector2Int.x, Y = vector2Int.y };
      default:
        Debug.LogError("Can't convert unknown type " + type.FullName);
        return null;
    }
  }

  public static Vector2Int[] ConvertPath(LinkedList<AStarPathNode> path) {
    if (path == null) {
      return null;
    }
    Vector2Int[] pathVec = new Vector2Int[path.Count];
    int i = 0;
    foreach (AStarPathNode node in path) {
      pathVec[i].x = node.X;
      pathVec[i].y = node.Y;
      i++;
    }
    return pathVec;
  }
}
