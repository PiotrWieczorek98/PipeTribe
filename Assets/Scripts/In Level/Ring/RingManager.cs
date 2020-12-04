using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
	public RingElementSelector selectedElement;

	Dictionary<int, Transform> ringElements;
	//MoveBackground bg;

	void Awake()
	{
		ringElements = new Dictionary<int, Transform>();
		int childCnt = transform.childCount;
		for (int i = 0; i < childCnt - 2; i++)
		{
			Transform childX = transform.GetChild(i);
			ringElements.Add(i, childX.transform);
		}

		selectedElement = null;
		//bg = GameObject.FindGameObjectWithTag("Background").GetComponent(typeof(MoveBackground)) as MoveBackground;
	}

	public void ChangeSelectedPart(RingElementSelector newSelectedPart)
	{
		if (selectedElement != null)
			selectedElement.SetElementSelection(false);

		selectedElement = newSelectedPart;
		//bg.SetDestination(ringElements[(int)selectedElement].position);
	}

	public Dictionary<int, Transform> GetRingElements() { return ringElements; }
	public Transform GetRingElement(int i)
	{
		if (i < ringElements.Count)
			return ringElements[i];

		return null;
	}

	public RingElementSelector SelectedRingElement { get { return selectedElement; } }

}
