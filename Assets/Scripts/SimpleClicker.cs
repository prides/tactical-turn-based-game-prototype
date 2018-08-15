using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClicker : MonoBehaviour {

  [SerializeField]
  private Camera currentCamera;
  private bool isHitGround = false;
  private Vector2Int lastPos = Vector2Int.zero;

  void Start() {
    if (currentCamera == null) {
      currentCamera = FindObjectOfType<Camera>();
    }
  }

  void Update() {
    Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
    RaycastHit rayHit;
    if (Physics.Raycast(ray, out rayHit)) {
      if (rayHit.collider.tag == "Ground") {
        isHitGround = true;
        Vector2Int point = new Vector2Int((int)rayHit.point.x, (int)rayHit.point.z);
        if (point != lastPos) {
          lastPos = point;
          PathDrawer.GetInstance().HidePath();
          LinkedList<AStarPathNode> path = AStarManager.GetInstance().Search(Converter.Convert<SettlersEngine.Point>(BattleManager.GetInstance().GetCurrentActor().GridTransform.Position), Converter.Convert<SettlersEngine.Point>(point));
          if (path != null && path.Count < BattleManager.GetInstance().GetCurrentActor().currentMovePoints) {
            Vector2Int[] pathVec = Converter.ConvertPath(path);
            PathDrawer.GetInstance().ShowPath(pathVec);
          }
        }
        if (Input.GetMouseButtonDown(0)) {
          BattleManager.GetInstance().GetCurrentActor().MoveTo(rayHit.point);
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
