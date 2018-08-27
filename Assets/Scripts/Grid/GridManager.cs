using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
  public enum Direction {
      Up,
      Right,
      Down,
      Left
  }

  private static Vector2Int GridMax = new Vector2Int(80, 80);

  private static GridManager instance;
  [SerializeField]
  private TileBase[] tileBases;

  private Tilemap tilemap;
  
  List<List<Vector2Int>> currentBattleGrid = new List<List<Vector2Int>>();

  [SerializeField]
  private GridAgent[,] globalTilesList;

  [SerializeField]
  private static Vector2Int tilesListSize = new Vector2Int(3, 3);
  [SerializeField]
  private LinkedList<LinkedList<GridAgent>> tilesList = new LinkedList<LinkedList<GridAgent>>();

  private AStarPathNode[,] currentGrid = new AStarPathNode[tilesListSize.x * GridAgent.TileSize.x, tilesListSize.y * GridAgent.TileSize.y];

  private bool isGridMoved = true;

  private void Awake () {
    if (instance == null) {
      instance = this;
    } else {
      Destroy(this);
      return;
    }
    tilemap = GetComponentInChildren<Tilemap>();
    if (tilemap == null) {
      Debug.LogError("tilemap is null");
    }
  }

  private void Start() {

    GridAgent[] gridAgents = FindObjectsOfType<GridAgent>();
    int size = (int)Mathf.Sqrt(gridAgents.Length);
    globalTilesList = new GridAgent[size, size];
    for (int i = 0; i < gridAgents.Length; i++) {
      globalTilesList[gridAgents[i].position.x, gridAgents[i].position.y] = gridAgents[i];
    }

    UpdateGrid();
    AStarManager.GetInstance().CreateNewSolver(currentGrid);
  }

  public static GridManager GetInstance() {
    return instance;
  }

  public void UpdateGrid() {
    if (!isGridMoved) {
      return;
    }
    if (tilesList.Count == 0) {
      for (int i = 0; i < tilesListSize.x; i++) {
        tilesList.AddLast(new LinkedList<GridAgent>());
        for (int j = 0; j < tilesListSize.y; j++) {
          tilesList.Last.Value.AddLast(globalTilesList[i, j]);
        }
      }
    }
    isGridMoved = false;
    int x = 0;
    int y = 0;
    foreach (LinkedList<GridAgent> column in tilesList) {
      if (column == null || column.Count == 0) {
        continue;
      }
      y = 0;
      foreach (GridAgent ga in column) {
        if (ga == null) {
          continue;
        }
        AStarPathNode[,] grid = ga.GetGrid();
        for (int i = 0; i < GridAgent.TileSize.x; i++) {
          for (int j = 0; j < GridAgent.TileSize.y; j++) {
            currentGrid[x * GridAgent.TileSize.x + i, y * GridAgent.TileSize.y + j] = grid[i, j];
          }
        }
        y++;
      }
      x++;
    }
  }

  public void MoveGrid(Direction direction) {
    Debug.Log("MoveGrid: direction:" + direction);
    switch (direction) {
      case Direction.Up: {
        int curx = 0;
        int newy = tilesList.First.Value.Last.Value.position.y + 1;
        foreach (LinkedList<GridAgent> column in tilesList) {
          curx = column.Last.Value.position.x;
          column.RemoveFirst();
          column.AddLast(globalTilesList[curx, newy]);
        }
      }
        break;
      case Direction.Right: {
        int newx = tilesList.Last.Value.Last.Value.position.x + 1;
        int cury = 0;
        LinkedList<GridAgent> column = tilesList.First.Value;
        cury = column.First.Value.position.y;
        column.Clear();
        tilesList.RemoveFirst();
        for (int y = 0; y < tilesListSize.y; y++) {
          column.AddLast(globalTilesList[newx, cury]);
          cury++;
        }
        tilesList.AddLast(column);
      }
        break;
      case Direction.Down: {
        int curx = 0;
        int newy = tilesList.First.Value.First.Value.position.y - 1;
        foreach (LinkedList<GridAgent> column in tilesList) {
          curx = column.Last.Value.position.x;
          column.RemoveLast();
          column.AddFirst(globalTilesList[curx, newy]);
        }
      }
        break;
      case Direction.Left: {
        int newx = tilesList.First.Value.First.Value.position.x - 1;
        int cury = 0;
        LinkedList<GridAgent> column = tilesList.Last.Value;
        cury = column.First.Value.position.y;
        column.Clear();
        tilesList.RemoveLast();
        for (int y = 0; y < tilesListSize.y; y++) {
          column.AddLast(globalTilesList[newx, cury]);
          cury++;
        }
        tilesList.AddFirst(column);
      }
        break;
      default:
        break;
    }
    isGridMoved = true;
    UpdateGrid();
  }

  public void ShowBattleGrid(int battleId) {
    if (globalTilesList == null) {
      return;
    }
    ClearBattleGrid();

    foreach (LinkedList<GridAgent> column in tilesList) {
      foreach (GridAgent ga in column) {
        if (ga.isDirty) {
          ga.GetGrid();
        }
      }
    }

    Actor currentActor = BattleManager.GetInstance().GetCurrentActor(battleId);
    if (currentActor == null) {
      Debug.LogError("There is no current actor in battle manager");
      return;
    }
    Vector2Int startPos = currentActor.GridTransform.Position;

    int drawRange = currentActor.Stat.MovePoints.Value;

    Vector2Int gridMin = new Vector2Int(currentGrid[0, 0].X, currentGrid[0, 0].Y);
    Vector2Int gridMax = new Vector2Int(currentGrid[0, 0].X + currentGrid.GetLength(0) - 1, currentGrid[0, 0].Y + currentGrid.GetLength(1) - 1);

    currentBattleGrid.Clear();

    AStarManager aStarManager = AStarManager.GetInstance();

    int minX = startPos.x - drawRange;
    if (minX < gridMin.x) {
      if (minX < 0) {
        minX = gridMin.x;
      } else {
        MoveGrid(Direction.Left);
        ShowBattleGrid(battleId);
        return;
      }
    }

    int minY = startPos.y - drawRange;
    if (minY < gridMin.y) {
      if (minY < 0) {
        minY = gridMin.y;
      } else {
        MoveGrid(Direction.Down);
        ShowBattleGrid(battleId);
        return;
      }
    }

    int maxX = startPos.x + drawRange;
    if (maxX > gridMax.x) {
      if (maxX > GridMax.x) {
        maxX = gridMax.x;
      } else {
        MoveGrid(Direction.Right);
        ShowBattleGrid(battleId);
        return;
      }
    }

    int maxY = startPos.y + drawRange;
    if (maxY > gridMax.y) {
      if (maxY > GridMax.y) {
        maxY = gridMax.y;
      } else {
        MoveGrid(Direction.Up);
        ShowBattleGrid(battleId);
        return;
      }
    }

    int index = 0;

    for (int x = minX; x <= maxX; x++) {
      currentBattleGrid.Add(new List<Vector2Int>());
      for (int y = minY; y <= maxY; y++) {
        LinkedList<AStarPathNode> path = aStarManager.Search(startPos, new Vector2Int(x, y));
        int tileType = 0;
        if (path == null) {
          tileType = 3;
        }
        if (path != null && path.Count - 1 <= drawRange) {
          tileType = ((float)(path.Count - 1) / (float)drawRange) < 0.7f ? 2 : 1;
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
