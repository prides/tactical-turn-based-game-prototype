using UnityEngine;

public class GridAgent : MonoBehaviour {
  public static Vector2Int TileSize = new Vector2Int(16, 16);

  public Vector2Int position;

  public int sizeX;
  public int sizeY;
  private AStarPathNode[,] grid;
  public bool isDirty = true;

  private void Start() {
  }

  private void CreateGrid() {
    if (grid != null) {
      return;
    }
    grid = new AStarPathNode[sizeX, sizeY];
    for (int x = 0; x < sizeX; x++) {
      for (int y = 0; y < sizeY; y++) {
        grid[x, y] = new AStarPathNode() {
          IsWall = false,
          X = (position.x * TileSize.x) + x,
          Y = (position.y * TileSize.y) + y,
        };
      }
    }
  }

  public AStarPathNode[,] GetGrid() {
    if (grid == null) {
      CreateGrid();
    }
    if (isDirty) {

      isDirty = false;
      LevelObject[] babies = GetComponentsInChildren<LevelObject>();

      for (int x = 0; x < sizeX; x++) {
        for (int y = 0; y < sizeY; y++) {
          grid[x, y].IsWall = false;
        }
      }

      foreach (LevelObject baby in babies) {
        if (baby.Type == LevelObject.ObjectType.Obstacle ||
            baby.Type == LevelObject.ObjectType.Actor) {

          Vector2Int[] positions = baby.GetLocalOccupiedPositions();
          foreach (Vector2Int pos in positions) {
            grid[pos.x, pos.y].IsWall = true;
          }
        }
      }
    }
    return grid;
  }

}
