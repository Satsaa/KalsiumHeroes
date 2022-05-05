using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public abstract class TextQueueText : TMPro.TextMeshProUGUI {

	/// <summary> When the object is shown </summary>
	public abstract void OnShow(string text);

	/// <summary> When the object is being faded in </summary>
	/// <param name="t">Delta of fade in duration</param>
	/// <param name="p">Position value from curve</param>
	/// <param name="a">Alpha value from curve</param>
	public abstract void OnFadeIn(float t, Vector3 p, float a);

	/// <summary> When the object stops to linger </summary>
	public abstract void OnLingerStart(bool instantFadeOut);

	/// <summary> When the object is lingering </summary>
	/// <param name="t">Delta of linger duration</param>
	public abstract void OnLinger(float t);

	/// <summary> When the object starts to fade out </summary>
	public abstract void OnFadeOutStart();

	/// <summary> When the object is being faded out </summary>
	/// <param name="t">Delta of fade out duration</param>
	/// <param name="p">Position value from curve</param>
	/// <param name="a">Alpha value from curve</param>
	public abstract void OnFadeOut(float t, Vector3 p, float a);

	/// <summary> When the object is hidden </summary>
	/// <param name="pooled">Whether the QueueText was returned to a pool</param>
	public abstract void OnHide(bool pooled);
}
