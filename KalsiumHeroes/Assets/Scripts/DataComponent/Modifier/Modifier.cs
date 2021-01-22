
using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// Base class for all Modifiers of Masters.
/// </summary>
public abstract class Modifier : DataObject {

	public new ModifierData source => (ModifierData)_source;
	public new ModifierData data => (ModifierData)_data;
	public override Type dataType => typeof(ModifierData);

	[Tooltip("Master component for this Modifier."), SerializeField]
	Master _master;
	public Master master => _master;

	/// <summary> Optional GameObject created for this Modifier </summary>
	[field: SerializeField]
	public GameObject container { get; private set; }

	/// <summary> Removes this Modifier </summary>
	public void Remove() {
		if (removed) return;
		removed = true;

		master.OnRemove(this);
		Game.dataObjects.Remove(this);
		Game.onEvents.Remove(this);

		OnConfigureNonpersistent(false);
		OnRemove();
		if (container) ObjectUtil.Destroy(container);
	}

	/// <summary> Creates a Modifier based on the given source and attaches it to the master. </summary>
	public static Modifier Create(Master master, ModifierData source, Action<Modifier> initializer = null) {
		return Create<Modifier>(master, source, initializer);
	}
	/// <summary> Creates a Modifier based on the given source and attaches it to the master. </summary>
	public static T Create<T>(Master master, ModifierData source, Action<T> initializer = null) where T : Modifier {
		var modifier = (T)CreateInstance(source.createType);
		modifier._master = master;
		modifier._source = source;
		modifier._data = Instantiate(source);
		if (source.container) {
			Canvas canvas;
			// Create containers containing RectTransforms on the Canvas of the Master.
			if (source.container.GetComponent<RectTransform>() && (canvas = master.gameObject.GetComponentInChildren<Canvas>())) {
				modifier.container = Instantiate(source.container, canvas.transform);
			} else {
				modifier.container = Instantiate(source.container, master.gameObject.transform);
			}
			modifier.container.transform.rotation = source.container.transform.localRotation;
		}

		master.OnCreate(modifier);
		Game.dataObjects.Add(modifier);
		Game.onEvents.Add(modifier);

		initializer?.Invoke(modifier);

		modifier.OnConfigureNonpersistent(true);
		modifier.OnCreate();

		return modifier;
	}

	protected virtual void DealDamage(Unit unit, float damage, DamageType type) {
		unit.DealCalculatedDamage(this, damage, type);
	}

}
