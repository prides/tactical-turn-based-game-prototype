using System;
using System.Linq;
using System.Collections.Generic;

public class BattleQueue {
  private static const int QUEUE_LENGTH = 3;

  public class Cycle {
    private Queue<Actor> queue = new Queue<Actor>();

    public Cycle(List<Actor> participants) {
      GenerateQueue(participants);
    }

    public Regenerate(List<Actor> participants) {
      queue.Clear();
      GenerateQueue(participants);
    }

    private void GenerateQueue(List<Actor> participants) {
      List<Actor> ordered = participants.OrderByDescending(a => a.Stat.Initiative).ToList();
      foreach (Actor a in ordered) {
        queue.Enqueue(a);
      }
    }

    public Actor Peek() {
      return queue.Peek();
    }
  }

  private Queue<Cycle> queue = new Queue<Cycle>();

  private List<Actor> participants;

  public BattleQueue(List<Actor> participants) {
    this.participants = participants;

    for (int i = 0; i < QUEUE_LENGTH; i++) {
      queue.Enqueue(new Cycle(participants));
    }
  }

  public Actor GetCurrentActor() {
    return queue.Peek().Peek();
  }

  public void Dequeue() {
    Cycle currentCycle = queue.Peek();
    Actor currentActor = currentCycle.Dequeue();
    if (currentCycle.Count == 0) {
      queue.Dequeue();
      queue.Enqueue(participants);
    }
  }
}