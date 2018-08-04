using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

  private static GridManager instance;

  void Awake () {
    if (instance == null) {
      instance = this;
    } else {
      Destroy(this);
    }
  }

  public GridManager GetInstance() {
    return instance;
  }


}
