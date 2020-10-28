using UnityEditor;
using UnityEngine;

public class RingElementSelector : MonoBehaviour
{
    public enum RingPart { None = -1, TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
    public RingPart ringPart;
    public Sprite normalSprite;
    public Sprite highlitedSprite;
    SpriteRenderer sr;
    RingManager parent;
    bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        parent = transform.parent.GetComponent(typeof(RingManager)) as RingManager;
    }

    void OnMouseOver()
    {
        if(!isSelected)
        {
            HighlightPart(true);
            parent.ChangeSelectedPart((int)ringPart);
        }
    }
    //void OnMouseExit()
    //{
    //    sr.sprite = normalSprite;
    //}
    public void HighlightPart(bool highlight)
    {
        if(highlight)
        {
            sr.sprite = highlitedSprite;
            isSelected = true;
        }
        else
        {
            sr.sprite = normalSprite;
            isSelected = false;
        }

    }

    public int GetRingPart() { return (int)ringPart; }
    public bool IsSelected() { return isSelected; }
}
