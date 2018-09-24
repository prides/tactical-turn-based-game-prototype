using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour {
  public class AStarSolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
  {
    protected override Double Heuristic(PathNode inStart, PathNode inEnd) {
      return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
    }

    protected override Double NeighborDistance(PathNode inStart, PathNode inEnd) {
      int diffX = Math.Abs(inStart.X - inEnd.X);
      int diffY = Math.Abs(inStart.Y - inEnd.Y);

      switch (diffX + diffY) {
        case 1: return 1;
        case 2: return int.MaxValue;
        case 0: return 0;
        default:
          throw new ApplicationException();
      }
    }

    public AStarSolver(TPathNode[,] inGrid)
        : base(inGrid)
    {
    }
  }

  public class SearchOptions {
    public List<SettlersEngine.Point> forceWalkable = new List<SettlersEngine.Point>();
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

  public LinkedList<AStarPathNode> Search(SettlersEngine.Point startPoint, SettlersEngine.Point endPoint) {
    if (currentSolver == null) {
      return null;
    } else {
      return currentSolver.Search(startPoint, endPoint, null);
    }
  }

  public LinkedList<AStarPathNode> Search(Vector2Int startPoint, Vector2Int endPoint) {
    return Search(Converter.Convert<SettlersEngine.Point>(startPoint), Converter.Convert<SettlersEngine.Point>(endPoint));
  }

  public LinkedList<AStarPathNode> SearchClosest(SettlersEngine.Point startPoint, SettlersEngine.Point endPoint) {
    if (currentSolver == null) {
      return null;
    } else {
      SearchOptions opt = new SearchOptions();
      opt.forceWalkable.Add(endPoint);
      LinkedList<AStarPathNode> result = currentSolver.Search(startPoint, endPoint, opt);
      if (result != null && result.Count > 0) {
        result.RemoveLast();
      }
      return result;
    }
  }

  public LinkedList<AStarPathNode> SearchClosest(Vector2Int startPoint, Vector2Int endPoint) {
    return SearchClosest(Converter.Convert<SettlersEngine.Point>(startPoint), Converter.Convert<SettlersEngine.Point>(endPoint));
  }
}
