using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour {
  public class AStarSolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
  {
    protected override Double Heuristic(PathNode inStart, PathNode inEnd)
    {
        return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
    }

    protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
    {
        return Heuristic(inStart, inEnd);
    }

    public AStarSolver(TPathNode[,] inGrid)
        : base(inGrid)
    {
    }
  }

  private static AStarManager instance;

  private AStarSolver<AStarPathNode, object> currentSolver;

  public static AStarManager GetInstance() {
    return instance;
  }

  private void Awake() {
    if (instance == null) {
      instance = this;
    } else {
      GameObject.Destroy(this.gameObject);
    }
  }

  public void CreateNewSolver(AStarPathNode[,] grid) {
    currentSolver = new AStarSolver<AStarPathNode, object>(grid);
  }

  public IEnumerable<AStarPathNode> Search(SettlersEngine.Point startPoint, SettlersEngine.Point endPoint) {
    if (currentSolver == null) {
      return null;
    } else {
      return currentSolver.Search(startPoint, endPoint, null);
    }
  }
}
