using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Contains a list of data sources
/// </summary>
public abstract class GameMode : StoredScriptableObject, IIDentifiable {

	public string identifier;
	string IIDentifiable.GetIdentifier() => identifier;
	public override string storageName => identifier;

	public string version = "0.0.0";

	[SerializeField, Tooltip("Teams available in this GameMode.")]
	public List<Team> teams = new List<Team>() { Team.Team1, Team.Team2 };

	[SerializeField, Tooltip("Units that can be drafted in this GameMode.")]
	public List<UnitData> draftableUnits;

	[JsonProperty] public string[] draft;
	[JsonProperty] public Vector3Int[] draftPositions;
	[JsonProperty] public string[] altDraft;
	[JsonProperty] public Vector3Int[] altDraftPositions;

	public bool ValidateDraft(IEnumerable<string> draft, out string failMessage) => ValidateDraft(draft.Select(v => App.library.GetById<UnitData>(v)), out failMessage);
	public abstract bool ValidateDraft(IEnumerable<UnitData> draftUnits, out string failMessage);


}

