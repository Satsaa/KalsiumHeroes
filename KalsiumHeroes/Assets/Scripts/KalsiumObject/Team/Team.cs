
using UnityEngine;

[CreateAssetMenu(fileName = nameof(Team), menuName = "KalsiumHeroes/Master/" + nameof(Team))]
public class Team : Master<Team, TeamModifier> {

	public Color color = Color.magenta;

}