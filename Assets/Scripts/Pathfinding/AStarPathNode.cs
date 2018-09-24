using System;

public class AStarPathNode : SettlersEngine.IPathNode<Object> {
  public Int32 X { get; set; }
  public Int32 Y { get; set; }
  public Boolean IsWall { get; set; }
  public bool IsWalkable(Object options) {
    if (options != null) {
      AStarManager.SearchOptions opt = (AStarManager.SearchOptions)options;
      foreach(SettlersEngine.Point point in opt.forceWalkable) {
        if (point.X == X && point.Y == Y) {
          return true;
        }
      }
    }
    return !IsWall;
  }
}
