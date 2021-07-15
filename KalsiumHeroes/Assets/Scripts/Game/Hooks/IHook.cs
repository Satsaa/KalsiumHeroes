
#nullable enable annotations

public interface IHook { }

public interface IUnitHook : IHook { }
public interface ITileHook : IHook { }
public interface IEdgeHook : IHook { }
public interface IGlobalHook : IHook { }

public interface IOnUpdate : IGlobalHook { void OnUpdate(); }
public interface IOnLateUpdate : IGlobalHook { void OnLateUpdate(); }


public interface IOnModifierCreate : IGlobalHook { void OnModifierCreate(Modifier modifier); }
public interface IOnModifierRemove : IGlobalHook { void OnModifierRemove(Modifier modifier); }

public interface IOnUnitModifierCreate_Unit : IUnitHook { void OnUnitModifierCreate(UnitModifier modifier); }
public interface IOnUnitModifierCreate_Tile : ITileHook { void OnUnitModifierCreate(UnitModifier modifier); }
public interface IOnUnitModifierCreate_Global : IGlobalHook { void OnUnitModifierCreate(UnitModifier modifier); }

public interface IOnUnitModifierRemove_Unit : IUnitHook { void OnUnitModifierRemove(UnitModifier modifier); }
public interface IOnUnitModifierRemove_Tile : ITileHook { void OnUnitModifierRemove(UnitModifier modifier); }
public interface IOnUnitModifierRemove_Global : IGlobalHook { void OnUnitModifierRemove(UnitModifier modifier); }


public interface IOnTileModifierCreate_Tile : ITileHook { void OnTileModifierCreate(TileModifier modifier); }
public interface IOnTileModifierCreate_Global : IGlobalHook { void OnTileModifierCreate(TileModifier modifier); }

public interface IOnTileModifierRemove_Tile : ITileHook { void OnTileModifierRemove(TileModifier modifier); }
public interface IOnTileModifierRemove_Global : IGlobalHook { void OnTileModifierRemove(TileModifier modifier); }


public interface IOnEdgeModifierCreate_Edge : IEdgeHook { void OnEdgeModifierCreate(EdgeModifier modifier); }
public interface IOnEdgeModifierCreate_Global : IGlobalHook { void OnEdgeModifierCreate(EdgeModifier modifier); }

public interface IOnEdgeModifierRemove_Edge : IEdgeHook { void OnEdgeModifierRemove(EdgeModifier modifier); }
public interface IOnEdgeModifierRemove_Global : IGlobalHook { void OnEdgeModifierRemove(EdgeModifier modifier); }


