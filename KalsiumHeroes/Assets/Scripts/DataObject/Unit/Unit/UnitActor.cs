

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System.Linq;
using static Muc.Editor.GizmosUtil;

public partial class UnitActor : MonoBehaviour {

	public enum AnimationType {
		None,
		Walk,
		Run,
		Stagger,
		Die,
	}

	[HideInInspector] public AnimationType animationType;
	public bool animating => animationType != 0;

	void OnDrawGizmos() {
		if (!Application.isPlaying) return;
		using (ColorScope(Color.blue)) Gizmos.DrawRay(new Ray(GetPos(), transform.forward));
		OnDrawGizmosMove();
	}

	protected void Awake() {
		transform.parent = Game.instance.transform;
	}

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
				SetPos(spline._controls.Last());
				moveT = spline._controls.Count - 1;
				isMoving = false;
				break;
		}
		animationType = AnimationType.None;
	}

	public Vector2 Get2DPos() => transform.position.xz();
	public Vector3 GetPos() => transform.position;
	public void Set2DPos(Vector2 position) => transform.position = position.x0y().SetY(transform.position.y);
	public void SetPos(Vector3 position) => transform.position = position;

}
