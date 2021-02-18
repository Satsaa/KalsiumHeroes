

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(-601)]
public class UnitSpawner : MonoBehaviour {

	public UnitData source;
	public Team team;

	void Start() {
		Unit.Create(source, transform.position, team);
		Destroy(gameObject);
	}
}