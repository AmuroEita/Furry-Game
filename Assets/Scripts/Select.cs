using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using JTUtility;

public class Select : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] SpriteRenderer selectFrame = null;
	[SerializeField] Bounds selectBounds;

	HashSet<Character> selecting = new HashSet<Character>();
	HashSet<Character> selected = new HashSet<Character>();

	Vector2 startPosition;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			foreach (var unit in selected)
			{
				unit.Selected = false;
			}
			selected.Clear();
		}
		else if (eventData.button == PointerEventData.InputButton.Left)
		{
			var pointerPos = Camera.main.ScreenToWorldPoint(eventData.position).AlterZ(0);
			GameManager.Instance.MoveUnits(selected, pointerPos);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		startPosition = eventData.pointerCurrentRaycast.worldPosition;
		selecting.Clear();
		selectFrame.enabled = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector3 currentPosition = eventData.pointerCurrentRaycast.worldPosition;
		Bounds bounds = new Bounds(startPosition, Vector3.zero);
		bounds.Encapsulate(currentPosition);
		selectBounds = bounds;
		selectFrame.transform.position = bounds.center;
		selectFrame.size = bounds.size;
		
		HashSet<Character> currentSelect = new HashSet<Character>();
		foreach (var unit in GameManager.Instance.PlayerUnits)
		{
			if (bounds.Contains(unit.transform.position))
			{
				if (unit.CanBeSelected)
				{
					currentSelect.Add(unit);
				}
			}
		}
		
		currentSelect.SymmetricExceptWith(selecting);
		foreach (var unit in currentSelect)
		{
			if (!unit.IsExists())
			{
				selecting.Remove(unit);
				continue;
			}

			if (selecting.Contains(unit))
			{
				selecting.Remove(unit);
			}
			else
			{
				selecting.Add(unit);
			}
			unit.Selected = !unit.Selected;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		selecting.SymmetricExceptWith(selected);
		foreach (var unit in selecting)
		{
			if (!unit.IsExists())
			{
				selected.Remove(unit);
				continue;
			}

			if (selected.Contains(unit))
			{
				selected.Remove(unit);
			}
			else
			{
				selected.Add(unit);
			}
		}

		selectFrame.enabled = false;
	}
}
