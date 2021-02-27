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

	[Tooltip("Selected units for the GameMode")]
	public string[] draft;

	public abstract bool ValidateDraft(List<string> unitIds, out TextSource failReason);

}

