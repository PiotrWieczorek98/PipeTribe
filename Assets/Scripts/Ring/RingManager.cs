using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    public enum SelectedElement {None = -1, TopRight, Right, BottomRight, BottomLeft, Left, TopLeft };
    public SelectedElement selectedElement;

    Dictionary<int, Transform> ringElements;
    //MoveBackground bg;

    void Start()
    {
        ringElements = new Dictionary<int, Transform>();
        int childCnt = transform.childCount;
        for (int i = 0; i < childCnt - 2; i++)
        {
            Transform childX = transform.GetChild(i);
            ringElements.Add(i, childX.transform);
        }

        //bg = GameObject.FindGameObjectWithTag("Background").GetComponent(typeof(MoveBackground)) as MoveBackground;
    }

    public void ChangeSelectedPart(int newSelectedPart)
    {
        if(selectedElement != SelectedElement.None)
        {
            RingElementSelector res = ringElements[(int)selectedElement].GetComponent(typeof(RingElementSelector)) as RingElementSelector;
            res.HighlightPart(false);
        }
        selectedElement = (SelectedElement)newSelectedPart;
        //bg.SetDestination(ringElements[(int)selectedElement].position);
    }

    public Dictionary<int, Transform> GetRingElements() { return ringElements; }
    public Transform GetRingElement(int i) 
    {
        if (i < ringElements.Count)
            return ringElements[i];
        
        return null;
    }

    public SelectedElement GetSelectedElement { get { return selectedElement; } }

}
