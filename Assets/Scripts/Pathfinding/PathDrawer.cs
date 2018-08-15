using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour {
  private enum ArrowDirection {
    Right = 0,
    Down,
    Left,
    Up,

    Unknown = 9
  }
  private static PathDrawer instance;

  [SerializeField]
  private SpriteRenderer[] arrows;

  private void Awake() {
    if (instance != null) {
      Destroy(this);
      return;
    }
    instance = this;
  }

  public static PathDrawer GetInstance() {
    return instance;
  }

  private void DisableArrows() {
    foreach (SpriteRenderer sr in arrows) {
      sr.gameObject.SetActive(false);
    }
  }

  public void ShowPath(Vector2Int[] path) {
    if (path == null || path.Length <= 1) {
      return;
    }

    List<Vector2Int> optimizedPath = OptimizePath(path);

    int currentArrowIndex = 0;
    SpriteRenderer currentArrow;
    ArrowDirection direction;
    Vector2 arrowSize;
    Vector2 center;

    for (int i = 1; i < optimizedPath.Count; i++) {
      currentArrow = arrows[currentArrowIndex];
      currentArrow.gameObject.SetActive(true);

      direction = GetArrowDirection(optimizedPath[i - 1], optimizedPath[i]);

      Vector3 rot = currentArrow.transform.localRotation.eulerAngles;
      rot.y = (int)direction * 90.0f;
      currentArrow.transform.localRotation = Quaternion.Euler(rot);

      arrowSize = currentArrow.size;
      arrowSize.x = Vector2Int.Distance(optimizedPath[i - 1], optimizedPath[i]);
      currentArrow.size = arrowSize;

      center.x = optimizedPath[i - 1].x + ((optimizedPath[i].x - optimizedPath[i - 1].x) / 2.0f) + 0.5f;
      center.y = optimizedPath[i - 1].y + ((optimizedPath[i].y - optimizedPath[i - 1].y) / 2.0f) + 0.5f;
      currentArrow.transform.localPosition = new Vector3(center.x, 0.001f, center.y);
      currentArrowIndex++;
      if (currentArrowIndex >= arrows.Length) {
        Debug.LogWarning("arrows not enough");
        break;
      }
    }
  }

  public void HidePath() {
    DisableArrows();
  }

  private ArrowDirection GetArrowDirection(Vector2Int from, Vector2Int to) {
    if (from.x != to.x && from.y != to.y) {
      Debug.LogWarning("Failed to determine arrow direction");
      return ArrowDirection.Unknown;
    }

    if (from.x - to.x > 0) {
      return ArrowDirection.Left;
    }
    if (from.x - to.x < 0) {
      return ArrowDirection.Right;
    }
    if (from.y - to.y > 0) {
      return ArrowDirection.Down;
    }
    if (from.y - to.y < 0) {
      return ArrowDirection.Up;
    }

    return ArrowDirection.Unknown;
  }

  private List<Vector2Int> OptimizePath(Vector2Int[] path) {
    if (path == null || path.Length <= 1) {
      return null;
    }

    List<Vector2Int> result = new List<Vector2Int>();

    ArrowDirection lastDirection = ArrowDirection.Unknown;
    ArrowDirection direction = ArrowDirection.Unknown;

    for (int i = 1; i < path.Length; i++) {
      direction = GetArrowDirection(path[i - 1], path[i]);
      if (direction != lastDirection) {
        result.Add(path[i - 1]);
      }
      lastDirection = direction;
      if (i == path.Length - 1) {
        result.Add(path[i]);
      }
    }

    return result;
  }
}
