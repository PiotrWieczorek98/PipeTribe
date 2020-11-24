using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTimelineIndicator : MonoBehaviour
{
    public float TimelinePosition { get; set; }
    public float Timestamp { get; set; }
    public float TimeHeld { get; set; }

    float deltaX;
    float mouseAnchoredPosition;
    RectTransform rectTransform;
    TimelineIndicator timelineIndicator;
    AudioSource audioSource;

    KeyCode action1, action2;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        timelineIndicator = transform.parent.GetComponent<TimelineIndicator>();
        audioSource = FindObjectOfType<LevelCreatorManager>().MusicSource;

        GameSettings gameSettings = FindObjectOfType<GameSettings>();
        action1 = gameSettings.GetKeyBind(GameSettings.KeyMap.Action1);
        action2 = gameSettings.GetKeyBind(GameSettings.KeyMap.Action2);
    }

    void OnMouseDrag()
    {
        TimelinePosition = Input.mousePosition.x - deltaX;
        TimelinePosition = Remap(TimelinePosition, 0, Screen.width, timelineIndicator.LeftBorder, timelineIndicator.RightBorder);
        Timestamp = Remap(TimelinePosition, 0, Screen.width, 0, audioSource.clip.length);

        timelineIndicator.updateSingleNotePosition(transform);
    }

    private void OnMouseOver()
    {
        // Anchor position on action1 key
        if (Input.GetKeyDown(action1))
        {
            mouseAnchoredPosition = Remap(Input.mousePosition.x, 0, Screen.width,
                -timelineIndicator.TimelineCanvasRadius, timelineIndicator.TimelineCanvasRadius);

            deltaX = mouseAnchoredPosition - rectTransform.anchoredPosition.x;
        }
        // Delete object on action2 key
        else if (Input.GetKeyDown(action2))
        {
            Destroy(transform.gameObject);
        }
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
