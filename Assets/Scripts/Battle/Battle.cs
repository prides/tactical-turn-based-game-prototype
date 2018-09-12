using System;
using System.Collections.Generic;

[System.Serializable]
public class Battle {
  public class NextTurnStartedEventArgs : EventArgs {
    public Actor actor { get; set; }
  }
  public event EventHandler NextTurnStarted;
  public event EventHandler OnOverEvent;

  private int id;
  public int ID {
    get { return id; }
  }

  private bool isOver = false;

  private BattleQueue battleQueue;

  public List<Actor> Participants {
    get { return battleQueue.Participants; }
  }

  public Battle(int id, List<Actor> participants) {
    this.id = id;
    this.battleQueue = new BattleQueue(participants);

    foreach (Actor actor in participants) {
      actor.OnDiedEvent += OnActorDied;
    }
  }

  private void OnActorDied(object sender, EventArgs e) {
    Actor actor = (Actor) sender;
    actor.OnDiedEvent -= OnActorDied;

    battleQueue.RemoveParticipant(actor);

    if (battleQueue.GroupsNumber <= 1) {
      Finish();
    }
  }

  public void Finish() {
    if (isOver) {
      return;
    }
    isOver = true;
    if (OnOverEvent != null) {
      OnOverEvent(this, EventArgs.Empty);
    }
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
