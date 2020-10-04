using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public enum BattleStage
{
    Start,
    RoundStart,
    RoundProcess,
    RoundEnd,
    Win,
    Lose,
}

public class GameManager : GlobalSingleton<GameManager>
{
    [SerializeField] List<Card> cards = new List<Card>();
    [SerializeField] List<Map> maps = new List<Map>();
    [Header("Debug")]
    [SerializeField] BattleStage _currentStage = BattleStage.Start;
    [SerializeField] List<Character> otherUnits = new List<Character>();
    [SerializeField] List<Character> playerUnits = new List<Character>();

	int gold;
	int morale;
    int health;
    Map _currentMap;
    Camera _cacheCamera;
    Grid _cacheGrid;

	bool _enableAI = true;
	public bool EnableAI
	{
		get => _enableAI;
		set
		{
			if (value == _enableAI)
				return;

			_enableAI = value;
			foreach (var unit in EnemyUnits)
			{
				unit.GetComponent<GeneralUnitAI>().enabled = value;
			}

			foreach (var unit in PlayerUnits)
			{
				unit.GetComponent<GeneralUnitAI>().enabled = value;
			}
		}
	}

    public int Gold => gold;
    public int Morale => morale;
    public int Health => health;

    public Map CurrentMap => _currentMap;

    public Camera MainCamera => _cacheCamera ? _cacheCamera : _cacheCamera = Camera.main;
    public BattleStage CurrentStage => _currentStage;
    public List<Character> PlayerUnits => playerUnits;
    public List<Character> EnemyUnits => otherUnits;

    public event Action<BattleStage> OnStageChanged;

	private void Start()
	{
        GameStart();
    }

	public void GameStart()
	{
        if (_cacheGrid == null)
		{
            _cacheGrid = new GameObject("Grid").AddComponent<Grid>();
            _cacheGrid.cellSwizzle = GridLayout.CellSwizzle.XZY;
            _cacheGrid.transform.position = Vector3.zero;
            _cacheGrid.transform.eulerAngles = Vector3.up * -45;
        }
        _currentMap = Instantiate(maps.PickRandom());
        _currentMap.transform.SetParent(_cacheGrid.transform);
        _currentMap.transform.position = _cacheGrid.transform.position;
        _currentMap.transform.rotation = _cacheGrid.transform.rotation;

        gold = _currentMap.StartingGold;
		morale = _currentMap.StartingMorale;
        health = _currentMap.StartingHealth;
    }

    public void ProgressStage()
	{
        if (_currentStage == BattleStage.Start)
		{
            _currentStage = BattleStage.RoundStart;
            OnStageChanged?.Invoke(_currentStage);
        }
        else if(_currentStage == BattleStage.RoundStart)
        {
            _currentStage = BattleStage.RoundProcess;
            OnStageChanged?.Invoke(_currentStage);
        }
        else if (_currentStage == BattleStage.RoundProcess)
        {
            _currentStage = BattleStage.RoundEnd;
            OnStageChanged?.Invoke(_currentStage);
        }
        else if (_currentStage == BattleStage.RoundEnd)
        {
            _currentStage = BattleStage.RoundProcess;
            OnStageChanged?.Invoke(_currentStage);
        }
    }

	public void MoveUnits(ICollection<Character> units, Vector3 position)
	{
		foreach (var unit in units)
		{
			unit.MoveCommand(position);
		}
	}

    public Vector3 ScreenToMapPosition(Vector3 position)
	{
        var ray = MainCamera.ScreenPointToRay(position);
        var t = ray.origin.y / ray.direction.y;
        return ray.origin - ray.direction * t;
    }

    public bool CanSpawn(SpawnableObject prefab, Vector3 position)
	{
        if (!prefab.CanSpawnAt(position))
            return false;

        if (prefab is Character)
		{
            var character = prefab as Character;
            if (character.IsPlayerUnit)
			{
                if (character.Properties.goldCost.Key > Gold)
                    return false;
			}
        }

        return true;
	}

	public void RegisterUnit(Character unit, bool isPlayerUnit, bool withCost = true)
	{
		unit.GetComponent<GeneralUnitAI>().enabled = EnableAI;
		if (isPlayerUnit)
		{
            playerUnits.Add(unit);
            if (withCost)
            {
                gold -= unit.data.ints[CDInt.GoldCost];
            }
        }
        else
		{
            otherUnits.Add(unit);
		}
	}

    public void UnregisterUnit(Character unit, bool isPlayerUnit, bool withCost = true)
	{
        if (isPlayerUnit)
        {
            playerUnits.Remove(unit);
            if (withCost)
            {
                morale -= unit.data.ints[CDInt.MoraleCost];
                if (morale < 0) morale = 0;
            }
        }
        else
        {
            otherUnits.Remove(unit);
            if (withCost)
            {
                gold += unit.data.ints[CDInt.GoldCost];
                morale += unit.data.ints[CDInt.MoraleCost];
            }
        }
    }
}
