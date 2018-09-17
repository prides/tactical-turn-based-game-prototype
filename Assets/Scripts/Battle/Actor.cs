using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType {
  Physical
}

public interface IDamagable {
  void ReceiveDamage(int damage, AttackType type);
}

public class Actor : GridMonoBehaviour, IDamagable {

  public event EventHandler OnDiedEvent;

  private Coroutine currentMoveEnumerator;

  public GridAgent currentTile;

  [SerializeField]
  private Statistics stat;
  public Statistics Stat {
    get { return stat; }
  }

  [SerializeField]
  private List<ITurnListener> turnListeners = new List<ITurnListener>();

  [SerializeField]
  private int currentBattleId = -1;

  [SerializeField]
  private int groupId = 0;
  public int GroupId {
    get { return groupId; }
    set { groupId = value; }
  }

  [SerializeField]
  private ActionType possibleActions = ActionType.None;

  public bool IsParticipateInBattle {
    get { return currentBattleId != -1; }
  }

  private void Awake() {
    Stat.Health.Changed += OnHealthValueChanged;
  }

  private void OnHealthValueChanged(object sender, IntStatValue.StatValueEventArgs e) {
    if (e.type == IntStatValue.ValueType.Current) {
      if (Stat.Health.Value <= 0) {
        if (OnDiedEvent != null) {
          OnDiedEvent(this, EventArgs.Empty);
        }
      }
    }
  }

  public void ReceiveDamage(int damage, AttackType attackType) {
    switch (attackType) {
      case AttackType.Physical: {
          int healthDamage = 0;
          if (Stat.PhysicalShield.Value < damage) {
            healthDamage = damage - Stat.PhysicalShield.Value;
          }
          Stat.PhysicalShield.Value -= damage;
          if (healthDamage != 0) {
            Stat.Health.Value -= healthDamage;
          }
        }
        break;
      default:
        Debug.LogWarning("Unknown attack type: " + attackType);
        break;
    }
  }

  public void StartTurn() {
    Stat.MovePoints.ApplyTotal();

    possibleActions = ActionType.Walk | ActionType.Melee;

    foreach (ITurnListener listener in turnListeners) {
      listener.OnNextTurn();
    }
  }

  public ActionInfo GetActionInfo(Vector2Int position) {
    if ((possibleActions & ActionType.Walk) == ActionType.Walk) {
      LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(GridTransform.Position, position);
      if (path == null || path.Count <= 1) {
        Debug.LogWarning("Couldn't find path to " + position.ToString());
        return null;
      }
      if (path.Count - 1 > Stat.MovePoints.Value) {
        Debug.LogWarning("Not enough move points");
        return null;
      }
      ActionInfo actionInfo = new ActionInfo();
      actionInfo.ActionList.Add(new MovingAction(path.Count - 1, path));
      return actionInfo;
    }
    Debug.LogWarning("Can't walk");
    return null;
  }

  public ActionInfo GetActionInfo(Actor selectedActor) {
    if (selectedActor == null) {
      Debug.LogError("Invalid param");
      return null;
    }
    LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(GridTransform.Position, selectedActor.GridTransform.Position);
    if (path == null || path.Count <= 1) {
      Debug.LogWarning("Couldn't find path to " + selectedActor.GridTransform.Position.ToString());
      return null;
    }
    if (path.Count - 1 + 2 <= Stat.MovePoints.Value) {
      Debug.LogWarning("Not enough move points");
      return null;
    }
    ActionInfo actionInfo = new ActionInfo();
    if (path.Count > 2) {
      if ((possibleActions & ActionType.Walk) == ActionType.Walk) {
        path.RemoveLast();
        actionInfo.ActionList.Add(new MovingAction(path.Count - 1, path));
        actionInfo.ActionList.Add(new AttackAction(2, ActionType.Melee, selectedActor));
        return actionInfo;
      }
    } else {
      actionInfo.ActionList.Add(new AttackAction(2, ActionType.Melee, selectedActor));
      return actionInfo;
    }
    return null;
  }

  public void Perform(ActionInfo actionInfo, Action<bool> actionOverCallback) {
    if (actionInfo == null) {
      Debug.LogError("Invalid param");
      actionOverCallback(false);
      return;
    }
    StartCoroutine(PerformCoroutine(actionInfo, actionOverCallback));
  }

  private IEnumerator PerformCoroutine(ActionInfo actionInfo, Action<bool> actionOverCallback) {
    foreach(ActionBase actionBase in actionInfo.ActionList) {
      switch(actionBase.Type) {
        case ActionType.Walk: {
          MovingAction ma = (MovingAction)actionBase;
          yield return StartCoroutine(StartMoveTo(ma.Path, delegate (bool result) {}));
        }
        break;
        case ActionType.Melee: {
          bool isOver = false;
          AttackAction aa = (AttackAction)actionBase;
          Attack(aa.Target, () => { isOver = true; });
          yield return new WaitUntil(() => isOver);
        }
        break;
        case ActionType.Distance: {

        }
        break;
      }
    }

    actionOverCallback(true);
  }

  public void Attack(Actor target, Action overCallback) {
    target.ReceiveDamage(25, AttackType.Physical);
    Stat.MovePoints.Value-= 2;
    overCallback();
  }

//TODO: need to move to another component
#region Moving logic
  public void MoveTo(Vector3 position, Action<bool> movingOverCallback) {
    SettlersEngine.Point startPoint = new SettlersEngine.Point() {
      X = (int)this.transform.position.x,
      Y = (int)this.transform.position.z
    };
    SettlersEngine.Point endPoint = new SettlersEngine.Point() {
      X = (int)position.x,
      Y = (int)position.z
    };
    IEnumerable<AStarPathNode> path = AStarManager.GetInstance().Search(startPoint, endPoint);
    if (path != null) {
      if (currentMoveEnumerator != null) {
        StopCoroutine(currentMoveEnumerator);
      }
      currentMoveEnumerator = StartCoroutine(StartMoveTo(path, movingOverCallback));
    } else {
      movingOverCallback(false);
    }
  }

  private IEnumerator StartMoveTo(IEnumerable<AStarPathNode> path, Action<bool> movingOverCallback) {
    foreach (AStarPathNode node in path) {
      Debug.Log("node.x:" + node.X + " node.y:" + node.Y);
      yield return StartCoroutine(MoveToNode(node));
      Stat.MovePoints.Value--;
    }
    currentTile.isDirty = true;
    movingOverCallback(true);
  }

  private IEnumerator MoveToNode(AStarPathNode node) {
    if (node.X != (int)transform.position.x
      || node.Y != (int)transform.position.y) {
      float t = 0.0f;
      while (true) {
        t = Mathf.Clamp01(t + Time.deltaTime);
        Vector3 pos = new Vector3((float)node.X + 0.5f, transform.position.y, (float)node.Y + 0.5f);
        transform.position = Vector3.Lerp(transform.position, pos, t);

        yield return new WaitForEndOfFrame();

        float diffX = Mathf.Abs((float)node.X + 0.5f - transform.position.x);
        float diffY = Mathf.Abs((float)node.Y + 0.5f - transform.position.z);
        if (diffX < 0.001f && diffY < 0.001f) {
          transform.position = new Vector3((float)node.X + 0.5f, transform.position.y, (float)node.Y + 0.5f);
          break;
        }
      }
    }
  }
#endregion

  public void JoinToBattle(Battle battle) {
    currentBattleId = battle.ID;
    battle.OnOverEvent += new EventHandler(delegate (object sender, EventArgs e) {
      currentBattleId = -1;
    });
  }
}
