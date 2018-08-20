using System;
using System.Collections.Generic;

public class Battle {
  public NextTurnStartedEventArgs : EventArgs {
    public Actor actor { get; set; }
  }
  public event EventHandler NextTurnStarted;

  private int id;

  private List<Actor> participants = new List<Actor>();

  private BattleQueue battleQueue;

  public Battle(int id, List<Actor> participants) {
    this.id = id;
    this.participants = participants;
    this.battleQueue = new BattleQueue(participants);
  }

  public Actor GetCurrentActor() {
    return battleQueue.GetCurrentActor();
  }

  public void NextTurn() {
    battleQueue.Dequeue();

    EventHandler handler = NextTurnStarted;
    if (handler != null) {
      NextTurnStartedEventArgs args = new NextTurnStartedEventArgs() {
        actor = GetCurrentActor();
      }
      handler(this, args);
    }
  }
}
