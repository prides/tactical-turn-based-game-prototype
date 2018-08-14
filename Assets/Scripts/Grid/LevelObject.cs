using UnityEngine;

public class LevelObject : GridMonoBehaviour {

  public enum ObjectType {
    Obstacle
  }

  [SerializeField]
  private Vector2Int[] occupiedTilesPosition;

  [SerializeField]
  protected ObjectType type;
  public ObjectType Type {
    get { return type; }
  }

  public Vector2Int[] GetPosition() {
    Vector2Int[] result = occupiedTilesPosition;
    for (int i = 0; i < result.Length; i++) {
      result[i] += GridTransform.Position;
    }
    return occupiedTilesPosition;
  }
}
