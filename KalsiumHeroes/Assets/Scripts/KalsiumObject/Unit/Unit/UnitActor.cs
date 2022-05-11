

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System.Linq;
using static Muc.Editor.GizmosUtil;


public partial class UnitActor : Actor {

	public enum AnimationType {
		None,
		Move,
		Run,
		Stagger,
		Die,
	}

	[HideInInspector] public AnimationType animationType;
	public bool animating => animationType != 0;

	void OnDrawGizmos() {
		if (!Application.isPlaying) return;
		using (ColorScope(Color.blue)) Gizmos.DrawRay(new(GetPos(), transform.forward));
		OnDrawGizmosMove();
	}

	protected void Update() {
		switch (animationType) {
			case AnimationType.None:
			case AnimationType.Die:
				break;
			case AnimationType.Stagger:
				break;
			case AnimationType.Move:
			case AnimationType.Run:
				DoMovement();
				break;
		}
	}


	public override void EndAnimations() {
		switch (animationType) {
			case AnimationType.None:
			case AnimationType.Die:
			case AnimationType.Stagger:
				break;
			case AnimationType.Run:
			case AnimationType.Move:
				SetPos(spline.controls.Last());
				moveT = spline.controls.Count - 1;
				isMoving = false;
				animator.ResetTrigger("Walk");
				animator.SetTrigger("Idle");
				break;
			default:
				break;
		}
		animationType = AnimationType.None;
	}

	Vector2 Get2DPos() => transform.localPosition.xz();
	Vector3 GetPos() => transform.localPosition;
	void Set2DPos(Vector2 localPosition) => transform.localPosition = localPosition.x0y().SetY(transform.localPosition.y);
	void SetPos(Vector3 localPosition) => transform.localPosition = localPosition;

}
