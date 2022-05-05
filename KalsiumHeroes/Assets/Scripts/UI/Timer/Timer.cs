using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

	[Tooltip("Digits in order from smallest to largest")]
	public List<TextQueue> digits;

	[SerializeField]
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
			switch (i) {
				case 0: //s0
					digit = secs % 10;
					break;
				case 1: //s1
					digit = secs / 10;
					break;
				case 2: //m0
					digit = mins % 10;
					break;
				case 3: //m1
					digit = mins / 10;
					break;
				case 4: //h0
					digit = hours % 10;
					break;
				default: //h1+n
					digit = hours / (Mathf.FloorToInt(Mathf.Pow(10, i - 4)) * 10);
					break;
			}
			paramTime[i] = digit;
		}
		return paramTime;
	}


	void Update() {
		SetTime(Mathf.FloorToInt(Time.time));
	}

}

