using System;

public class AStarPathNode : SettlersEngine.IPathNode<Object>
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
    public Boolean IsWall {get; set;}
    public bool IsWalkable(Object unused)
    {
        return !IsWall;
    }
}
