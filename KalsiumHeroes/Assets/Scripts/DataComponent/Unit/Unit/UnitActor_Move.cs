

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using Muc.Geometry;
using System.Linq;

public partial class UnitActor {


	public float walkSpeed = 1f;
	public float runSpeed = 1.75f;

	[HideInInspector, SerializeField] bool isMoving;
	[HideInInspector, SerializeField] public float moveT;
	[HideInInspector, SerializeField] public Spline spline;


	public void Walk(IEnumerable<Vector3> positions) {
		moveT = 0;
		spline = new Spline(positions);
		animationType = AnimationType.Walk;
		isMoving = true;
		Set2DPos(positions.First());
	}

	public void Run(IEnumerable<Vector3> positions) {
		moveT = 0;
		spline = new Spline(positions);
		animationType = AnimationType.Run;
		isMoving = true;
		Set2DPos(positions.First());
	}

	private void DoMovement() {
		moveT += walkSpeed * Time.deltaTime;
		var point = spline.Eval(moveT);
		var diff = point - GetPos();
		Set2DPos(point.xz());
		base.transform.parent.LookAt(GetPos() + diff);
		if (moveT >= spline._controls.Count - 1) {
			EndAnimations();
		}
	}

	private void OnDrawGizmos() {
		if (spline._controls.Count < 2) return;
		var points = spline.RenderSpline(8);

		int i = 0;
		var prev = Vector3.zero;
		foreach (var point in points) {
			if (i > 0) {
				Gizmos.DrawLine(prev, point);
			}
			prev = point;
			i++;
		}
	}

}
