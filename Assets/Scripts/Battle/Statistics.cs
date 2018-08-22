using System;
using UnityEngine;

public abstract class StatValue<T> {
  public enum ValueType
  {
    Base,
    Additional,
    Current
  }

  public class StatValueEventArgs : EventArgs {
    public ValueType type;
    public T previousValue;
  }

  public event EventHandler Changed;

  [SerializeField]
  private T baseValue;
  public T BaseValue {
    get { return baseValue; }
    set {
      StatValueEventArgs eventArgs = new StatValueEventArgs() {
        type = ValueType.Base,
        previousValue = baseValue
      };
      baseValue = value;
      if (Changed != null) {
        Changed(this, eventArgs);
      }
    }
  }

  [SerializeField]
  private T additionalValue;
  public T AdditionalValue {
    get { return additionalValue; }
    set {
      StatValueEventArgs eventArgs = new StatValueEventArgs() {
        type = ValueType.Additional,
        previousValue = additionalValue,
      };
      additionalValue = value;
      if (Changed != null) {
        Changed(this, eventArgs);
      }
    }
  }

  [SerializeField]
  private T value;
  public T Value {
    get { return value; }
    set {
      StatValueEventArgs eventArgs = new StatValueEventArgs() {
        type = ValueType.Current,
        previousValue = this.value
      };
      this.value = value;
      if (Changed != null) {
        Changed(this, eventArgs);
      }
    }
  }

  public abstract void ApplyTotal();

  public abstract void ApplyBase();

  public abstract void AddValue(T value);

  public abstract void SubtractValue(T value);
}

[System.Serializable]
public class IntStatValue : StatValue<int>, IComparable<IntStatValue> {
  public override void ApplyTotal() {
    Value = BaseValue + AdditionalValue;
  }

  public override void ApplyBase() {
    Value = BaseValue;
  }

  public override void AddValue(int value) {
    AdditionalValue += value;
    Value += value;
  }

  public override void SubtractValue(int value) {
    AdditionalValue -= value;
    Value -= value;
  }

  public int CompareTo(IntStatValue obj) {
    if (Value > obj.Value) {
      return -1;
    } else if (Value == obj.Value) {
      return 0;
    } else {
      return 1;
    }
  }
}

[System.Serializable]
public class Statistics {
  #region Health
  [SerializeField]
  private IntStatValue health;
  public IntStatValue Health {
    get { return health; }
    set { health = value; }
  }

  [SerializeField]
  private IntStatValue magicShield;
  public IntStatValue MagicShield {
    get { return magicShield; }
    set { magicShield = value; }
  }

  [SerializeField]
  private IntStatValue physicalShield;
  public IntStatValue PhysicalShield {
    get { return physicalShield; }
    set { physicalShield = value; }
  }
  #endregion

  #region Resistance
  [SerializeField]
  private IntStatValue fireResist;
  public IntStatValue FireResist {
    get { return fireResist; }
    set { fireResist = value; }
  }
  [SerializeField]
  private IntStatValue waterResist;
  public IntStatValue WaterResist {
    get { return waterResist; }
    set { waterResist = value; }
  }
  [SerializeField]
  private IntStatValue poisonResist;
  public IntStatValue PoisonResist {
    get { return poisonResist; }
    set { poisonResist = value; }
  }
  [SerializeField]
  private IntStatValue windResist;
  public IntStatValue WindResist {
    get { return windResist; }
    set { windResist = value; }
  }
  #endregion

  [SerializeField]
  private IntStatValue movePoints;
  public IntStatValue MovePoints {
    get { return movePoints; }
    set { movePoints = value; }
  }

  [SerializeField]
  private IntStatValue initiative;
  public IntStatValue Initiative {
    get { return initiative; }
    set { initiative = value; }
  }
}
