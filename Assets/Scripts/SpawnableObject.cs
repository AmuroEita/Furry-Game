using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JTUtility;

public abstract class SpawnableObject : MonoBehaviour
{
    [Serializable] public class BasePropertyPair : PairedValue<int, bool> { }

    [Serializable] public class DispPropertyGroup
    {
        public BasePropertyPair goldCost;
        public BasePropertyPair moraleCost;
        public BasePropertyPair health;
        public BasePropertyPair damage;
    }

    [Header("Base-Properties")]
    [SerializeField] protected DispPropertyGroup properties;

    [Header("Base-References")]
    [SerializeField] protected Transform spriteGroup;

    [SerializeField] protected SpawnableObject[] additionalObject;

    [SerializeField] protected MonoBehaviour parent;

    public DispPropertyGroup Properties => properties;
    public Transform SpriteGroup => spriteGroup;

    public abstract void Init();

    public virtual void SpawnOn(Vector3 position, MonoBehaviour parent)
	{
        this.parent = parent;
        this.transform.position = position;
	}

    public abstract void Remove();

    public abstract bool CanSpawnAt(Vector3 position);
}
