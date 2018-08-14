using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClicker : MonoBehaviour {

  [SerializeField]
  private Camera currentCamera;

  void Start() {
    if (currentCamera == null) {
      currentCamera = FindObjectOfType<Camera>();
    }
  }

  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
      RaycastHit rayHit;
      if (Physics.Raycast(ray, out rayHit)) {
        if (rayHit.collider.tag == "Ground") {
          BattleManager.GetInstance().GetCurrentActor().MoveTo(rayHit.point);
        }
      }
    }
  }
}
