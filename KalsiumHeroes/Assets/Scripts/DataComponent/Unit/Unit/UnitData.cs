
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitData), menuName = "DataSources/" + nameof(UnitData))]
public class UnitData : MasterData {

	public override Type createTypeConstraint => typeof(Unit);

	[Tooltip("Display name of the Unit.")]
	public TextSource displayName;

	[Tooltip("Description of the Unit.")]
	public TextSource description;

	[Tooltip("Lore text of the Unit.")]
	public TextSource lore;

	[Tooltip("The default portrait sprite for this Unit.")]
	public Sprite portrait;

	[Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
	public Attribute<int> speed = new Attribute<int>(1);

	[Tooltip("Determines how many tiles any unit can move per turn.")]
	public Attribute<int> movement = new Attribute<int>(1);

	[AttributeLabels("Current", "Max")]
	[Tooltip("The health of the unit.")]
	public DualAttribute<float> health = new DualAttribute<float>(100, 100);

	[AttributeLabels("Current", "Max")]
	[Tooltip("Casting and extra movement costs energy.")]
	public DualAttribute<int> energy = new DualAttribute<int>(10, 20);

	[Tooltip("How much energy is regenerated per round.")]
	public Attribute<int> energyRegen = new Attribute<int>(5);

	[Tooltip("The amount of resistance to physical damage the unit posesses.")]
	public Attribute<int> defense;

	[Tooltip("The amount of resistance to magical damage the unit posesses.")]
	public Attribute<int> resistance;

	[Tooltip("Outgoing damage amplifications.")]
	public Amps amps;

	[Tooltip("Silenced units cannot cast spells."), HideInInspector]
	public SeededAttribute<bool> silenced = new SeededAttribute<bool>();

	[Tooltip("Disarmed units cannot cast weapon skills."), HideInInspector]
	public SeededAttribute<bool> disarmed = new SeededAttribute<bool>();

	[Tooltip("Rooted units cannot move."), HideInInspector]
	public SeededAttribute<bool> rooted = new SeededAttribute<bool>();

}