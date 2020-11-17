using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineIndicator : MonoBehaviour
{
    TapRecorder buttonRecorder;
    AudioSource audioSource;
    RectTransform timeline, arrowIndicator; //canvasTransform;
    WaveformDrawer waveformDrawer;
    UIComponents uIComponents;
    Text UItimer;

    public float zoomSpeed;
    public float smoothSpeed;
    float currentZoom, minZoom, maxZoom;
    float leftBorder;
    float rightBorder;

    float timelineCanvasRadius;

    public Transform notePrefab;
    List<Transform> notesObjects;

    private void Awake()
    {
        buttonRecorder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TapRecorder>();
        uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent<UIComponents>();

        audioSource = buttonRecorder.GetComponent<AudioSource>();
        timeline = transform.GetComponent<RectTransform>();
        arrowIndicator = transform.GetChild(0).GetComponent<RectTransform>();
        waveformDrawer = GetComponent<WaveformDrawer>();
        
        currentZoom = Screen.width;
        leftBorder = minZoom = 0f;
        rightBorder = maxZoom = Screen.width;
        timelineCanvasRadius = timeline.rect.width / 2;
        updateIndicatorPosition();

        UItimer = uIComponents.Timer.GetChild(0).GetComponent<Text>();
        UItimer.text = "0.0000 / " + audioSource.clip.length + "sec";

        notesObjects = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        UItimer.text = audioSource.time.ToString("F4") + " / " + audioSource.clip.length.ToString("F4") + " sec";

        if (audioSource.isPlaying)
            updateIndicatorPosition();
    }

    void updateIndicatorPosition()
    {
        // Update indicator position
        float newPosition = Remap(audioSource.time, 0, audioSource.clip.length, 0, Screen.width);
        if (newPosition >= leftBorder && newPosition <= rightBorder)
        {
            arrowIndicator.gameObject.SetActive(true);
            newPosition = Remap(newPosition, leftBorder, rightBorder, -timelineCanvasRadius, timelineCanvasRadius);

            arrowIndicator.anchoredPosition = new Vector2(newPosition, arrowIndicator.anchoredPosition.y);
        }
        else
            arrowIndicator.gameObject.SetActive(false);

        // Toggle play/pause to register change - otherwise indicator wouldn't move until audio was played
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.Pause();
        }
    }

    public void addNoteObject(float timestamp, float timeHeld)
    {
        Transform newNoteObject = Instantiate(notePrefab, transform);

        float timelinePosition = Remap(timestamp, 0, audioSource.clip.length, 0, Screen.width);

        NoteTimelineIndicator noteTimelineIndicator = newNoteObject.GetComponent<NoteTimelineIndicator>();
        noteTimelineIndicator.TimelinePosition = timelinePosition;
        noteTimelineIndicator.Timestamp = timestamp;
        noteTimelineIndicator.TimeHeld = timeHeld;

        notesObjects.Add(newNoteObject);
        updateSingleNotePosition(newNoteObject);
    }

    public void updateAllNotesPosition()
    {
        foreach (var note in notesObjects)
        {
            updateSingleNotePosition(note);
        }
    }

    public void updateSingleNotePosition(Transform noteObject)
    {

            float position = noteObject.GetComponent<NoteTimelineIndicator>().TimelinePosition;

            if (position > leftBorder && position < rightBorder)
            {
                position = Remap(position, leftBorder, rightBorder, -timelineCanvasRadius, timelineCanvasRadius);
                RectTransform rectTransform = noteObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(position, rectTransform.anchoredPosition.y);
                noteObject.gameObject.SetActive(true);
            }
            else
            {
                noteObject.gameObject.SetActive(false);
            }
    }

    private void OnMouseDown()
    {
        // Set indicator and audio player to clicked position
        float mousePos = Input.mousePosition.x;
        // Remap to include zoom level to zoom
        mousePos = Remap(mousePos, 0, Screen.width, leftBorder, rightBorder);
        // Remap to audio time
        mousePos = Remap(mousePos, 0, Screen.width, 0, audioSource.clip.length);

        audioSource.time = mousePos;
        updateIndicatorPosition();
    }

    // Zoom on scroll
    private void OnMouseOver()
    {
        float zoomDirection = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDirection != 0.0f)
        {
            float zoomCenter = Remap(Input.mousePosition.x, 0, Screen.width, leftBorder, rightBorder);

            float newZoom = currentZoom;
            newZoom -= zoomDirection * zoomSpeed;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            newZoom = Mathf.MoveTowards(currentZoom, newZoom, smoothSpeed * Time.deltaTime);

            ZoomTimeline(zoomDirection, zoomCenter, newZoom);
        }
    }

    // Zoom in and out the time line
    public void ZoomTimeline(float zoomDirection, float zoomCenter, float newZoom)
    {
        float oldLeftBorder = leftBorder;
        float oldRightBorder = rightBorder;
        leftBorder = zoomCenter - newZoom / 2;
        rightBorder = zoomCenter + newZoom / 2;

        if (zoomDirection > 0)
        {
            if (leftBorder < oldLeftBorder)
            {
                rightBorder += oldLeftBorder - leftBorder;
                leftBorder = oldLeftBorder;
            }
            if (rightBorder > oldRightBorder)
            {
                leftBorder -= rightBorder - oldRightBorder;
                rightBorder = oldRightBorder;
            }
        }
        else
        {
            if (leftBorder < 0)
            {
                rightBorder += oldLeftBorder - leftBorder;
                leftBorder = 0;
            }
            if (rightBorder > Screen.width)
            {
                leftBorder -= rightBorder - oldRightBorder;
                rightBorder = Screen.width;
            }
        }

        leftBorder = Mathf.Clamp(leftBorder, 0, Screen.width);
        rightBorder = Mathf.Clamp(rightBorder, 0, Screen.width);
        currentZoom = rightBorder - leftBorder;
        Texture2D zoomedTexture = waveformDrawer.CreateWaveformSpectrumTexture((int)leftBorder, (int)rightBorder);
        waveformDrawer.OverrideSprite(zoomedTexture);
        currentZoom = newZoom;

        updateAllNotesPosition();
        updateIndicatorPosition();
    }

    public void DestroyAllNoteObjects()
    {
        foreach(Transform noteObject in notesObjects)
        {
            GameObject.Destroy(noteObject.gameObject);
        }
        notesObjects.Clear();
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public float TimelineCanvasRadius { get { return timelineCanvasRadius; } }
    public float LeftBorder { get { return leftBorder; } }
    public float RightBorder { get { return rightBorder; } }
    public float CurrentZoom { get { return currentZoom; } }
    public RectTransform ArrowIndicator { get { return arrowIndicator; } }
    public List<Transform> NotesObjects { get { return notesObjects; } set { notesObjects = value; } }
}
