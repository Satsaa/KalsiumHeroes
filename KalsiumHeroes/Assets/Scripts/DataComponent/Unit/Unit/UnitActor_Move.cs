

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

public partial class UnitActor {


	public float walkSpeed = 1f;
	public float runSpeed = 1.75f;

	public float walkSpeedUpDuration = 0.25f;
	public float runSpeedUpDuration = 0.25f;
	public AnimationCurve speedUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

	public float walkSlowDownDistance = 0.1f;
	public float runSlowDownDistance = 0.1f;
	public AnimationCurve slowDownCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

	[HideInInspector, SerializeField] bool isMoving;
	[HideInInspector, SerializeField] float speed;
	[HideInInspector, SerializeField] Vector2 moveTarget;
	[HideInInspector, SerializeField] float moveTime;
	[HideInInspector, SerializeField] bool doStop;
	private enum MoveStage { SpeedUp, Stable, SlowDown }
	[HideInInspector, SerializeField] MoveStage moveStage;
	[HideInInspector, SerializeField] float slowDownStartTime;
	[HideInInspector, SerializeField] Vector2 slowDownStartPosition;
	[HideInInspector, SerializeField] float slowDownDuration;


	public void WalkTo(Vector3 position, bool doStop) => WalkTo(position.xz(), doStop);
	public void WalkTo(Vector2 position, bool doStop) {
		if (!isMoving) {
			EndAnimations();
			ResetMovementVars();
		}
		isMoving = true;
		this.doStop = doStop;
		animationType = AnimationType.Walk;
		moveTarget = position;
	}

	public void RunTo(Vector3 position, bool doStop) => RunTo(position.xz(), doStop);
	public void RunTo(Vector2 position, bool doStop) {
		if (!isMoving) {
			EndAnimations();
			ResetMovementVars();
		}
		isMoving = true;
		this.doStop = doStop;
		animationType = AnimationType.Run;
		moveTarget = position;
	}

	private void ResetMovementVars() {
		moveTime = Time.time;
		moveStage = MoveStage.SpeedUp;
		speed = 0;
	}

	private void DoMovement() {
		if (doStop && moveStage != MoveStage.SlowDown) {
			var slowDownDistance = animationType == AnimationType.Walk ? walkSlowDownDistance : runSlowDownDistance;
			var distance = Vector2.Distance(Get2DPos(), moveTarget);
			if (distance <= slowDownDistance) {
				slowDownStartTime = Time.time;
				slowDownStartPosition = Get2DPos();
				slowDownDuration = Vector2.Distance(slowDownStartPosition, moveTarget) / speed;
				moveStage = MoveStage.SlowDown;
			}
		}
		switch (moveStage) {
			case MoveStage.SpeedUp: {
					var targetSpeed = animationType == AnimationType.Walk ? walkSpeed : runSpeed;
					var speedUpDuration = animationType == AnimationType.Walk ? walkSpeedUpDuration : runSpeedUpDuration;

					var timePassed = Time.time - moveTime;
					var timeFract = timePassed / speedUpDuration;

					if (timeFract >= 1) {
						speed = targetSpeed;
						moveStage = MoveStage.Stable;
					} else {
						speed = speedUpCurve.Evaluate(timeFract);
					}
					Set2DPos(Vector2.MoveTowards(Get2DPos(), moveTarget, speed * Time.deltaTime));
				}
				break;

			case MoveStage.Stable: {
					Set2DPos(Vector2.MoveTowards(Get2DPos(), moveTarget, speed * Time.deltaTime));
					if (Get2DPos() == moveTarget) {
						if (doStop) EndAnimations();
						else animationType = AnimationType.None; // Without isMoving = false
					}
				}
				break;

			case MoveStage.SlowDown: {
					var slowDownDistance = animationType == AnimationType.Walk ? walkSlowDownDistance : runSlowDownDistance;

					var timePassed = Time.time - slowDownStartTime;
					var timeFract = timePassed / slowDownDuration;

					var lerpPos = Vector2.Lerp(slowDownStartPosition, moveTarget, 1 - slowDownCurve.Evaluate(timeFract));
					Set2DPos(lerpPos);
					if (timeFract >= 1) {
						if (doStop) EndAnimations();
						else animationType = AnimationType.None; // Without isMoving = false
					}
				}
				break;
		}
	}

}
