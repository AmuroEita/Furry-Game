using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;
using System;
using UnityEngine.Tilemaps;

public enum GroundType
{
	Obstacle = 0,
	Road,
	Forest,
	Ground,
	Void,
}

public class Map : MonoBehaviour
{
	[SerializeField] int startingGold = 100;
	[SerializeField] int startingMorale = 100;
	[SerializeField] int startingHealth = 100;
	[SerializeField] Tilemap tilemap;

	public int StartingGold => startingGold;
	public int StartingMorale => startingMorale;
	public int StartingHealth => startingHealth;

	private void Awake()
	{
		if (tilemap == null)
		{
			Debug.LogError($"This Map({name}) needs a TileMap to work.");
			this.enabled = false;
			return;
		}
	}

	public T GetTile<T>(Vector3 position) where T : TileBase
	{
		return tilemap.GetTile<T>(tilemap.WorldToCell(position));
	}
}
