
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract class Master<TMod, TModData, THook> : Master where TMod : Modifier where TModData : ModifierData where THook : IHook {

	public static Type modifierType => typeof(TMod);
	public static Type hookType => typeof(THook);
	public static Type modifierDataType => typeof(TModData);

	public ObjectDict<TMod> modifiers = new ObjectDict<TMod>();
	public Hooks<THook> hooks = new Hooks<THook>();
	public override Hooks rawHooks => hooks;

	protected override void OnCreate() {
		hooks.Hook(this);
	}

	protected override void OnRemove() {
		hooks.Unhook(this);
		foreach (var modifier in modifiers.ToList()) {
			modifier.Remove();
		}
	}

	public sealed override void AttachModifier(Modifier modifier) {
		modifiers.Add<TMod>((TMod)modifier);
		hooks.Hook(modifier);
	}

	public sealed override void DetachModifier(Modifier modifier) {
		modifiers.Remove((TMod)modifier);
		hooks.Unhook(modifier);
	}

	public override List<Modifier> GetRawModifiers() {
		return modifiers.Get().Cast<Modifier>().ToList();
	}
}

public abstract class Master : DataObject {

	public new MasterData data => (MasterData)_data;
	public override Type dataType => typeof(MasterData);

	/// <summary> Created GameObject for this Master. <summary>
	[field: SerializeField]
	public GameObject gameObject { get; private set; }
	public Transform transform => gameObject ? gameObject.transform : null;

	public abstract Hooks rawHooks { get; }

	public void Remove() {
		if (removed) return;
		removed = true;

		Game.dataObjects.Remove(this);
		Game.hooks.Unhook(this);

		OnConfigureNonpersistent(false);
		OnRemove();
		if (gameObject) {
			ObjectUtil.Destroy(gameObject);
			gameObject = null;
		}
	}

	/// <summary> Creates a Master based on the given source. </summary>
	protected static T Create<T>(MasterData source, Action<T> initializer = null) where T : Master {
		var master = (T)ScriptableObject.CreateInstance(source.createType);
		master._source = source;
		master._data = Instantiate(source);

		Game.dataObjects.Add(master);
		Game.hooks.Hook(master);

		initializer?.Invoke(master);

		master.OnConfigureNonpersistent(true);
		master.OnCreate();
		master.Show();

		foreach (var baseModifier in source.baseModifiers.Where(v => v != null)) {
			Modifier.Create(master, baseModifier);
		}

		return master;
	}

	protected override void OnShow() {
		base.OnShow();
		if (shown) {
			var gameObject = data.container.value ? ObjectUtil.Instantiate(data.container.value, Game.game.transform) : new GameObject();
			this.gameObject = gameObject;
		}
	}

	protected override void OnHide() {
		if (gameObject) {
			ObjectUtil.Destroy(gameObject);
			gameObject = null;
		}
		base.OnHide();
	}

	public abstract void AttachModifier(Modifier modifier);

	public abstract void DetachModifier(Modifier modifier);

	public abstract List<Modifier> GetRawModifiers();

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	// [CanEditMultipleObjects]
	[CustomEditor(typeof(Master), true)]
	public class MasterEditor : Editor {

		Master t => (Master)target;

		List<bool> expands = new List<bool>();

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			var i = 0;
			foreach (var mod in t.GetRawModifiers()) {
				if (expands.Count <= i) expands.Add(false);
				if (expands[i] = EditorGUILayout.Foldout(expands[i], mod.GetType().Name, true)) {
					using (IndentScope()) {
						using (var serializedObject = new SerializedObject(mod)) {
							var prop = serializedObject.GetIterator();
							while (prop.NextVisible(true)) {
								if (prop.name != "m_Script") {
									EditorGUILayout.PropertyField(prop, false);
								}
							}
							EditorGUILayout.Space();
						}
					}
				}
				i++;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif