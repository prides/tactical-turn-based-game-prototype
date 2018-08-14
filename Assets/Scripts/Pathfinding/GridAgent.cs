using UnityEngine;

public class GridAgent : MonoBehaviour {

  public int sizeX;
  public int sizeY;
  private AStarPathNode[,] grid;
  public bool isDirty = true;

  private void Start() {
    AStarManager.GetInstance().CreateNewSolver(GetGrid());
  }

  public AStarPathNode[,] GetGrid() {
    if (grid == null) {
      grid = new AStarPathNode[sizeX, sizeY];
    }
    if (isDirty) {

      isDirty = false;
      LevelObject[] babies = GetComponentsInChildren<LevelObject>();

      for (int x = 0; x < sizeX; x++) {
        for (int y = 0; y < sizeY; y++) {
          grid[x, y] = null;
        }
      }

      foreach (LevelObject baby in babies) {

        if (baby.Type == LevelObject.ObjectType.Obstacle) {

          Vector2Int[] positions = baby.GetPosition();
          foreach (Vector2Int pos in positions) {
            grid[pos.x, pos.y] = new AStarPathNode() {
              IsWall = true,
              X = pos.x,
              Y = pos.y
            };
          }
        }
      }

      for (int x = 0; x < sizeX; x++) {
        for (int y = 0; y < sizeY; y++) {

          if (grid[x, y] != null) {
            continue;
          }

          grid[x, y] = new AStarPathNode() {
            IsWall = false,
            X = x,
            Y = y,
          };
        }
      }
    }
    return grid;
  }

}
