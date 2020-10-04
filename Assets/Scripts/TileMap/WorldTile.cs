using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using JTUtility;

public enum TileType
{
    Ground,
    Path,
    Forest,
    Obstacle,
}

public interface ICanOccupy
{
    int Size { get; }
}

[CreateAssetMenu(fileName ="WorldTile", menuName = "Tiles/WorldTile")]
[System.Serializable]
public class WorldTile : Tile
{
    [Header("World Tile")]
    [SerializeField]
    int unitCapcity = 3;

    [SerializeField]
    TileType tileType;

    List<ICanOccupy> occupants;

    public TileType TileType => tileType;

    public int UnitCapcity => unitCapcity;

    public bool CanEnter(ICanOccupy occupant)
	{
        return true;
	}

    public void Enter(ICanOccupy occupant)
	{
        if (!CanEnter(occupant))
            return;
	}

    public void Leave(ICanOccupy occupant)
	{

	}

    public event Action<WorldTile> OnOccupantsChanged;
}
