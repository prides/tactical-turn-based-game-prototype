using UnityEngine;

public class GridTransform : MonoBehaviour {
  public Vector2Int Position {
    get { return new Vector2Int((int)transform.position.x, (int)transform.position.z); }
  }
}
