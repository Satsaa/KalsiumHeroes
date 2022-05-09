using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

	[Tooltip("Digits in order from smallest to largest")]
	public List<TextQueue> digits;

	[SerializeField, HideInInspector]
	public int[] time;

	private void Start() {
		time = new int[digits.Count];
		for (int i = 0; i < time.Length; i++) {
			time[i] = -1;
		}
	}

	public void SetTime(int seconds) {
		var time = GetIntParams(seconds);
		for (int i = 0; i < time.Length; i++) {
			var t1 = time[i];
			var t2 = this.time[i];
			this.time[i] = t1;
			if (t1 != t2) {
				digits[i].QueueText(t1.ToString());
			}
		}
	}

	public int[] GetIntParams(int seconds) {
		var t = TimeSpan.FromSeconds(seconds);

		int hours = t.Hours;
		int mins = t.Minutes;
		int secs = t.Seconds;

		var paramTime = new int[digits.Count];

		for (int i = 0; i < digits.Count; i++) {
			int digit = 0;
			digit = i switch {
				//s0
				0 => secs % 10,
				//s1
				1 => secs / 10,
				//m0
				2 => mins % 10,
				//m1
				3 => mins / 10,
				//h0
				4 => hours % 10,
				//h1+n
				_ => hours / (Mathf.FloorToInt(Mathf.Pow(10, i - 4)) * 10),
			};
			paramTime[i] = digit;
		}
		return paramTime;
	}


	void Update() {
		SetTime(Mathf.FloorToInt(Time.time));
	}

}

