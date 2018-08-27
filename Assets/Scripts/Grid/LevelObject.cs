using UnityEngine;

public class LevelObject : GridMonoBehaviour {

  public enum ObjectType {
    Obstacle,
    Actor
  }

  [SerializeField]
  private Vector2Int[] occupiedTilesPosition;

  [SerializeField]
  protected ObjectType type;
  public ObjectType Type {
    get { return type; }
  }

  public Vector2Int[] GetLocalOccupiedPositions() {
    Vector2Int[] result = occupiedTilesPosition.Clone() as Vector2Int[];
    for (int i = 0; i < result.Length; i++) {
      result[i] += GridTransform.LocalPosition;
    }
    return result;
  }
}
