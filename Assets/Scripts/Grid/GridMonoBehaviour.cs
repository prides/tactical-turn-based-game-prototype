using UnityEngine;

public class GridMonoBehaviour : MonoBehaviour {
  private GridTransform gridTransform;
  public GridTransform GridTransform {
    get {
      if (gridTransform == null) {
        gridTransform = GetComponent<GridTransform>();
        if (gridTransform == null) {
          Debug.LogWarning("There is no GridTransform component in " + gameObject.name + "\n GridTransform was created");
          gridTransform = gameObject.AddComponent<GridTransform>();
        }
      }
      return gridTransform;
    }
  }
}
