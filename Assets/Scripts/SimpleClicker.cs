using System.Collections;
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

  void Start() {
    if (currentCamera == null) {
      currentCamera = FindObjectOfType<Camera>();
    }
    currentBattleId = BattleManager.GetInstance().StartBattle(participants);
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
          LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(Converter.Convert<SettlersEngine.Point>(currentActor.GridTransform.Position), Converter.Convert<SettlersEngine.Point>(point));
          if (path != null && path.Count < currentActor.Stat.Move) {
            Vector2Int[] pathVec = Converter.ConvertPath(path);
            PathDrawer.GetInstance().ShowPath(pathVec);
          }
        }
        if (Input.GetMouseButtonDown(0)) {
          currentActor.MoveTo(rayHit.point, delegate (bool result) {
            isHitGround = false;
            lastPos = Vector2Int.zero;
            BattleManager.GetInstance().GetBattle(currentBattleId).NextTurn();
            GridManager.GetInstance().ShowBattleGrid(currentBattleId);
          });
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
