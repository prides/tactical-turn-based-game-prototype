using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClicker : MonoBehaviour {

  [SerializeField]
  private Camera currentCamera;
  private bool isHitGround = false;
  private Vector2Int lastPos = Vector2Int.zero;

  private int currentBattleId;

  [SerializeField]
  [Header("Debug only")]
  private List<Actor> participants = new List<Actor>();

  private void Start() {
    if (currentCamera == null) {
      currentCamera = FindObjectOfType<Camera>();
    }
    currentBattleId = BattleManager.GetInstance().StartBattle(participants);
    BattleManager.GetInstance().GetBattle(currentBattleId).NextTurnStarted += OnNextMove;
    GridManager.GetInstance().ShowBattleGrid(currentBattleId);

    foreach (Actor actor in participants) {
      actor.OnDiedEvent += OnActorDied;
    }
  }

  private void OnNextMove(object sender, EventArgs e) {
    BattleManager.GetInstance().GetCurrentActor(currentBattleId).StartTurn();
  }

  private void OnActorDied(object sender, EventArgs e) {
    Actor actor = (Actor)sender;
    actor.OnDiedEvent -= OnActorDied;
    Destroy(actor.gameObject);
  }

  void Update() {
    Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
    RaycastHit rayHit;
    if (Physics.Raycast(ray, out rayHit)) {
      if (rayHit.collider.tag == "Ground") {
        isHitGround = true;
        Vector2Int point = new Vector2Int((int)rayHit.point.x, (int)rayHit.point.z);
        Actor currentActor = BattleManager.GetInstance().GetCurrentActor(currentBattleId);
        if (point != lastPos) {
          lastPos = point;
          PathDrawer.GetInstance().HidePath();
          LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(currentActor.GridTransform.Position, point);
          if (path != null && path.Count - 1 <= currentActor.Stat.MovePoints.Value) {
            Vector2Int[] pathVec = Converter.ConvertPath(path);
            PathDrawer.GetInstance().ShowPath(pathVec);
          }
        }
        if (Input.GetMouseButtonDown(0)) {
          currentActor.MoveTo(rayHit.point, delegate (bool result) {
            isHitGround = false;
            lastPos = Vector2Int.zero;
            if (currentActor.Stat.MovePoints.Value <= 0) {
              BattleManager.GetInstance().GetBattle(currentBattleId).NextTurn();
            }
            GridManager.GetInstance().ShowBattleGrid(currentBattleId);
          });
        }
      } else if (rayHit.collider.tag == "Actor") {
        if (Input.GetMouseButtonDown(0)) {
          Actor currentActor = BattleManager.GetInstance().GetCurrentActor(currentBattleId);
          if (currentActor.Stat.MovePoints.Value > 0) {
            Actor selectedActor = rayHit.collider.GetComponentInParent<Actor>();
            if (selectedActor != null && currentActor != selectedActor) {
              LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(currentActor.GridTransform.Position, selectedActor.GridTransform.Position);
              if (path != null && path.Count != 0) {
                if (path.Count == 1) {
                  if (currentActor.Stat.MovePoints.Value >= 2) {
                    selectedActor.ReceiveDamage(25, AttackType.Physical);
                    currentActor.Stat.MovePoints.Value-= 2;
                    if (currentActor.Stat.MovePoints.Value<= 0) {
                      BattleManager.GetInstance().GetBattle(currentBattleId).NextTurn();
                    }
                    GridManager.GetInstance().ShowBattleGrid(currentBattleId);
                  }
                } else {
                  if (path.Count - 1 < currentActor.Stat.MovePoints.Value) {
                    path.RemoveLast();
                    AStarPathNode node = path.Last.Value;
                    currentActor.MoveTo(new Vector3(node.X, 0, node.Y), delegate(bool result) {
                      if (currentActor.Stat.MovePoints.Value>= 2) {
                        selectedActor.ReceiveDamage(25, AttackType.Physical);
                        currentActor.Stat.MovePoints.Value-= 2;
                        if (currentActor.Stat.MovePoints.Value<= 0) {
                          BattleManager.GetInstance().GetBattle(currentBattleId).NextTurn();
                        }
                        GridManager.GetInstance().ShowBattleGrid(currentBattleId);
                      }
                    });
                  }
                }
              }
            }
          }
        }
      } else {
        if (isHitGround) {
          isHitGround = false;
          lastPos = Vector2Int.zero;
          PathDrawer.GetInstance().HidePath();
        }
      }
    }
  }
}
