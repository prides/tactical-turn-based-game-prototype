using System;
using System.Collections.Generic;

public enum ActionType {
  None = 0,
  Walk = 1,
  Melee = 1 << 1,
  Distance = 1 << 2,
}

public abstract class ActionBase {
  protected int cost;
  public int Cost {
    get { return cost; }
  }

  protected ActionType type;
  public ActionType Type {
    get { return type; }
  }
}

public class MovingAction : ActionBase {
  protected LinkedList<AStarPathNode> path;
  public LinkedList<AStarPathNode> Path {
    get { return path; }
  }

  public MovingAction(int cost, LinkedList<AStarPathNode> path) {
    this.cost = cost;
    this.type = ActionType.Walk;
    this.path = path;
  }
}

public class AttackAction : ActionBase {
  protected Actor target;
  public Actor Target {
    get { return target; }
  }

  public AttackAction(int cost, ActionType type, Actor target) {
    this.cost = cost;
    this.type = type;
    this.target = target;
  }
}

[System.Serializable]
public class ActionInfo {
  public List<ActionBase> ActionList = new List<ActionBase>();
}