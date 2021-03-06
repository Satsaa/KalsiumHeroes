

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System.Linq;
using static Muc.Editor.GizmosUtil;

[RequireComponent(typeof(Animator))]
public partial class UnitActor : MonoBehaviour {

	public enum AnimationType {
		None,
		Walk,
		Run,
		Stagger,
		Die,
	}

	[HideInInspector] public AnimationType animationType;
	[HideInInspector] public Animator animator;
	public bool animating => animationType != 0;

	void OnDrawGizmos() {
		if (!Application.isPlaying) return;
		using (ColorScope(Color.blue)) Gizmos.DrawRay(new Ray(GetPos(), transform.forward));
		OnDrawGizmosMove();
	}

	protected void Awake() {
		if (!transform.parent) transform.parent = Game.game.transform;
		animator = GetComponent<Animator>();
	}

	protected void Update() {
		switch (animationType) {
			case AnimationType.None:
			case AnimationType.Die:
				break;
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
				SetPos(spline.controls.Last());
				moveT = spline.controls.Count - 1;
				isMoving = false;
				animator.SetTrigger("Idle");
				break;
		}
		animationType = AnimationType.None;
	}

	public Vector2 Get2DPos() => transform.localPosition.xz();
	public Vector3 GetPos() => transform.localPosition;
	public void Set2DPos(Vector2 localPosition) => transform.localPosition = localPosition.x0y().SetY(transform.localPosition.y);
	public void SetPos(Vector3 localPosition) => transform.localPosition = localPosition;

}
