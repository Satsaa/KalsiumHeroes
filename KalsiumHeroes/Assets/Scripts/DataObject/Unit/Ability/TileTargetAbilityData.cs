

using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileTargetAbilityData), menuName = "DataSources/" + nameof(TileTargetAbilityData))]
public class TileTargetAbilityData : TargetAbilityData {

	public override Type createTypeConstraint => typeof(TileTargetAbility);

	[Tooltip("Types of valid targets.")]
	public TargetType targetType;

	[Tooltip("Radius of the affected tiles around the target.")]
	public Radius radius;

	[Serializable]
	public class TargetType : Attribute<TileTargetType> {
		private TargetType() : base(TileTargetType.Any) { }
		public override string identifier => "Attribute_TileTargetAbility_TargetType";
		public override string TooltipText(IAttribute source) => current == TileTargetType.Any ? null : DefaultTooltip(source, Lang.GetStr("Targets"));
		public override string Format(bool isSource) {
			var str = current.value.ToString();
			return String.Join(", ", str
				.Split(new string[] { ", " }, 0)
				.Where(v => v != nameof(TileTargetType.Any))
				.Select(v => Lang.GetStr($"{identifier}_{v}"))
			);
		}
	}

	[Serializable]
	public class Radius : Attribute<int> {
		public override string identifier => "Attribute_TileTargetAbility_Radius";
		public override string TooltipText(IAttribute source) => current == 0 ? null : DefaultTooltip(source);
	}
}
