using System;
using UnityEngine;

[System.Serializable]
public class StatValueWithEvent<T> {
  public event EventHandler Changed;
  [SerializeField]
  private T total;
  public T Total {
    get { return total; }
    set {
      total = value;
      if (Changed != null) {
        Changed(this, EventArgs.Empty);
      }
    }
  }
  [SerializeField]
  private T value;
  public T Value {
    get { return value; }
    set {
      this.value = value;
      if (Changed != null) {
        Changed(this, EventArgs.Empty);
      }
    }
  }
}

[System.Serializable]
public class Statistics {

  [SerializeField]
  private StatValueWithEvent<int> health;
  public StatValueWithEvent<int> Health {
    get { return health; }
    set { health = value; }
  }

  [SerializeField]
  private StatValueWithEvent<int> movePoints;
  public StatValueWithEvent<int> MovePoints {
    get { return movePoints; }
    set { movePoints = value; }
  }

  [SerializeField]
  private int initiative;
  public int Initiative {
    get { return initiative; }
    set { initiative = value; }
  }
}
