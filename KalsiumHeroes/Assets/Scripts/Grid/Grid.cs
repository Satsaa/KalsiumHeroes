using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Grid : MonoBehaviour {

  [SerializeField]
  public Vector2Int size { get; private set; }


  [SerializeField, HideInInspector]
  Dictionary<Vector3Int, Hex> _hexes;
  ReadOnlyDictionary<Vector3Int, Hex> hexes;

  void OnValidate() {
  }

  void Start() {

  }

  void Update() {

  }
}
