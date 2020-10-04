using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class GeneralUnitAI : MonoBehaviour
{
	protected Root root = BT.Root();
	protected GeneralUnitBehaviour behave;

    private void OnEnable()
    {
        var hostData = GetComponent<Character>()?.data;

		root.OpenBranch(
			BT.Call(behave.RefreshCombatStatus),
			BT.Selector().OpenBranch(
				BT.Sequence().OpenBranch(
					BT.Condition(behave.MoveCondition),
					BT.Call(behave.Move)
				),
				BT.Sequence().OpenBranch(
					BT.Condition(behave.AttackCondition),
					BT.Call(behave.Attack),
					BT.Wait(1 / hostData.floats[CDFloat.AttackFreq])
				),
				BT.Sequence().OpenBranch(
					BT.Condition(behave.AlertConditioin),
					BT.Call(behave.MoveToEnemy)
					),
				BT.Call(behave.Idle)
			)
		);
	}

	private void Awake()
	{
		behave = GetComponent<GeneralUnitBehaviour>();
		if (behave == null)
		{
			Debug.LogError($"This AI({name}) requires a behavior object to function!");
			this.enabled = false;
		}
	}

	void Update()
    {
		// Force it to finish the whole tree at least once per frame.
		// Use a for loop so if there's a "Wait" type node it won't loop forever
		for (int i = 0; i < 7 && root.Tick() == BTState.Continue; i++) ;
    }
}
