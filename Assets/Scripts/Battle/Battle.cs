using System;
using System.Collections.Generic;

[System.Serializable]
public class Battle {
  public class NextTurnStartedEventArgs : EventArgs {
    public Actor actor { get; set; }
  }
  public event EventHandler NextTurnStarted;

  private int id;

  private BattleQueue battleQueue;

  public Battle(int id, List<Actor> participants) {
    this.id = id;
    this.battleQueue = new BattleQueue(participants);
  }

  public Actor GetCurrentActor() {
    return battleQueue.GetCurrentActor();
  }

  public void NextTurn() {
    battleQueue.Dequeue();

    if (NextTurnStarted != null) {
      NextTurnStartedEventArgs args = new NextTurnStartedEventArgs() {
        actor = GetCurrentActor()
      };
      NextTurnStarted(this, args);
    }
  }
}
