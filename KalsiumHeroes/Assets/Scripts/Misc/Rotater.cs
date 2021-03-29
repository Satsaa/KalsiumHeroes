using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

public class Rotater : MonoBehaviour {

	public float maxRotation;
	Vector3 rotation;

	Vector3 rng1;
	Vector3 rng2;
	Vector3 rng3;
	Vector3 rng4;
	Vector3 rng5;

	// Update is called once per frame
	void Update() {
		rng5 = rng4;
		rng4 = rng3;
		rng3 = rng2;
		rng1 = new Vector3(
			Random.Range(-250f, 250f),
			Random.Range(-250f, 250f),
			Random.Range(-250f, 250f)
		);
		var rng = (rng1 + rng2 + rng3 + rng4 + rng5) / 5;
		rotation += rng * Time.deltaTime;
		rotation = rotation.Clamp(-maxRotation, maxRotation);
		transform.Rotate(rotation * Time.deltaTime);
	}
}
