
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

[RequireComponent(typeof(EventStack<>))]
public class StackItem : EventStackItem {

	[HideInInspector] public EventStack es;
	[HideInInspector] public RoundStack rs;

	public override float width => es != null ? es.MaxX() : rs.MaxX();


	new protected void Awake() {
		base.Awake();
		es = GetComponent<EventStack>();
		rs = GetComponent<RoundStack>();
	}

}