using UnityEngine;

public enum TickMode {
	[Tooltip("Ticks at the start of the unit's turn.")]
	TurnStart,
	[Tooltip("Ticks at the end of the unit's turn.")]
	TurnEnd,
	[Tooltip("Ticks at the start of the unit's turn but is removed at the end of the turn.")]
	HybridTurn,
	[Tooltip("Ticks at the start of a round.")]
	RoundStart,
	[Tooltip("Ticks at custom times defined by the script using Tick().")]
	Custom,
}