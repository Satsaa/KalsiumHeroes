
#nullable enable annotations

public interface IHook { }

public interface IUnitHook : IHook { }
public interface ITileHook : IHook { }
public interface IEdgeHook : IHook { }
public interface IGameHook : IHook { }

public interface IOnUpdate : IGameHook { void OnUpdate(); }
public interface IOnLateUpdate : IGameHook { void OnLateUpdate(); }


public interface IOnModifierCreate : IGameHook { void OnModifierCreate(Modifier modifier); }
public interface IOnModifierRemove : IGameHook { void OnModifierRemove(Modifier modifier); }

public interface IOnUnitModifierCreate_Unit : IUnitHook { void OnUnitModifierCreate(UnitModifier modifier); }
public interface IOnUnitModifierCreate_Tile : ITileHook { void OnUnitModifierCreate(UnitModifier modifier); }
public interface IOnUnitModifierCreate_Game : IGameHook { void OnUnitModifierCreate(UnitModifier modifier); }

public interface IOnUnitModifierRemove_Unit : IUnitHook { void OnUnitModifierRemove(UnitModifier modifier); }
public interface IOnUnitModifierRemove_Tile : ITileHook { void OnUnitModifierRemove(UnitModifier modifier); }
public interface IOnUnitModifierRemove_Game : IGameHook { void OnUnitModifierRemove(UnitModifier modifier); }


public interface IOnTileModifierCreate_Tile : ITileHook { void OnTileModifierCreate(TileModifier modifier); }
public interface IOnTileModifierCreate_Game : IGameHook { void OnTileModifierCreate(TileModifier modifier); }

public interface IOnTileModifierRemove_Tile : ITileHook { void OnTileModifierRemove(TileModifier modifier); }
public interface IOnTileModifierRemove_Game : IGameHook { void OnTileModifierRemove(TileModifier modifier); }


public interface IOnEdgeModifierCreate_Edge : IEdgeHook { void OnEdgeModifierCreate(EdgeModifier modifier); }
public interface IOnEdgeModifierCreate_Game : IGameHook { void OnEdgeModifierCreate(EdgeModifier modifier); }

public interface IOnEdgeModifierRemove_Edge : IEdgeHook { void OnEdgeModifierRemove(EdgeModifier modifier); }
public interface IOnEdgeModifierRemove_Game : IGameHook { void OnEdgeModifierRemove(EdgeModifier modifier); }


