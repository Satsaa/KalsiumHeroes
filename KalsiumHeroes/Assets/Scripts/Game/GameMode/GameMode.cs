using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a list of data sources
/// </summary>
public abstract class GameMode : ScriptableObject {

	public string title = "Untitled";
	public string version = "0.0.0";

	[SerializeField, Tooltip("Units that can be drafted in this GameMode.")]
	public List<UnitData> draftableUnits;
	[SerializeField, Tooltip("Teams available in this GameMode.")]
	public List<Team> teams = new List<Team>() { Team.Team1, Team.Team2 };

	public string[] draft;
	public Vector3Int[] draftPositions;
	public string[] altDraft;
	public Vector3Int[] altDraftPositions;

	public bool ValidateDraft(IEnumerable<string> draft, out TextSource failReason) => ValidateDraft(draft.Select(v => App.library.GetById<UnitData>(v)), out failReason);
	public abstract bool ValidateDraft(IEnumerable<UnitData> draftUnits, out TextSource failReason);

}

