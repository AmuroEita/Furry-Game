using JTUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
	public enum PutbackMethod
	{ 
		Top,
		Random,
		Bottom
	}

	[SerializeField] Card cardPrefab = null;
	[SerializeField] List<SpawnableObject> presetSpawnObjects = null;
	[SerializeField] List<SpawnableObject> pickeddSpawnObjects = null;
	[SerializeField] RectTransform cardPanel = null;
	[SerializeField] int sizeOfDeck = 20;
	[SerializeField] List<SpawnableObject> currentCards;

	private void Awake()
	{
		CreateDeck();
	}

	public void CreateDeck()
	{
		currentCards = new List<SpawnableObject>();
		foreach (var item in presetSpawnObjects)
		{
			currentCards.Add(item);
		}

		while(currentCards.Count < sizeOfDeck)
		{
			currentCards.Add(pickeddSpawnObjects.PickRandom());
		}
	}

	public Card DrawCard()
	{
		var obj = currentCards.PickRandom();
		currentCards.Remove(obj);
		return CreateCard(obj);
	}

	public void Putback(Card card)
	{
		currentCards.Add(card.SpawnObject);

	}

	private Card CreateCard(SpawnableObject obj)
	{
		var card = Instantiate(cardPrefab);
		card.SetSpawnObject(obj);
		return card;
	}
}
