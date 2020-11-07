using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTimelineIndicator : MonoBehaviour
{
    float timelinePosition;
    float timestamp;
    float timeHeld;
    float deltaX;
    float mouseAnchoredPosition;
    RectTransform rectTransform;
    TimelineIndicator timelineIndicator;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        timelineIndicator = transform.parent.GetComponent<TimelineIndicator>();
    }

    private void OnMouseDown()
    {
        mouseAnchoredPosition = Remap(Input.mousePosition.x, 0, Screen.width, 
                        -timelineIndicator.TimelineCanvasRadius, timelineIndicator.TimelineCanvasRadius);

        deltaX = mouseAnchoredPosition - rectTransform.anchoredPosition.x;
    }

    void OnMouseDrag()
    {
        timelinePosition = Input.mousePosition.x - deltaX;
        timelinePosition = Remap(timelinePosition, 0, Screen.width, timelineIndicator.LeftBorder, timelineIndicator.RightBorder);
        timestamp = Remap(timelinePosition, 0, Screen.width, 0, transform.root.GetComponent<AudioSource>().clip.length);

        timelineIndicator.updateSingleNotePosition(transform);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public float TimelinePosition { get { return timelinePosition; } set { timelinePosition = value; } }
    public float Timestamp { get { return timestamp; } set { timestamp = value; } }
    public float TimeHeld { get { return timeHeld; } set { timeHeld = value; } }
}