public interface IOnAbilityCastStart_Unit : IUnitHook { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Tile : ITileHook { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Global : IGlobalHook { void OnAbilityCastStart(Ability ability); }

public interface IOnAbilityCastEnd_Unit : IUnitHook { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Tile : ITileHook { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Global : IGlobalHook { void OnAbilityCastEnd(Ability ability); }

public interface IOnDispell_Unit : IUnitHook { void OnDispell(); }
public interface IOnDispell_Tile : ITileHook { void OnDispell(Unit unit); }
public interface IOnDispell_Global : IGlobalHook { void OnDispell(Unit unit); }

public interface IOnEnergyDeficit_Unit : IUnitHook { void OnEnergyDeficit(int deficit); }
public interface IOnEnergyDeficit_Tile : ITileHook { void OnEnergyDeficit(Unit unit, int deficit); }
public interface IOnEnergyDeficit_Global : IGlobalHook { void OnEnergyDeficit(Unit unit, int deficit); }

public interface IOnEnergyExcess_Unit : IUnitHook { void OnEnergyExcess(int excess); }
public interface IOnEnergyExcess_Tile : ITileHook { void OnEnergyExcess(Unit unit, int excess); }
public interface IOnEnergyExcess_Global : IGlobalHook { void OnEnergyExcess(Unit unit, int excess); }

public interface IOnGetEstimatedSpeed_Unit : IUnitHook { void OnGetEstimatedSpeed(int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Tile : ITileHook { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Global : IGlobalHook { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }

public interface IOnHeal_Unit : IUnitHook { void OnHeal(ref float value); }
public interface IOnHeal_Tile : ITileHook { void OnHeal(Unit unit, ref float value); }
public interface IOnHeal_Global : IGlobalHook { void OnHeal(Unit unit, ref float value); }

public interface IOnTakeDamage_Unit : IUnitHook { void OnTakeDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Tile : ITileHook { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Global : IGlobalHook { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }

public interface IOnDealDamage_Unit : IUnitHook { void OnDealDamage(UnitModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Edge : IEdgeHook { void OnDealDamage(EdgeModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Tile : ITileHook { void OnDealDamage(TileModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Global : IGlobalHook { void OnDealDamage(Modifier source, Unit target, float damage, DamageType type); }

public interface IOnCalculateDamage_Unit : IUnitHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Tile : ITileHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Global : IGlobalHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }

public interface IOnSpawn_Unit : IUnitHook { void OnSpawn(); }
public interface IOnSpawn_Tile : ITileHook { void OnSpawn(Unit unit); }
public interface IOnSpawn_Global : IGlobalHook { void OnSpawn(Unit unit); }

public interface IOnDeath_Unit : IUnitHook { void OnDeath(); }
public interface IOnDeath_Tile : ITileHook { void OnDeath(Unit unit); }
public interface IOnDeath_Global : IGlobalHook { void OnDeath(Unit unit); }

public interface IOnTurnStart_Unit : IUnitHook { void OnTurnStart(); }
public interface IOnTurnStart_Tile : ITileHook { void OnTurnStart(Unit unit); }
public interface IOnTurnStart_Global : IGlobalHook { void OnTurnStart(Unit unit); }

public interface IOnTurnEnd_Unit : IUnitHook { void OnTurnEnd(); }
public interface IOnTurnEnd_Tile : ITileHook { void OnTurnEnd(Unit unit); }
public interface IOnTurnEnd_Global : IGlobalHook { void OnTurnEnd(Unit unit); }

public interface IOnRoundStart : IGlobalHook { void OnRoundStart(); }

public interface IOnTeamReady : IGlobalHook { void OnTeamReady(Team team); }

public interface IOnGameInit : IGlobalHook { void OnGameInit(); }
public interface IOnGameStart : IGlobalHook { void OnGameStart(); }
public interface IOnGameEnd : IGlobalHook { void OnGameEnd(); }

public interface IOnAnimationEventStart : IGlobalHook { void OnAnimationEventStart(EventHandler handler); }
public interface IOnAnimationEventEnd : IGlobalHook { void OnAnimationEventEnd(); }

public interface IOnTargeterStart : IGlobalHook { void OnTargeterStart(Targeter targeter); }
public interface IOnTargeterEnd : IGlobalHook { void OnTargeterEnd(); }

public interface IOnCombatLog : IGlobalHook { void OnCombatLog(string str); }

public interface IOnChangeDirection_Unit : IUnitHook { void OnChangeDirection(TileDir from, TileDir to); }
public interface IOnChangeDirection_Tile : ITileHook { void OnChangeDirection(Unit unit, TileDir from, TileDir to); }
public interface IOnChangeDirection_Global : IGlobalHook { void OnChangeDirection(Unit unit, TileDir from, TileDir to); }

public interface IOnChangePosition_Unit : IUnitHook { void OnChangePosition(Tile? from, Tile to); }
public interface IOnChangePosition_Tile : ITileHook { void OnChangePosition(Unit unit, Tile? from, Tile to); }
public interface IOnChangePosition_Global : IGlobalHook { void OnChangePosition(Unit unit, Tile? from, Tile to); }

public interface IOnMoveOn_Tile : ITileHook { void OnMoveOn(Modifier reason, Unit unit); }
public interface IOnMoveOn_Unit : IUnitHook { void OnMoveOn(Modifier reason, Tile tile); }
public interface IOnMoveOn_Global : IGlobalHook { void OnMoveOn(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOff_Tile : ITileHook { void OnMoveOff(Modifier reason, Unit unit); }
public interface IOnMoveOff_Unit : IUnitHook { void OnMoveOff(Modifier reason, Tile tile); }
public interface IOnMoveOff_Global : IGlobalHook { void OnMoveOff(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOver_Edge : IEdgeHook { void OnMoveOver(Modifier reason, Unit unit, Tile from, Tile to); }
public interface IOnMoveOver_Unit : IUnitHook { void OnMoveOver(Modifier reason, Tile from, Edge edge, Tile to); }
public interface IOnMoveOver_Global : IGlobalHook { void OnMoveOver(Modifier reason, Unit unit, Tile from, Edge edge, Tile to); }

public interface IOnGetCanPass_Edge : IEdgeHook {
	void OnGetCanPass(Tile from, Tile to, ref bool current);
	void OnGetCanPass(Unit unit, Tile from, Tile to, ref bool current);
}
public interface IOnGetCanPass_Unit : IUnitHook {
	void OnGetCanPass(Tile from, Edge edge, Tile to, ref bool current);
}
public interface IOnGetCanPass_Global : IGlobalHook {
	void OnGetCanPass(Tile from, Edge edge, Tile to, ref bool current);
	void OnGetCanPass(Unit unit, Tile from, Edge edge, Tile to, ref bool current);
}

public interface IOnGetMoveCost_Edge : IEdgeHook {
	void OnGetMoveCost(Tile from, Tile to, ref float current);
	void OnGetMoveCost(Unit unit, Tile from, Tile to, ref float current);
}
public interface IOnGetMoveCost_Unit : IUnitHook {
	void OnGetMoveCost(Tile from, Edge edge, Tile to, ref float current);
}
public interface IOnGetMoveCost_Global : IGlobalHook {
	void OnGetMoveCost(Tile from, Edge edge, Tile to, ref float current);
	void OnGetMoveCost(Unit unit, Tile from, Edge edge, Tile to, ref float current);
}

#nullable disable
