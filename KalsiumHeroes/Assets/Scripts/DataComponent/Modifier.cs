#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;


/// <summary>
/// DataComponent which receives a bunch of global events in form of overrideable functions.
/// </summary>
public abstract class Modifier : DataComponent {

	public ModifierData modifierData => (ModifierData)data;
	public override Type dataType => typeof(ModifierData);

	protected new void Awake() {
		base.Awake();
		OnConfigureNonpersistent(true);
	}

	protected new void OnDestroy() {
		OnConfigureNonpersistent(false);
		base.OnDestroy();
		DestroyContainer();
	}

	private void DestroyContainer() {
		if (modifierData && modifierData.container != null) {
			ObjectUtil.Destroy(gameObject);
		}
	}

	/// <summary> Gets a MasterComponent of type T from the GameObject or it's parent. </summary>
	protected T GetMasterComponent<T>() where T : MasterComponent {
		if (TryGetComponent<T>(out var res)) return res;
		if (transform.parent != null && transform.parent.TryGetComponent<T>(out res)) return res;
		return null;
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<UnitModifier>()) {
				mod.OnConfigureNonpersistent(true);
			}
		}
	}
#endif

	/// <summary>
	/// Modifier is instantiated or the scripts are reloaded.
	/// Also when the Modifier is removed but with add = false.
	/// Conditionally add or remove non-persistent things here.
	/// </summary>
	protected virtual void OnConfigureNonpersistent(bool add) { }

	/// <summary> When a round starts. </summary>
	public virtual void OnRoundStart() { }

	/// <summary> When any Unit's turn starts. </summary>
	public virtual void OnTurnStart(Unit unit) { }

	/// <summary> When any Unit spawns. </summary>
	public virtual void OnSpawn(Unit unit) { }

	/// <summary> When any Unit dies. </summary>
	public virtual void OnDeath(Unit unit) { }

	/// <summary> When the game starts. </summary>
	public virtual void OnGameStart() { }

	/// <summary> When the game ends. </summary>
	public virtual void OnGameEnd() { }

	/// <summary> When an event handler starts (animations). </summary>
	public virtual void OnEventStart() { }

	/// <summary> When an event handler finishes (animations). </summary>
	public virtual void OnEventEnd() { }
}
