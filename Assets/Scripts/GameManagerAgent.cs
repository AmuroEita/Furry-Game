using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerAgent : MonoBehaviour
{
	[SerializeField] Text gold = null;
	[SerializeField] Text morale = null;
	[SerializeField] Text health = null;

	public bool EnableAI
	{
		get => GameManager.Instance.EnableAI;
		set => GameManager.Instance.EnableAI = value;
	}

	private void Update()
	{
		if (gold != null)
			gold.text = GameManager.Instance.Gold.ToString();
		if (morale != null)
			morale.text = GameManager.Instance.Morale.ToString();
		if (health != null)
			health.text = GameManager.Instance.Health.ToString();
	}
}
