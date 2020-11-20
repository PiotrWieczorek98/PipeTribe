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
    Text UItimer;
    KeyCode actionKey;

    public float zoomSpeed;
    public float smoothSpeed;
    float minZoom, maxZoom;
    public Transform notePrefab;

    private void Awake()
    {
        buttonRecorder = FindObjectOfType<TapRecorder>();

        audioSource = buttonRecorder.GetComponent<AudioSource>();
        timeline = transform.GetComponent<RectTransform>();
        arrowIndicator = transform.GetChild(0).GetComponent<RectTransform>();
        waveformDrawer = GetComponent<WaveformDrawer>();
        UItimer = FindObjectOfType<UIComponents>().Timer.GetComponent<Text>();

        UItimer.text = "0.0000 / " + audioSource.clip.length + "sec";
        CurrentZoom = Screen.width;
        LeftBorder = minZoom = 0f;
        RightBorder = maxZoom = Screen.width;
        TimelineCanvasRadius = timeline.rect.width / 2;
        updateIndicatorPosition();



        NotesObjects = new List<Transform>();
    }

    private void Start()
    {
        actionKey = FindObjectOfType<GameSettings>().GetKeyBind(GameSettings.KeyMap.Action1);
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
        if (newPosition >= LeftBorder && newPosition <= RightBorder)
        {
            arrowIndicator.gameObject.SetActive(true);
            newPosition = Remap(newPosition, LeftBorder, RightBorder, -TimelineCanvasRadius, TimelineCanvasRadius);

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

        NotesObjects.Add(newNoteObject);
        updateSingleNotePosition(newNoteObject);
    }

    public void updateAllNotesPosition()
    {
        foreach (var note in NotesObjects)
        {
            updateSingleNotePosition(note);
        }
    }

    public void updateSingleNotePosition(Transform noteObject)
    {

            float position = noteObject.GetComponent<NoteTimelineIndicator>().TimelinePosition;

            if (position > LeftBorder && position < RightBorder)
            {
                position = Remap(position, LeftBorder, RightBorder, -TimelineCanvasRadius, TimelineCanvasRadius);
                RectTransform rectTransform = noteObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(position, rectTransform.anchoredPosition.y);
                noteObject.gameObject.SetActive(true);
            }
            else
            {
                noteObject.gameObject.SetActive(false);
            }
    }

    private void OnMouseDragOrButtonDown()
    {
        // Set indicator and audio player to clicked position
        float mousePos = Input.mousePosition.x;
        // Remap to include zoom level to zoom
        mousePos = Remap(mousePos, 0, Screen.width, LeftBorder, RightBorder);
        // Remap to audio time
        mousePos = Remap(mousePos, 0, Screen.width, 0, audioSource.clip.length);

        audioSource.time = mousePos;
        updateIndicatorPosition();
    }

    private void OnMouseOver()
    {
        // Zoom on scroll
        float zoomDirection = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDirection != 0.0f)
        {
            float zoomCenter = Remap(Input.mousePosition.x, 0, Screen.width, LeftBorder, RightBorder);

            float newZoom = CurrentZoom;
            newZoom -= zoomDirection * zoomSpeed;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            newZoom = Mathf.MoveTowards(CurrentZoom, newZoom, smoothSpeed * Time.deltaTime);

            ZoomTimeline(zoomDirection, zoomCenter, newZoom);
        }

        // Update indicator on mouse or button down
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(actionKey))
            OnMouseDragOrButtonDown();
    }

    // Zoom in and out the time line
    public void ZoomTimeline(float zoomDirection, float zoomCenter, float newZoom)
    {
        float oldLeftBorder = LeftBorder;
        float oldRightBorder = RightBorder;
        LeftBorder = zoomCenter - newZoom / 2;
        RightBorder = zoomCenter + newZoom / 2;

        if (zoomDirection > 0)
        {
            if (LeftBorder < oldLeftBorder)
            {
                RightBorder += oldLeftBorder - LeftBorder;
                LeftBorder = oldLeftBorder;
            }
            if (RightBorder > oldRightBorder)
            {
                LeftBorder -= RightBorder - oldRightBorder;
                RightBorder = oldRightBorder;
            }
        }
        else
        {
            if (LeftBorder < 0)
            {
                RightBorder += oldLeftBorder - LeftBorder;
                LeftBorder = 0;
            }
            if (RightBorder > Screen.width)
            {
                LeftBorder -= RightBorder - oldRightBorder;
                RightBorder = Screen.width;
            }
        }

        LeftBorder = Mathf.Clamp(LeftBorder, 0, Screen.width);
        RightBorder = Mathf.Clamp(RightBorder, 0, Screen.width);
        CurrentZoom = RightBorder - LeftBorder;
        Texture2D zoomedTexture = waveformDrawer.CreateWaveformSpectrumTexture((int)LeftBorder, (int)RightBorder);
        waveformDrawer.OverrideSprite(zoomedTexture);
        CurrentZoom = newZoom;

        updateAllNotesPosition();
        updateIndicatorPosition();
    }

    public void DestroyAllNoteObjects()
    {
        foreach(Transform noteObject in NotesObjects)
        {
            GameObject.Destroy(noteObject.gameObject);
        }
        NotesObjects.Clear();
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public float TimelineCanvasRadius { get; private set; }
    public float LeftBorder { get; private set; }
    public float RightBorder { get; private set; }
    public float CurrentZoom { get; private set; }
    public RectTransform ArrowIndicator { get { return arrowIndicator; } }
    public List<Transform> NotesObjects { get; set; }
}
