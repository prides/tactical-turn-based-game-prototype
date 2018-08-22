using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class EffectBase {
  protected int duration = 0;
  protected int turn = 0;

  protected bool isActive = false;
  public bool IsActive {
    get { return isActive; }
  }

  protected Actor target;

  public event EventHandler OnStartEvent;
  public event EventHandler OnOverEvent;

  public void Start(Actor target) {
    if (isActive) {
      return;
    }
    isActive = true;
    this.target = target;
    InitialAction();
    if (turn == duration) {
      Stop();
    }
  }
  
  public void NextTurn() {
    if (!isActive) {
      return;
    }
    turn++;
    TurnAction();
    if (turn == duration) {
      Stop();
    }
  }

  public void Stop() {
    isActive = false;
    OverAction();
    if (OnOverEvent != null) {
      OnOverEvent(this, EventArgs.Empty);
    }
  }

  protected virtual void InitialAction() {}
  protected virtual void TurnAction() {}
  protected virtual void OverAction() {}
}

public class StatTemporaryChangeEffect<T> : EffectBase {
  protected T value;
  protected string statName;

  protected override void InitialAction() {
    PropertyInfo p = target.Stat.GetType().GetProperty(statName);
    MethodInfo m = p.PropertyType.GetMethod("AddValue");
    m.Invoke(p.GetValue(target.Stat, null), new object[] { value });
  }

  protected override void OverAction() {
    PropertyInfo p = target.Stat.GetType().GetProperty(statName);
    MethodInfo m = p.PropertyType.GetMethod("SubtractValue");
    m.Invoke(p.GetValue(target.Stat, null), new object[] { value });
  }
}

public interface IEffectReceivable {
  void ReceiveEffect(object sender, EffectBase effect);
  void RemoveEffect(object sender, EffectBase effect);
}

public interface ITurnListener {
  void OnNextTurn();
}

public class EffectApplyer : MonoBehaviour, IEffectReceivable, ITurnListener {
  [SerializeField]
  private Actor actor;

  [SerializeField]
  private List<EffectBase> applyedEffects = new List<EffectBase>();

  public void OnNextTurn() {
    foreach (EffectBase effect in applyedEffects) {
      effect.NextTurn();
    }
  }

  public void ReceiveEffect(object sender, EffectBase effect) {
    effect.OnOverEvent += OnEffectOver;
    effect.Start(actor);
    applyedEffects.Add(effect);
  }

  public void RemoveEffect(object sender, EffectBase effect) {
    if (!applyedEffects.Contains(effect)) {
      return;
    }
    effect.Stop();
  }

  public void OnEffectOver(object sender, EventArgs e) {
    EffectBase effect = (EffectBase)sender;
    effect.OnOverEvent -= OnEffectOver;
    applyedEffects.Remove(effect);
  }
}