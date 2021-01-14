

public interface IOnEvent { }
public interface IUnitOnEvent : IOnEvent { }
public interface ITileOnEvent : IOnEvent { }
public interface IEdgeOnEvent : IOnEvent { }
public interface IGlobalOnEvent : IOnEvent { }


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

public interface IOnGetEstimatedSpeed_Unit : IUnitOnEvent { int OnGetEstimatedSpeed(int roundsAhead); }
public interface IOnGetEstimatedSpeed_Tile : ITileOnEvent { int OnGetEstimatedSpeed(Unit unit, int roundsAhead); }
public interface IOnGetEstimatedSpeed_Global : IGlobalOnEvent { int OnGetEstimatedSpeed(Unit unit, int roundsAhead); }

public interface IOnHeal_Unit : IUnitOnEvent { float OnHeal(float value); }
public interface IOnHeal_Tile : ITileOnEvent { float OnHeal(Unit unit, float value); }
public interface IOnHeal_Global : IGlobalOnEvent { float OnHeal(Unit unit, float value); }
public interface IOnDamage_Unit : IUnitOnEvent { (float damage, DamageType type) OnDamage(float damage, DamageType type); }
public interface IOnDamage_Tile : ITileOnEvent { (float damage, DamageType type) OnDamage(Unit unit, float damage, DamageType type); }
public interface IOnDamage_Global : IGlobalOnEvent { (float damage, DamageType type) OnDamage(Unit unit, float damage, DamageType type); }

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

public interface IOnGameStart : IGlobalOnEvent { void OnGameStart(); }
public interface IOnGameEnd : IGlobalOnEvent { void OnGameEnd(); }

public interface IOnAnimationEventStart : IGlobalOnEvent { void OnAnimationEventStart(EventHandler handler); }
public interface IOnAnimationEventEnd : IGlobalOnEvent { void OnAnimationEventEnd(); }

public interface IOnTargeterStart : IGlobalOnEvent { void OnTargeterStart(Targeter targeter); }
public interface IOnTargeterEnd : IGlobalOnEvent { void OnTargeterEnd(); }

public interface IOnMoveOn_Tile : ITileOnEvent { void OnMoveOn(Unit unit); }
public interface IOnMoveOn_Unit : IUnitOnEvent { void OnMoveOn(Tile tile); }
public interface IOnMoveOn_Global : IGlobalOnEvent { void OnMoveOn(Unit unit, Tile tile); }
public interface IOnMoveOff_Tile : ITileOnEvent { void OnMoveOff(Unit unit); }
public interface IOnMoveOff_Unit : IUnitOnEvent { void OnMoveOff(Tile tile); }
public interface IOnMoveOff_Global : IGlobalOnEvent { void OnMoveOff(Unit unit, Tile tile); }

public interface IOnMoveOver_Edge : IEdgeOnEvent { void OnMoveOver(Unit unit, Tile from, Tile to); }
public interface IOnMoveOver_Unit : IUnitOnEvent { void OnMoveOver(Tile from, Edge edge, Tile to); }
public interface IOnMoveOver_Global : IGlobalOnEvent { void OnMoveOver(Unit unit, Tile from, Edge edge, Tile to); }

public interface IOnGetCanPass_Edge : IEdgeOnEvent {
	bool OnGetCanPass(Tile from, Tile to, bool current);
	bool OnGetCanPass(Unit unit, Tile from, Tile to, bool current);
}
public interface IOnGetCanPass_Unit : IUnitOnEvent {
	bool OnGetCanPass(Tile from, Edge edge, Tile to, bool current);
}
public interface IOnGetCanPass_Global : IGlobalOnEvent {
	bool OnGetCanPass(Tile from, Edge edge, Tile to, bool current);
	bool OnGetCanPass(Unit unit, Tile from, Edge edge, Tile to, bool current);
}

public interface IOnGetMoveCost_Edge : IEdgeOnEvent {
	float OnGetMoveCost(Tile from, Tile to, float current);
	float OnGetMoveCost(Unit unit, Tile from, Tile to, float current);
}
public interface IOnGetMoveCost_Unit : IUnitOnEvent {
	float OnGetMoveCost(Tile from, Edge edge, Tile to, float current);
}
public interface IOnGetMoveCost_Global : IGlobalOnEvent {
	float OnGetMoveCost(Tile from, Edge edge, Tile to, float current);
	float OnGetMoveCost(Unit unit, Tile from, Edge edge, Tile to, float current);
}

public interface IOnGetCalculatedAbilityDamage_Unit : IUnitOnEvent {
	float OnGetCalculatedAbilityDamage(float damage, Ability ability, DamageType damageType);
}
public interface IOnGetCalculatedAbilityDamage_Tile : ITileOnEvent {
	float OnGetCalculatedAbilityDamage(float damage, Ability ability, DamageType damageType);
}
public interface IOnGetCalculatedAbilityDamage_Global : IGlobalOnEvent {
	float OnGetCalculatedAbilityDamage(float damage, Ability ability, DamageType damageType);
}
