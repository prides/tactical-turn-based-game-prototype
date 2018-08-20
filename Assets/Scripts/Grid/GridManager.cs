using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {

  private static GridManager instance;
  [SerializeField]
  private TileBase[] tileBases;

  private Tilemap tilemap;

  List<List<Vector2Int>> currentBattleGrid = new List<List<Vector2Int>>();

  void Awake () {
    if (instance == null) {
      instance = this;
    } else {
      Destroy(this);
    }
  }

  private void Start() {
    tilemap = GetComponentInChildren<Tilemap>();
    if (tilemap == null) {
      Debug.LogError("tilemap is null");
    }
  }

  public static GridManager GetInstance() {
    return instance;
  }

  public void ShowBattleGrid(int battleId) {
    ClearBattleGrid();

    Actor currentActor = BattleManager.GetInstance().GetCurrentActor(battleId);
    if (currentActor == null) {
      Debug.LogError("There is no current actor in battle manager");
      return;
    }
    Vector2Int startPos = currentActor.GridTransform.Position;

    int drawRange = currentActor.currentMovePoints;

    GridAgent tile = currentActor.currentTile;
    if (tile == null) {
      Debug.LogError("tile is null");
      return;
    }

    AStarPathNode[,] grid = tile.GetGrid();
    Vector2Int gridMin = new Vector2Int(grid[0, 0].X, grid[0, 0].Y);
    Vector2Int gridMax = new Vector2Int(grid[0, 0].X + tile.sizeX - 1, grid[0, 0].Y + tile.sizeY - 1);

    currentBattleGrid.Clear();

    AStarManager aStarManager = AStarManager.GetInstance();
    aStarManager.CreateNewSolver(grid);

    int minX = startPos.x - drawRange;
    minX = gridMin.x > minX ? gridMin.x : minX;
    int minY = startPos.y - drawRange;
    minY = gridMin.y > minY ? gridMin.y : minY;

    int maxX = startPos.x + drawRange;
    maxX = gridMax.x < maxX ? gridMax.x : maxX;
    int maxY = startPos.y + drawRange;
    maxY = gridMax.y < maxY ? gridMax.y : maxY;

    int index = 0;

    for (int x = minX; x <= maxX; x++) {
      currentBattleGrid.Add(new List<Vector2Int>());
      for (int y = minY; y <= maxY; y++) {
        LinkedList<AStarPathNode> path = aStarManager.Search(Converter.Convert<SettlersEngine.Point>(startPos), new SettlersEngine.Point() { X = x, Y = y });
        int tileType = 0;
        if (path == null) {
          tileType = 3;
        }
        if (path != null && path.Count < drawRange) {
          tileType = ((float)path.Count / (float)drawRange) < 0.7f ? 2 : 1;
        }
        tilemap.SetTile(new Vector3Int(x, y, 0), tileBases[tileType]);
        currentBattleGrid[index].Add(new Vector2Int(x, y));
      }
      index++;
    }
  }

  private void ClearBattleGrid() {
    if (currentBattleGrid == null) {
      return;
    }
    foreach (List<Vector2Int> row in currentBattleGrid) {
      foreach (Vector2Int cell in row) {
        tilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), tileBases[0]);
      }
    }
  }
}
