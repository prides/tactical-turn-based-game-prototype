using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

  private static BattleManager instance;

  [SerializeField]
  private Dictionary<int, Battle> currentBattles = new Dictionary<int, Battle>();

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

  public int StartBattle(List<Actor> participants) {
    int battleId = Utils.Randomizer.Range(0, int.MaxValue);
    Battle battle = new Battle(battleId, participants);
    currentBattles.Add(battleId, battle);
    battle.OnOverEvent += OnBattleOver;
    foreach (Actor actor in participants) {
      actor.JoinToBattle(battle);
    }
    return battleId;
  }

  public void OnBattleOver(object sender, EventArgs e) {
    Battle battle = (Battle)sender;
    currentBattles.Remove(battle.ID);
  }

  public Battle GetBattle(int battleId) {
    Battle result;
    if (!currentBattles.TryGetValue(battleId, out result)) {
      Debug.LogWarning("Failed to get battle with id:" + battleId);
      return null;
    }
    return result;
  }

  public Actor GetCurrentActor(int battleId) {
    if (!currentBattles.ContainsKey(battleId)) {
      return null;
    }
    Battle battle = currentBattles[battleId];
    return battle.GetCurrentActor();
  }
}
