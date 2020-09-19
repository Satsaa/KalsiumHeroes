using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Grid : MonoBehaviour {

  [SerializeField]
  public Vector2Int size { get; private set; }


  [SerializeField, HideInInspector]
  Hex[,] _hexes;
  ReadOnlyCollection<ReadOnlyCollection<Hex>> hexes;

  void OnValidate() {
    _hexes = new Hex[size.x, size.y];
    for (int x = 0; x < size.x; x++) {
      for (int y = 0; y < size.y; y++) {
        _hexes[x, y] = new Hex(x, y);
      }
    }
  }

  void Start() {

  }

  void Update() {

  }
}
