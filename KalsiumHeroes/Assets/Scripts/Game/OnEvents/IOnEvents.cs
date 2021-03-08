
#nullable enable annotations

public interface IOnEvent { }

public interface IUnitOnEvent : IOnEvent { }
public interface ITileOnEvent : IOnEvent { }
public interface IEdgeOnEvent : IOnEvent { }
public interface IGlobalOnEvent : IOnEvent { }

public interface IOnUpdate : IGlobalOnEvent { void OnUpdate(); }
public interface IOnLateUpdate : IGlobalOnEvent { void OnLateUpdate(); }

public interface IOnAbilityCastStart_Unit : IUnitOnEvent { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Tile : ITileOnEvent { void OnAbilityCastStart(Ability ability); }
public interface IOnAbilityCastStart_Global : IGlobalOnEvent { void OnAbilityCastStart(Ability ability); }

public interface IOnAbilityCastEnd_Unit : IUnitOnEvent { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Tile : ITileOnEvent { void OnAbilityCastEnd(Ability ability); }
public interface IOnAbilityCastEnd_Global : IGlobalOnEvent { void OnAbilityCastEnd(Ability ability); }

public interface IOnDispell_Unit : IUnitOnEvent { void OnDispell(); }
public interface IOnDispell_Tile : ITileOnEvent { void OnDispell(Unit unit); }
public interface IOnDispell_Global : IGlobalOnEvent { void OnDispell(Unit unit); }

public interface IOnEnergyDeficit_Unit : IUnitOnEvent { void OnEnergyDeficit(int deficit); }
public interface IOnEnergyDeficit_Tile : ITileOnEvent { void OnEnergyDeficit(Unit unit, int deficit); }
public interface IOnEnergyDeficit_Global : IGlobalOnEvent { void OnEnergyDeficit(Unit unit, int deficit); }

public interface IOnEnergyExcess_Unit : IUnitOnEvent { void OnEnergyExcess(int excess); }
public interface IOnEnergyExcess_Tile : ITileOnEvent { void OnEnergyExcess(Unit unit, int excess); }
public interface IOnEnergyExcess_Global : IGlobalOnEvent { void OnEnergyExcess(Unit unit, int excess); }

public interface IOnGetEstimatedSpeed_Unit : IUnitOnEvent { void OnGetEstimatedSpeed(int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Tile : ITileOnEvent { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }
public interface IOnGetEstimatedSpeed_Global : IGlobalOnEvent { void OnGetEstimatedSpeed(Unit unit, int roundsAhead, ref int speed); }

public interface IOnHeal_Unit : IUnitOnEvent { void OnHeal(ref float value); }
public interface IOnHeal_Tile : ITileOnEvent { void OnHeal(Unit unit, ref float value); }
public interface IOnHeal_Global : IGlobalOnEvent { void OnHeal(Unit unit, ref float value); }

public interface IOnTakeDamage_Unit : IUnitOnEvent { void OnTakeDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Tile : ITileOnEvent { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }
public interface IOnTakeDamage_Global : IGlobalOnEvent { void OnTakeDamage(Unit unit, Modifier source, ref float damage, ref DamageType type); }

public interface IOnDealDamage_Unit : IUnitOnEvent { void OnDealDamage(UnitModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Edge : IEdgeOnEvent { void OnDealDamage(EdgeModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Tile : ITileOnEvent { void OnDealDamage(TileModifier source, Unit target, float damage, DamageType type); }
public interface IOnDealDamage_Global : IGlobalOnEvent { void OnDealDamage(Modifier source, Unit target, float damage, DamageType type); }

public interface IOnCalculateDamage_Unit : IUnitOnEvent { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Tile : ITileOnEvent { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }
public interface IOnCalculateDamage_Global : IGlobalOnEvent { void OnCalculateDamage(Modifier source, ref float damage, ref DamageType type); }

public interface IOnSpawn_Unit : IUnitOnEvent { void OnSpawn(); }
public interface IOnSpawn_Tile : ITileOnEvent { void OnSpawn(Unit unit); }
public interface IOnSpawn_Global : IGlobalOnEvent { void OnSpawn(Unit unit); }

public interface IOnDeath_Unit : IUnitOnEvent { void OnDeath(); }
public interface IOnDeath_Tile : ITileOnEvent { void OnDeath(Unit unit); }
public interface IOnDeath_Global : IGlobalOnEvent { void OnDeath(Unit unit); }

public interface IOnTurnStart_Unit : IUnitOnEvent { void OnTurnStart(); }
public interface IOnTurnStart_Tile : ITileOnEvent { void OnTurnStart(Unit unit); }
public interface IOnTurnStart_Global : IGlobalOnEvent { void OnTurnStart(Unit unit); }

public interface IOnTurnEnd_Unit : IUnitOnEvent { void OnTurnEnd(); }
public interface IOnTurnEnd_Tile : ITileOnEvent { void OnTurnEnd(Unit unit); }
public interface IOnTurnEnd_Global : IGlobalOnEvent { void OnTurnEnd(Unit unit); }

public interface IOnRoundStart : IGlobalOnEvent { void OnRoundStart(); }

public interface IOnTeamReady : IGlobalOnEvent { void OnTeamReady(Team team); }

public interface IOnGameInit : IGlobalOnEvent { void OnGameInit(); }
public interface IOnGameStart : IGlobalOnEvent { void OnGameStart(); }
public interface IOnGameEnd : IGlobalOnEvent { void OnGameEnd(); }

public interface IOnAnimationEventStart : IGlobalOnEvent { void OnAnimationEventStart(EventHandler handler); }
public interface IOnAnimationEventEnd : IGlobalOnEvent { void OnAnimationEventEnd(); }

public interface IOnTargeterStart : IGlobalOnEvent { void OnTargeterStart(Targeter targeter); }
public interface IOnTargeterEnd : IGlobalOnEvent { void OnTargeterEnd(); }

public interface IOnChangePosition_Unit : IUnitOnEvent { void OnChangePosition(Tile? from, Tile to); }
public interface IOnChangePosition_Tile : ITileOnEvent { void OnChangePosition(Unit unit, Tile? from, Tile to); }
public interface IOnChangePosition_Global : IGlobalOnEvent { void OnChangePosition(Unit unit, Tile? from, Tile to); }

public interface IOnMoveOn_Tile : ITileOnEvent { void OnMoveOn(Modifier reason, Unit unit); }
public interface IOnMoveOn_Unit : IUnitOnEvent { void OnMoveOn(Modifier reason, Tile tile); }
public interface IOnMoveOn_Global : IGlobalOnEvent { void OnMoveOn(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOff_Tile : ITileOnEvent { void OnMoveOff(Modifier reason, Unit unit); }
public interface IOnMoveOff_Unit : IUnitOnEvent { void OnMoveOff(Modifier reason, Tile tile); }
public interface IOnMoveOff_Global : IGlobalOnEvent { void OnMoveOff(Modifier reason, Unit unit, Tile tile); }

public interface IOnMoveOver_Edge : IEdgeOnEvent { void OnMoveOver(Modifier reason, Unit unit, Tile from, Tile to); }
public interface IOnMoveOver_Unit : IUnitOnEvent { void OnMoveOver(Modifier reason, Tile from, Edge edge, Tile to); }
public interface IOnMoveOver_Global : IGlobalOnEvent { void OnMoveOver(Modifier reason, Unit unit, Tile from, Edge edge, Tile to); }

public interface IOnGetCanPass_Edge : IEdgeOnEvent {
	void OnGetCanPass(Tile from, Tile to, ref bool current);
	void OnGetCanPass(Unit unit, Tile from, Tile to, ref bool current);
}
public interface IOnGetCanPass_Unit : IUnitOnEvent {
	void OnGetCanPass(Tile from, Edge edge, Tile to, ref bool current);
}
public interface IOnGetCanPass_Global : IGlobalOnEvent {
	void OnGetCanPass(Tile from, Edge edge, Tile to, ref bool current);
	void OnGetCanPass(Unit unit, Tile from, Edge edge, Tile to, ref bool current);
}

public interface IOnGetMoveCost_Edge : IEdgeOnEvent {
	void OnGetMoveCost(Tile from, Tile to, ref float current);
	void OnGetMoveCost(Unit unit, Tile from, Tile to, ref float current);
}
public interface IOnGetMoveCost_Unit : IUnitOnEvent {
	void OnGetMoveCost(Tile from, Edge edge, Tile to, ref float current);
}
public interface IOnGetMoveCost_Global : IGlobalOnEvent {
	void OnGetMoveCost(Tile from, Edge edge, Tile to, ref float current);
	void OnGetMoveCost(Unit unit, Tile from, Edge edge, Tile to, ref float current);
}

#nullable disable
