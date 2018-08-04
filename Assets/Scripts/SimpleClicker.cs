using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClicker : MonoBehaviour {

  private Coroutine currentMoveEnumerator;
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
          MoveTo(rayHit.point);
        }
      }
    }
  }

  private void MoveTo(Vector3 position) {
    SettlersEngine.Point startPoint = new SettlersEngine.Point() {
      X = (int)this.transform.position.x,
      Y = (int)this.transform.position.z
    };
    SettlersEngine.Point endPoint = new SettlersEngine.Point() {
      X = (int)position.x,
      Y = (int)position.z
    };
    IEnumerable<AStarPathNode> path = AStarManager.GetInstance().Search(startPoint, endPoint);
    if (currentMoveEnumerator != null) {
      StopCoroutine(currentMoveEnumerator);
    }
    currentMoveEnumerator = StartCoroutine(StartMoveTo(path));
  }

  IEnumerator StartMoveTo(IEnumerable<AStarPathNode> path) {
    foreach (AStarPathNode node in path) {
      Debug.Log("node.x:" + node.X + " node.y:" + node.Y);
      yield return StartCoroutine(MoveToNode(node));
    }
  }

  IEnumerator MoveToNode(AStarPathNode node) {
    if (node.X != (int)transform.position.x
      || node.Y != (int)transform.position.y) {
      float t = 0.0f;
      while (true) {
        t = Mathf.Clamp01(t + Time.deltaTime);
        Vector3 pos = new Vector3((float)node.X + 0.5f, transform.position.y, (float)node.Y + 0.5f);
        transform.position = Vector3.Lerp(transform.position, pos, t);

        yield return new WaitForEndOfFrame();

        float diffX = Mathf.Abs((float)node.X + 0.5f - transform.position.x);
        float diffY = Mathf.Abs((float)node.Y + 0.5f - transform.position.z);
        if (diffX < 0.001f && diffY < 0.001f) {
          transform.position = new Vector3((float)node.X + 0.5f, transform.position.y, (float)node.Y + 0.5f);
          break;
        }
      }
    }
  }
}
