
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Muc.Editor;
using Serialization;
using UnityEngine;

[RefToken]
public abstract class DataObject : ScriptableObject, IIdentifiable {

	[Tooltip("String identifier of this DataObject. (\"Unit_Oracle\")")]
	public string identifier;
	string IIdentifiable.identifier => identifier;

	private static Regex removeData = new(@"Data$");
	private string _tooltip;
	public string tooltip {
		get {
			if (_tooltip != null) return _tooltip;
			var identifierTooltip = $"{identifier}_Info";
			if (Tooltips.instance.TooltipExists(identifierTooltip)) {
				return _tooltip = identifierTooltip;
			}
			var current = this.GetType();
			while (true) {
				var converted = current.FullName;
				converted = removeData.Replace(converted, "");
				converted += "_Info";
				if (Tooltips.instance.TooltipExists(converted) || current == typeof(DataObject)) {
					return _tooltip = converted;
				}
				current = current.BaseType;
			}
		}
	}

	[field: Tooltip("Source instance."), SerializeField]
	public DataObject source { get; protected set; }
	public bool isSource => source == this;

	[field: SerializeField]
	public bool removed { get; protected set; }

	[field: SerializeField, DoNotTokenize]
	public bool shown { get; set; }

	protected virtual void OnCreate() { }
	protected virtual void OnRemove() { }

	public void Show() {
		if (removed || shown == (shown = true)) return;
		OnShow();
	}
	public void Hide() {
		if (shown == (shown = false)) return;
		OnHide();
	}

	/// <summary> Create all GameObject related stuff here </summary>
	protected virtual void OnShow() { }
	/// <summary> Remove all GameObject related stuff here </summary>
	protected virtual void OnHide() { }

	/// <summary>
	/// Modifier is created or the scripts are reloaded.
	/// Also when the Modifier is removed but with add = false.
	/// Conditionally add or remove non-persistent things here.
	/// </summary>
	/// <param name="add"></param>
	protected virtual void OnConfigureNonpersistent(bool add) { }

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying && Game.game) {
			foreach (var dobj in Game.dataObjects.Get<DataObject>().Where(v => v)) {
				dobj.OnConfigureNonpersistent(true);
			}
		}
	}
#endif

}