using JTUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : SpawnableObject, ICanOccupy
{
	[Header("Unit-Properties")]
	[SerializeField] float moveSpeed = 1;
	[SerializeField] float attackFrequency = 1;
	[SerializeField] float alertRange = 3;
	[SerializeField] float attackRange = 1;
	[SerializeField] float deployTime = 3;
	[SerializeField] bool isPlayerUnit = false;
	[SerializeField] int occupationSize;

	[Header("Unit-References")]
	[SerializeField] Image healthBar = null;
	[SerializeField] Image depolyBar = null;
	[SerializeField] SpriteRenderer selectFrame = null;
	[SerializeField] LineRenderer attackIndicator = null;

	Timer deployTimer;
	Timer attackTimer;
	Dictionary<SpriteRenderer, Color> origColorMap;

	public CharacterData data = null;

	[SerializeField]
	bool _canBeSelected = false;
	public bool CanBeSelected => _canBeSelected;

	public bool IsPlayerUnit => isPlayerUnit;

	public bool Selected
	{
		get => selectFrame.enabled;
		set => selectFrame.enabled = value;
	}

	int ICanOccupy.Size => occupationSize;

	#region Override Methods
	public override void Init()
	{
		if (data != null) return;

		data = new CharacterData(this);
		data.ints.Add(CDInt.GoldCost, new StateValueInt(properties.goldCost.Key));
		data.ints.Add(CDInt.MoraleCost, new StateValueInt(properties.moraleCost.Key));
		data.floats.Add(CDFloat.Health, new StateValue(properties.health.Key));
		data.floats.Add(CDFloat.DeployTime, new StateValue(deployTime));
		data.floats.Add(CDFloat.Speed, new StateValue(moveSpeed));

		data.floats.Add(CDFloat.AlertRange, new StateValue(alertRange));
		data.floats.Add(CDFloat.AttackRange, new StateValue(attackRange));
		data.floats.Add(CDFloat.AttackDamage, new StateValue(properties.damage.Key / attackFrequency));
		data.floats.Add(CDFloat.AttackFreq, new StateValue(attackFrequency));

		data.vectors.Add(CDVector.Position, transform.position);
		data.vectors.Add(CDVector.Destination, Vector3.zero);

		data.states.Add(CDState.Faction, isPlayerUnit? 1:0);
		data.states.Add(CDState.Living, 1);
		data.states.Add(CDState.Targetable, 0);
		data.states.Add(CDState.AIStatus, 0);

		if (attackIndicator != null)
		{
			attackIndicator.startColor = attackIndicator.startColor.AlterAlpha(0);
			attackIndicator.endColor = attackIndicator.endColor.AlterAlpha(0);
		}

		deployTimer = new Timer();
		attackTimer = new Timer();
		deployTimer.OnProgress += DeployingTimer_OnProgress;
		attackTimer.OnProgress += AttackingTimer_OnProgress;
	}

	public override void SpawnOn(Vector3 position, MonoBehaviour parent)
	{
		base.SpawnOn(position, parent);
		data.vectors[CDVector.Position] = transform.position;

		GameManager.Instance.RegisterUnit(this, isPlayerUnit);
		if (GameManager.Instance.CurrentStage == BattleStage.RoundProcess)
		{
			Deploying();
			deployTimer.Start(deployTime, HandleDeployingCallback);
		}
		else
		{
			Deploy();
		}
	}

	public override void Remove()
	{
		GameManager.Instance.UnregisterUnit(this, isPlayerUnit);
	}

	public override bool CanSpawnAt(Vector3 position)
	{
		var tile = GameManager.Instance.CurrentMap.GetTile<WorldTile>(position);

		return tile != null && tile.TileType != TileType.Obstacle;
	}
	#endregion

	#region Unity Messages

	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		data.vectors[CDVector.Position] = transform.position;
	}

	private void OnDestroy()
	{
		Remove();
		deployTimer?.Dispose();
		attackTimer?.Dispose();
	}

	#endregion

	#region Deploy
	private void DeployingTimer_OnProgress(Timer timer)
	{
		depolyBar.fillAmount = timer.PassedPercentage;
	}

	protected virtual void Deploying()
	{
		data.states[CDState.Targetable] = 0;
		_canBeSelected = false;
		depolyBar.transform.parent.gameObject.SetActive(true);
		origColorMap = new Dictionary<SpriteRenderer, Color>();
		foreach (var renderer in SpriteGroup.GetComponentsInChildren<SpriteRenderer>())
		{
			origColorMap.Add(renderer, renderer.color);
			Color color = renderer.color;
			color.a = 0.5f;
			renderer.color = color;
		}
	}

	protected virtual void Deploy()
	{
		data.states[CDState.Targetable] = 1;
		_canBeSelected = true;
		depolyBar.transform.parent.gameObject.SetActive(false);
		if (origColorMap == null) return;
		foreach (var renderer in SpriteGroup.GetComponentsInChildren<SpriteRenderer>())
		{
			renderer.color = origColorMap[renderer];
		}
	}

	private void HandleDeployingCallback(Timer timer)
	{
		Deploy();
	}
	#endregion

	#region AI API

	public void MoveCommand(Vector3 position)
	{
		data.vectors[CDVector.Destination] = position;
		data.states[CDState.MoveCommand] = 1;
	}

	public void MoveTo(Vector3 position)
	{
		data.vectors[CDVector.Destination] = position;
		Debug.DrawLine(data.vectors[CDVector.Position], position, Color.blue);
		var toPosition = Vector3.MoveTowards(data.vectors[CDVector.Position], position, Time.deltaTime * data.floats[CDFloat.Speed]);
		var tile = GameManager.Instance.CurrentMap.GetTile<WorldTile>(toPosition);
		var type = tile?.TileType;

		switch(type)
		{
			case null:
			case TileType.Obstacle:
				// If cannot move to that position, stop moving
				data.vectors[CDVector.Destination] = data.vectors[CDVector.Position];
				return;

			case TileType.Forest:
				data.floats[CDFloat.Speed].Percent = 0.5f;
				break;
			case TileType.Ground:
			case TileType.Path:
				data.floats[CDFloat.Speed].Percent = 1f;
				break;
			default:
				break;
		}

		transform.Translate(toPosition - data.vectors[CDVector.Position]);
	}

	public void Attack(Character character)
	{
		Debug.DrawLine(data.vectors[CDVector.Position], character.data.vectors[CDVector.Position], Color.red);
		attackIndicator.enabled = true;
		if (!attackTimer.IsReachedTime())
			attackTimer.Abort();

		character.TakeAttack(data.floats[CDFloat.AttackDamage]);
		attackTimer.Start(1/data.floats[CDFloat.AttackFreq]);
	}

	private void AttackingTimer_OnProgress(Timer timer)
	{
		if (attackIndicator != null)
		{
			attackIndicator.SetPosition(0, data.vectors[CDVector.Position]);
			attackIndicator.SetPosition(1, ((List<Character>)data.refs[CDRef.AttackingUnits])[0].data.vectors[CDVector.Position]);
			attackIndicator.startColor = attackIndicator.startColor.AlterAlpha(timer.PassedPercentage);
			attackIndicator.endColor = attackIndicator.endColor.AlterAlpha(timer.PassedPercentage);
		}
	}

	#endregion

	#region Action

	public void Move(Vector3 position)
	{
		Debug.DrawLine(data.vectors[CDVector.Position], position, Color.blue);
		var toPosition = Vector3.MoveTowards(transform.position, position, Time.deltaTime * data.floats[CDFloat.Speed]);
		transform.Translate(toPosition - transform.position);
	}

	public void TakeAttack(float attack)
	{
		data.floats[CDFloat.Health] -= attack;
		healthBar.fillAmount = data.floats[CDFloat.Health].Percent;
		if (data.floats[CDFloat.Health] <= 0)
		{
			data.states[CDState.Living] = 0;
			data.states[CDState.Targetable] = 0;
			Destroy(this.gameObject, 0.1f);
		}
	}

	#endregion
}
