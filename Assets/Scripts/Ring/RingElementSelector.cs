using UnityEditor;
using UnityEngine;

public class RingElementSelector : MonoBehaviour
{
    public enum RingPart { None = -1, TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
    public RingPart ringPart;
    public Sprite normalSprite;
    public Sprite highlitedSprite;

    SpriteRenderer spriteRenderer;
    RingManager parent;
    Vector3 initialPosition;
    bool isSelected = false;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent = transform.parent.GetComponent< RingManager>();
        initialPosition = transform.localPosition;
    }

    void OnMouseEnter()
    {
        if(!isSelected)
        {
            SetElementSelection(true);
            parent.ChangeSelectedPart((int)ringPart);
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

            spriteRenderer.sprite = highlitedSprite;
            this.isSelected = true;
        }
        else
        {
            // Move element back to original position
            offsetPosition = initialPosition;

            spriteRenderer.sprite = normalSprite;
            this.isSelected = false;
        }

        transform.localPosition = offsetPosition;
    }

    public int GetRingPart() { return (int)ringPart; }
    public bool IsSelected() { return isSelected; }
}
