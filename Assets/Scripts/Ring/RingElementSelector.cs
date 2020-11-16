using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RingElementSelector : MonoBehaviour
{
    public enum RingElementEnum { None = -1, TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
    public RingElementEnum ringElement;
    public Sprite normalSprite;
    public Sprite highlitedSprite;

    Image segmentImage;
    RingManager ringManager;
    Vector3 initialPosition;
    bool isSelected = false;

    // Start is called before the first frame update
    void Awake()
    {
        segmentImage = GetComponent<Image>();
        ringManager = transform.parent.GetComponent< RingManager>();
        initialPosition = transform.localPosition;
    }

    void OnMouseEnter()
    {
        if(!isSelected)
        {
            SetElementSelection(true);
            ringManager.ChangeSelectedPart(this);
        }
    }

    public void SetElementSelection(bool isSelected)
    {
        Vector3 offsetPosition = transform.localPosition;
        if (isSelected)
        {
            // Move element a little further from center
            offsetPosition.x *= 1.03f;
            offsetPosition.y *= 1.06f;

            segmentImage.sprite = highlitedSprite;
            this.isSelected = true;
        }
        else
        {
            // Move element back to original position
            offsetPosition = initialPosition;

            segmentImage.sprite = normalSprite;
            this.isSelected = false;
        }

        transform.localPosition = offsetPosition;
    }

    public string RingElement() { return Enum.GetName(typeof(RingElementEnum), ringElement); }
    public bool IsSelected() { return isSelected; }
}
