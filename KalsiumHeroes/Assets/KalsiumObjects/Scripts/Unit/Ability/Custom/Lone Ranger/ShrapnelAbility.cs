using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ShrapnelAbility), menuName = "KalsiumHeroes/Ability/" + nameof(ShrapnelAbility))]
public class ShrapnelAbility : TileTargetAbility {

	public Attribute<float> damage;
	public DamageType damageType;

	[Tooltip("The modifier given to the affected units.")]
	public ShrapnelModifier shrapnelModifier;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]];
			var modifier = (ShrapnelModifier)UnitModifier.Create(master, shrapnelModifier);

			modifier.target = target;
			modifier.creator = this;
			modifier.aoe = base.GetAffectedArea(target).ToList();

			modifier.calculatedDamage = damage.current;
			modifier.damageType = damageType;

			CalculateDamage(ref modifier.calculatedDamage, ref modifier.damageType);
		});
	}

}
