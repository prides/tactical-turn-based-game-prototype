using System;
using System.Linq;
using System.Collections.Generic;

public class BattleQueue {
  private const int QUEUE_LENGTH = 3;

  public class Cycle {
    public Queue<Actor> queue = new Queue<Actor>();

    public int Count { get { return queue.Count; } }

    public Cycle(List<Actor> participants) {
      GenerateQueue(participants);
    }

    public void Regenerate(List<Actor> participants) {
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

    public Actor Dequeue() {
      return queue.Dequeue();
    }
  }

  private Queue<Cycle> queue = new Queue<Cycle>();

  private List<Actor> participants;
  public List<Actor> Participants {
    get { return participants; }
  }

  public int GroupsNumber {
    get {
      //TODO: need to optimize
      Dictionary<int, bool> counter = new Dictionary<int, bool>();
      foreach (Actor actor in participants) {
        counter[actor.GroupId] = true;
      }
      return counter.Count;
    }
  }

  public BattleQueue(List<Actor> participants) {
    this.participants = participants;

    for (int i = 0; i < QUEUE_LENGTH; i++) {
      queue.Enqueue(new Cycle(participants));
    }
  }

  public void RemoveParticipant(Actor actor) {
    participants.Remove(actor);
    UpdateQueue();
  }

  private void UpdateQueue() {
    foreach (Cycle cycle in queue) {
      cycle.Regenerate(this.participants);
    }
  }

  public Actor GetCurrentActor() {
    return queue.Peek().Peek();
  }

  public Actor Dequeue() {
    Cycle currentCycle = queue.Peek();
    Actor currentActor = currentCycle.Dequeue();
    if (currentCycle.Count == 0) {
      queue.Dequeue();
      queue.Enqueue(new Cycle(participants));
    }
    return currentActor;
  }

  public IEnumerable<Actor> GetEnumerable() {
    foreach (Cycle c in queue) {
      foreach (Actor a in c.queue) {
        yield return a;
      }
    }
  }
}