public interface IOnAbilityCastStart_Unit : IUnitHook { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Tile : ITileHook { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Game : IGameHook { void OnAbilityCastStart(Ability ability); }

public interface IOnAbilityCastEnd_Unit : IUnitHook { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Tile : ITileHook { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Game : IGameHook { void OnAbilityCastEnd(Ability ability); }

public interface IOnDispell_Unit : IUnitHook { void OnDispell(); }
public interface IOnDispell_Tile : ITileHook { void OnDispell(Unit unit); }
public interface IOnDispell_Game : IGameHook { void OnDispell(Unit unit); }

public interface IOnEnergyDeficit_Unit : IUnitHook { void OnEnergyDeficit(int deficit); }
public interface IOnEnergyDeficit_Tile : ITileHook { void OnEnergyDeficit(Unit unit, int deficit); }
public interface IOnEnergyDeficit_Game : IGameHook { void OnEnergyDeficit(Unit unit, int deficit); }

public interface IOnEnergyExcess_Unit : IUnitHook { void OnEnergyExcess(int excess); }
public interface IOnEnergyExcess_Tile : ITileHook { void OnEnergyExcess(Unit unit, int excess); }
public interface IOnEnergyExcess_Game : IGameHook { void OnEnergyExcess(Unit unit, int excess); }

public interface IOnGetEstimatedSpeed_Unit : IUnitHook { void OnGetEstimatedSpeed(int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Tile : ITileHook { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Game : IGameHook { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }

public interface IOnHeal_Unit : IUnitHook { void OnHeal(ref float value); }
public interface IOnHeal_Tile : ITileHook { void OnHeal(Unit unit, ref float value); }
public interface IOnHeal_Game : IGameHook { void OnHeal(Unit unit, ref float value); }

public interface IOnTakeDamage_Unit : IUnitHook { void OnTakeDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Tile : ITileHook { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Game : IGameHook { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }

public interface IOnDealDamage_Unit : IUnitHook { void OnDealDamage(UnitModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Edge : IEdgeHook { void OnDealDamage(EdgeModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Tile : ITileHook { void OnDealDamage(TileModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Game : IGameHook { void OnDealDamage(Modifier source, Unit target, float damage, DamageType type); }

public interface IOnCalculateDamage_Unit : IUnitHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Tile : ITileHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Game : IGameHook { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }

public interface IOnSpawn_Unit : IUnitHook { void OnSpawn(); }
public interface IOnSpawn_Tile : ITileHook { void OnSpawn(Unit unit); }
public interface IOnSpawn_Game : IGameHook { void OnSpawn(Unit unit); }

public interface IOnDeath_Unit : IUnitHook { void OnDeath(); }
public interface IOnDeath_Tile : ITileHook { void OnDeath(Unit unit); }
public interface IOnDeath_Game : IGameHook { void OnDeath(Unit unit); }

public interface IOnTurnStart_Unit : IUnitHook { void OnTurnStart(); }
public interface IOnTurnStart_Tile : ITileHook { void OnTurnStart(Unit unit); }
public interface IOnTurnStart_Game : IGameHook { void OnTurnStart(Unit unit); }

public interface IOnTurnEnd_Unit : IUnitHook { void OnTurnEnd(); }
public interface IOnTurnEnd_Tile : ITileHook { void OnTurnEnd(Unit unit); }
public interface IOnTurnEnd_Game : IGameHook { void OnTurnEnd(Unit unit); }

public interface IOnRoundStart : IGameHook { void OnRoundStart(); }

public interface IOnTeamReady : IGameHook { void OnTeamReady(Team team); }

public interface IOnGameInit : IGameHook { void OnGameInit(); }
public interface IOnGameStart : IGameHook { void OnGameStart(); }
public interface IOnGameEnd : IGameHook { void OnGameEnd(); }

public interface IOnAnimationEventStart : IGameHook { void OnAnimationEventStart(EventHandler handler); }
public interface IOnAnimationEventEnd : IGameHook { void OnAnimationEventEnd(); }

public interface IOnTargeterStart : IGameHook { void OnTargeterStart(Targeter targeter); }
public interface IOnTargeterEnd : IGameHook { void OnTargeterEnd(); }

public interface IOnCombatLog : IGameHook { void OnCombatLog(string str); }

public interface IOnChangeDirection_Unit : IUnitHook { void OnChangeDirection(TileDir from, TileDir to); }
public interface IOnChangeDirection_Tile : ITileHook { void OnChangeDirection(Unit unit, TileDir from, TileDir to); }
public interface IOnChangeDirection_Game : IGameHook { void OnChangeDirection(Unit unit, TileDir from, TileDir to); }

public interface IOnChangePosition_Unit : IUnitHook { void OnChangePosition(Tile? from, Tile to); }
public interface IOnChangePosition_Tile : ITileHook { void OnChangePosition(Unit unit, Tile? from, Tile to); }
public interface IOnChangePosition_Game : IGameHook { void OnChangePosition(Unit unit, Tile? from, Tile to); }

public interface IOnMoveOn_Tile : ITileHook { void OnMoveOn(Modifier reason, Unit unit); }
public interface IOnMoveOn_Unit : IUnitHook { void OnMoveOn(Modifier reason, Tile tile); }
public interface IOnMoveOn_Game : IGameHook { void OnMoveOn(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOff_Tile : ITileHook { void OnMoveOff(Modifier reason, Unit unit); }
public interface IOnMoveOff_Unit : IUnitHook { void OnMoveOff(Modifier reason, Tile tile); }
public interface IOnMoveOff_Game : IGameHook { void OnMoveOff(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOver_Edge : IEdgeHook { void OnMoveOver(Modifier reason, Unit unit, Tile from, Tile to); }
public interface IOnMoveOver_Unit : IUnitHook { void OnMoveOver(Modifier reason, Tile from, Edge edge, Tile to); }
public interface IOnMoveOver_Game : IGameHook { void OnMoveOver(Modifier reason, Unit unit, Tile from, Edge edge, Tile to); }

public interface IOnGetCanPass_Edge : IEdgeHook {
	void OnGetCanPass(Tile from, Tile to, ref bool current);
	void OnGetCanPass(Unit unit, Tile from, Tile to, ref bool current);
}
public interface IOnGetCanPass_Unit : IUnitHook {
	void OnGetCanPass(Tile from, Edge edge, Tile to, ref bool current);
}
public interface IOnGetCanPass_Game : IGameHook {
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
public interface IOnGetMoveCost_Game : IGameHook {
	void OnGetMoveCost(Tile from, Edge edge, Tile to, ref float current);
	void OnGetMoveCost(Unit unit, Tile from, Edge edge, Tile to, ref float current);
}

#nullable disable
