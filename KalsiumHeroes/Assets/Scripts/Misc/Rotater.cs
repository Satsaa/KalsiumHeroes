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
	Vector3 rng6;
	Vector3 rng7;
	Vector3 rng8;
	Vector3 rng9;
	Vector3 rng10;

	// Update is called once per frame
	void Update() {
		rng10 = rng9;
		rng9 = rng8;
		rng8 = rng7;
		rng7 = rng6;
		rng6 = rng5;
		rng5 = rng4;
		rng4 = rng3;
		rng3 = rng2;
		rng1 = new Vector3(
			Random.Range(-250f, 250f),
			Random.Range(-250f, 250f),
			Random.Range(-250f, 250f)
		);
		var rng = (rng1 + rng2 + rng3 + rng4 + rng5 + rng6 + rng7 + rng8 + rng9 + rng10) / 10;
		rotation += rng * Time.deltaTime;
		rotation = rotation.Clamp(-maxRotation, maxRotation);
		transform.Rotate(rotation.Pow(1) * Time.deltaTime);
	}
}
