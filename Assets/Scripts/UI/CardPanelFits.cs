using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanelFits : MonoBehaviour
{
	[SerializeField] float maxSpacing = 5;

	HorizontalLayoutGroup group;

	List<RectTransform> cards;

	int count = -1;

	private void Awake()
	{
		group = GetComponent<HorizontalLayoutGroup>();
		cards = new List<RectTransform>();
	}

	private void Update()
	{
		var widthSum = -group.spacing;
		if (transform.childCount == count) return;
		count = transform.childCount;

		for (int i = 0; i < transform.childCount; i++)
		{
			widthSum += (transform.GetChild(i) as RectTransform).rect.width + group.spacing;
		}

		var diff = ((RectTransform)group.transform).rect.width - widthSum;
		if (transform.childCount < 2) return;
		diff /= transform.childCount - 1;
		diff += group.spacing;
		if (diff > maxSpacing)
			diff = maxSpacing;

		group.spacing = diff;
	}

	public void AddCard(RectTransform card)
	{
		cards.Add(card);
		Fitting();
	}

	public void RemoveCard(RectTransform card)
	{
		cards.Remove(card);
		Fitting();
	}

	public void Fitting()
	{
		var widthSum = -group.spacing;
		foreach (var item in cards)
		{
			widthSum += item.rect.width + group.spacing;
		}

		var diff = ((RectTransform)group.transform).rect.width - widthSum;
		if (cards.Count < 2) return;
		diff /= cards.Count - 1;
		diff += group.spacing;
		if (diff > maxSpacing)
			diff = maxSpacing;

		group.spacing = diff;
	}
}
