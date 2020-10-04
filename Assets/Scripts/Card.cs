using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using JTUtility;

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] Text goldCostText = null;
    [SerializeField] Text moraleCostText = null;
    [SerializeField] Text healthText = null;
    [SerializeField] Text damageText = null;
	[SerializeField] RectTransform portrait = null;
	[SerializeField] string defaultLayer = "MainGUI";

    [SerializeField] string description = string.Empty;

	[SerializeField] GameObject spawnIndicator = null;

	[SerializeField] SpawnableObject spawnObject = null;

	public SpawnableObject SpawnObject => spawnObject;

	private void Start()
	{
		if (spawnObject == null)
		{
			Debug.LogError($"Card({name}) needs a CardSpawnObject to work");
			this.enabled = false;
			return;
		}

		// Set info display
		this.gameObject.name = spawnObject.name + "_Card";
		goldCostText.enabled = spawnObject.Properties.goldCost.Value;
		moraleCostText.enabled = spawnObject.Properties.moraleCost.Value;
		healthText.enabled = spawnObject.Properties.health.Value;
		damageText.enabled = spawnObject.Properties.damage.Value;

		goldCostText.text = spawnObject.Properties.goldCost.Key.ToString();
		moraleCostText.text = spawnObject.Properties.moraleCost.Key.ToString();
		healthText.text = spawnObject.Properties.health.Key.ToString();
		damageText.text = spawnObject.Properties.damage.Key.ToString();

		// Set up portrait
		var sprite = Instantiate(spawnObject.SpriteGroup);
		sprite.SetParent(portrait);
		sprite.localPosition = Vector2.zero;
		var sortGroup = sprite.GetComponent<SortingGroup>();
		if (sortGroup != null)
		{
			sortGroup.sortingLayerName = defaultLayer;
			sortGroup.transform.SetParent(portrait);
			sortGroup.transform.localPosition = new Vector3(0, 0, -5);
		}

		var spriteRenderer = sprite.GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			spriteRenderer.sortingLayerName = defaultLayer;
			spriteRenderer.transform.SetParent(portrait);
			spriteRenderer.transform.localPosition = new Vector3(0, 0, -5);
		}

		// Set up spawn indicator
		if (spawnIndicator == null)
		{
			if (sortGroup != null)
				spawnIndicator = sortGroup.gameObject;
			else if (spriteRenderer != null)
				spawnIndicator = spriteRenderer.gameObject;

			if (spawnIndicator != null)
			{
				var instance = Instantiate(spawnIndicator);
				var root = new GameObject("Spawn Indicator").transform;

				foreach (var renderer in instance.GetComponentsInChildren<SpriteRenderer>())
				{
					if (renderer.color.a > 0.5f)
						renderer.color = renderer.color.AlterAlpha(0.5f);
				}
				root.transform.rotation = spawnObject.SpriteGroup.root.rotation;
				root.transform.position = spawnObject.SpriteGroup.root.position;
				root.transform.localScale = spawnObject.SpriteGroup.root.localScale;
				instance.transform.SetParent(root);
				instance.transform.localScale = spawnObject.SpriteGroup.localScale;
				instance.transform.localPosition = spawnObject.SpriteGroup.localPosition;
				instance.transform.localRotation = spawnObject.SpriteGroup.localRotation;
				spawnIndicator = root.gameObject;
			}
			else
			{
				spawnIndicator = new GameObject("indicator").AddComponent<SpriteRenderer>().gameObject;
			}

			spawnIndicator.transform.SetParent(this.transform);
			spawnIndicator.SetActive(false);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		spawnIndicator.SetActive(true);
		var position = GameManager.Instance.ScreenToMapPosition(eventData.position);
		spawnIndicator.transform.position = position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		print("OnDrop");
		spawnIndicator.SetActive(false);

		var position = GameManager.Instance.ScreenToMapPosition(eventData.position);
		if (!GameManager.Instance.CanSpawn(spawnObject, position))
			return;

		Instantiate(spawnObject).SpawnOn(position, this);
		//this.enabled = false;
	}

	public void SetSpawnObject(SpawnableObject obj)
	{
		spawnObject = obj;
	}
}
