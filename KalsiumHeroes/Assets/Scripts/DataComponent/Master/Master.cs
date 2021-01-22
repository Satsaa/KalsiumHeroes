
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract class Master<TMod, TOnEvent> : Master where TMod : Modifier where TOnEvent : IOnEvent {

	public static Type modifierType => typeof(TMod);
	public static Type onEventType => typeof(TOnEvent);

	public ObjectDict<TMod> modifiers = new ObjectDict<TMod>();
	public OnEvents<TOnEvent> onEvents = new OnEvents<TOnEvent>();

	public override void OnCreate(Modifier modifier) {
		modifiers.Add<TMod>((TMod)modifier);
		onEvents.Add(modifier);
		onEvents.Add(this);
	}

	public override void OnRemove(Modifier modifier) {
		modifiers.Remove((TMod)modifier);
		onEvents.Remove(modifier);
		onEvents.Remove(this);
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
	public Transform transform => gameObject.transform;

	public void Remove() {
		if (removed) return;
		removed = true;

		Game.dataObjects.Remove(this);
		Game.onEvents.Remove(this);

		OnConfigureNonpersistent(false);
		OnRemove();
	}

	/// <summary> Creates a Master based on the given source. </summary>
	protected static T Create<T>(MasterData source, Action<T> initializer = null) where T : Master {
		var gameObject = source.container ? Instantiate(source.container) : new GameObject();
		var cont = gameObject.AddComponent<DataObjectContainer>();
		var master = (T)ScriptableObject.CreateInstance(source.createType);
		cont.dataObject = master;
		master.gameObject = gameObject;
		master._source = source;
		master._data = Instantiate(source);

		Game.dataObjects.Add(master);
		Game.onEvents.Add(master);

		initializer?.Invoke(master);

		master.OnConfigureNonpersistent(true);
		master.OnCreate();

		foreach (var baseModifier in source.baseModifiers.Where(v => v != null)) {
			Modifier.Create(master, baseModifier);
		}

		return master;
	}

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

		void OnEnable() {
		}

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