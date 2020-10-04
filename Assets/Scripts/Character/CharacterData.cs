using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public enum CDState
{
    /// <summary>
    /// 0 - Hostiles, 1 - Friendlies
    /// </summary>
    Faction,
    AIStatus,
    Living,
	Deploying,
	Targetable,
    MoveCommand,
}

public enum CDInt
{
    GoldCost,
    MoraleCost,
}

public enum CDFloat
{
    Speed,
	Health,
	DeployTime,
    AlertRange,
    AttackRange,
    AttackDamage,
    AttackFreq,
}

public enum CDVector
{
    Position,
    Destination,
}

public enum CDRef
{
    ///<summary>
    /// The units this character is attacking
    ///</summary>
    AttackingUnits,
    ///<summary>
    /// The units attacking this character
    ///</summary>
    BeingAttackUnits,
    ///<summary>
    /// The units this character is awared of
    ///</summary>
    AlertingUnits,
    ///<summary>
    /// The units that noticed this character
    ///</summary>
    BeingAlertUnits,
}

public class CharacterData
{
    public Character host;
    public Dictionary<CDState, int> states;
    public Dictionary<CDVector, Vector3> vectors;
    public Dictionary<CDInt, StateValueInt> ints;
    public Dictionary<CDFloat, StateValue> floats;
    public Dictionary<CDRef, object> refs;

    public CharacterData(Character host)
	{
        this.host = host;
		states = new Dictionary<CDState, int>();
		vectors = new Dictionary<CDVector, Vector3>();
		ints = new Dictionary<CDInt, StateValueInt>();
		floats = new Dictionary<CDFloat, StateValue>();
		refs = new Dictionary<CDRef, object>();
	}
}
