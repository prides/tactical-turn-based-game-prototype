using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

  private static BattleManager instance;

  [SerializeField]
  private Actor currentActor;

  private void Awake() {
    if (instance == null) {
      instance = this;
    } else {
      Destroy(this);
    }
  }

  public static BattleManager GetInstance() {
    return instance;
  }

  public Actor GetCurrentActor() {
    return currentActor;
  }
}
