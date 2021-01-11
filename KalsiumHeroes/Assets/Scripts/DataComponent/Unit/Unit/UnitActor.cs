

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System.Linq;

public partial class UnitActor : MonoBehaviour {

	[Obsolete("Use GetPos/SetPos for setting position.")]
	new Transform transform;

	public enum AnimationType {
		None,
		Walk,
		Run,
		Stagger,
		Die,
	}

	[HideInInspector] public AnimationType animationType;
	public bool animating => animationType != 0;

	protected void Update() {
		switch (animationType) {
			case AnimationType.None:
			case AnimationType.Die:
			case AnimationType.Stagger:
				break;
			case AnimationType.Walk:
			case AnimationType.Run:
				DoMovement();
				break;
		}
	}


	public void EndAnimations() {
		switch (animationType) {
			case AnimationType.None:
			case AnimationType.Die:
			case AnimationType.Stagger:
				break;
			case AnimationType.Run:
			case AnimationType.Walk:
				Set2DPos(spline._controls.Last().xz());
				moveT = spline._controls.Count - 1;
				isMoving = false;
				break;
		}
		animationType = AnimationType.None;
	}

	public Vector2 Get2DPos() => base.transform.parent.position.xz();
	public Vector3 GetPos() => base.transform.parent.position;
	public void Set2DPos(Vector2 position) => base.transform.parent.position = position.x0y().SetY(base.transform.parent.position.y);
	public void SetPos(Vector3 position) => base.transform.parent.position = position;

}
