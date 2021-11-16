using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;

/// <summary>
/// Contains a list of data sources
/// </summary>
[KeepRefToken]
public abstract class GameMode : StoredScriptableObject, IIdentifiable {

	public string identifier;
	string IIdentifiable.identifier => identifier;
	public override string storageName => identifier;

	public string version = "0.0.0";

	[Tooltip("Teams available in this GameMode.")]
	public List<Team> teams;

	[Tooltip("Units that can be drafted in this GameMode.")]
	public List<Unit> draftableUnits;

	[JsonProperty] public string[] draft;
	[JsonProperty] public Vector3Int[] draftPositions;
	[JsonProperty] public string[] altDraft;
	[JsonProperty] public Vector3Int[] altDraftPositions;

	public bool ValidateDraft(IEnumerable<string> draft, out string failMessage) => ValidateDraft(draft.Select(v => App.library.GetById<Unit>(v)), out failMessage);
	public abstract bool ValidateDraft(IEnumerable<Unit> draftUnits, out string failMessage);


}

