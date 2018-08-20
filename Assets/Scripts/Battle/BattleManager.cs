using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

  private static BattleManager instance;

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
    return battleId;
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
    Battle battle = currentBattles[battleId];
    return battle.GetCurrentActor();
  }
}
