using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAgent : MonoBehaviour {

  public int sizeX;
  public int sizeY;
  private AStarPathNode[,] grid;

  private void Start() {
    AStarManager.GetInstance().CreateNewSolver(GetGrid());
  }

  public AStarPathNode[,] GetGrid() {
    if (grid == null) {
      grid = new AStarPathNode[sizeX, sizeY];
    }
    for (int x = 0; x < sizeX; x++) {
      for (int y = 0; y < sizeY; y++) {
        grid[x, y] = new AStarPathNode() {
          IsWall = false,
          X = x,
          Y = y,
        };
      }
    }
    return grid;
  }

}
