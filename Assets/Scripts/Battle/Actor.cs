using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
  Physical
}

public interface IDamagable
{
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
    switch (attackType)
    {
      case AttackType.Physical:
        {
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

    foreach (ITurnListener listener in turnListeners) {
      listener.OnNextTurn();
    }
  }

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

  IEnumerator StartMoveTo(IEnumerable<AStarPathNode> path, Action<bool> movingOverCallback) {
    foreach (AStarPathNode node in path) {
      Debug.Log("node.x:" + node.X + " node.y:" + node.Y);
      yield return StartCoroutine(MoveToNode(node));
      Stat.MovePoints.Value--;
    }
    currentTile.isDirty = true;
    movingOverCallback(true);
  }

  IEnumerator MoveToNode(AStarPathNode node) {
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
}
