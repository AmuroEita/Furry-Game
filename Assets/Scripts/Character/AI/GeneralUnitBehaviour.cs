using JTUtility;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUnitBehaviour : MonoBehaviour
{
	protected CharacterData hostData;

    public virtual void RefreshCombatStatus()
    {
        List<Character> targets;
        if (!hostData.refs.ContainsKey(CDRef.AttackingUnits))
        {
            targets = new List<Character>();
            hostData.refs.Add(CDRef.AttackingUnits, targets);
        }
        else
        {
            targets = (List<Character>)hostData.refs[CDRef.AttackingUnits];
        }

        List<Character> attackers;
        if (!hostData.refs.ContainsKey(CDRef.BeingAttackUnits))
        {
            attackers = new List<Character>();
            hostData.refs.Add(CDRef.BeingAttackUnits, attackers);
        }
        else
        {
            attackers = (List<Character>)hostData.refs[CDRef.BeingAttackUnits];
        }

        // Remove those no longer existed
        targets.RemoveAll((c) => { return !c.IsExists(); });
        attackers.RemoveAll((c) => { return !c.IsExists(); });
    }

    public virtual bool MoveCondition()
	{
        return hostData.states.ContainsKey(CDState.MoveCommand)
            && hostData.states[CDState.MoveCommand] != 0;
    }

	public virtual void Idle()
	{

	}

	public virtual void Move()
	{
		var position = hostData.vectors[CDVector.Position];
		var destination = hostData.vectors[CDVector.Destination];
		if (Vector2.Distance(position, destination) < 0.02f)
		{
			hostData.states[CDState.MoveCommand] = 0;
		}
		else
		{
			hostData.host.MoveTo(destination);
		}
	}

	public virtual void MoveToEnemy()
    {
        List<Character> enemies = (List<Character>)hostData.refs[CDRef.AttackingUnits];

		hostData.host.MoveTo(enemies[0].data.vectors[CDVector.Position]);
    }

    public virtual void Attack()
    {
		var enemies = (List<Character>)hostData.refs[CDRef.AttackingUnits];
		hostData.host.Attack(enemies[0]);
    }

    public virtual bool AttackCondition()
    {
		//RefreshCombatStatus();

		// If no enemy in attacking list, 
		var enemies = (List<Character>)hostData.refs[CDRef.AttackingUnits];
        if (enemies.Count <= 0)
            return false;

        float attackRange = hostData.floats[CDFloat.AttackRange];
        Vector3 thisPosition = hostData.vectors[CDVector.Position];

        // If the first enemy is inside attacking range
        return Vector2.Distance(enemies[0].data.vectors[CDVector.Position], thisPosition) <= attackRange;
    }

    public virtual bool AlertConditioin()
	{
		//RefreshCombatStatus();

		float alertRange = hostData.floats[CDFloat.AlertRange];
		Vector3 thisPosition = hostData.vectors[CDVector.Position];
		List<Character> enemies = (List<Character>)hostData.refs[CDRef.AttackingUnits];

		// Remove those outside of alert range or cannot be target
		enemies.RemoveAll((c)=>
			Vector2.Distance(c.data.vectors[CDVector.Position], thisPosition) > alertRange ||
			c.data.states[CDState.Targetable] == 0
		);

		// If still has enemies left, stay alert
        if (enemies.Count > 0)
		{
            return true;
		}

		// Try to find enemies within alert range
		List<Character> find = new List<Character>();
		List<Character> enemyList = 
			hostData.states[CDState.Faction] == 0 ? 
			GameManager.Instance.PlayerUnits : 
			GameManager.Instance.EnemyUnits;

        foreach (var character in enemyList)
        {
            Vector3 toEnemy = character.data.vectors[CDVector.Position] - thisPosition;
            if (toEnemy.sqrMagnitude < alertRange * alertRange)
				find.Add(character);
        }

		// If no enemy found, drop alert
        if (find.Count <= 0)
		{
            return false;
        }

		// If found enemy, sort it by distance,
		find.Sort((a,b)=>
        {
            var aDist = (a.data.vectors[CDVector.Position] - thisPosition).sqrMagnitude;
            var bDist = (b.data.vectors[CDVector.Position] - thisPosition).sqrMagnitude;
            return aDist.CompareTo(bDist);
        });

		enemies.AddRange(find);

        return true;
    }

    protected virtual void Start()
    {
        hostData = GetComponent<Character>()?.data;
        if (hostData == null)
		{
            Debug.LogError("Cannot get Character's Runtime data");
		}
    }
}
