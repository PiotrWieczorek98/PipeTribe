using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineIndicator : MonoBehaviour
{
    ButtonRecorder buttonRecorder;
    AudioSource audioSource;
    RectTransform timeline, arrowIndicator; //canvasTransform;
    WaveformDrawer waveformDrawer;
    public float zoomSpeed;
    public float smoothSpeed;
    float currentZoom, minZoom, maxZoom;
    float leftBorder;
    float rightBorder;

    float timelineCanvasRadius;

    public Transform notePrefab;
    List<(Transform, float)> notesObjects;

    private void Awake()
    {
        buttonRecorder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ButtonRecorder>();
        audioSource = buttonRecorder.GetComponent<AudioSource>();
        timeline = transform.GetComponent<RectTransform>();
        arrowIndicator = transform.GetChild(0).GetComponent<RectTransform>();
        waveformDrawer = GetComponent<WaveformDrawer>();
        
        currentZoom = Screen.width;
        leftBorder = minZoom = 0f;
        rightBorder = maxZoom = Screen.width;
        timelineCanvasRadius = timeline.rect.width / 2;

        notesObjects = new List<(Transform, float)>();
    }

    // Update is called once per frame
    void Update()
    {
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

    public void addNoteObject(float timestamp)
    {
        timestamp = Remap(timestamp, 0, audioSource.clip.length, 0, Screen.width);
        notesObjects.Add((Instantiate(notePrefab, transform), timestamp));
        updateNotesPosition();
    }

    void updateNotesPosition()
    {
        foreach (var note in notesObjects)
        {
            float position = note.Item2;
            Transform noteObject = note.Item1;

            if (position > leftBorder && position < rightBorder)
            {
                position = Remap(position, leftBorder, rightBorder, -timelineCanvasRadius, timelineCanvasRadius);
                noteObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, 15);
                noteObject.gameObject.SetActive(true);
            }
            else
            {
                noteObject.gameObject.SetActive(false);
            }
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

    // Zoom in and out the time line
    private void OnMouseOver()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float newZoom = currentZoom;
            newZoom -= scroll * zoomSpeed;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            newZoom = Mathf.MoveTowards(currentZoom, newZoom, smoothSpeed * Time.deltaTime);

            float mouseRelPos = Remap(Input.mousePosition.x, 0, Screen.width, leftBorder, rightBorder);
            float oldLeftBorder = leftBorder;
            float oldRightBorder = rightBorder;
            leftBorder = mouseRelPos - newZoom / 2;
            rightBorder = mouseRelPos + newZoom / 2;

            if(scroll > 0)
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

            updateNotesPosition();
        }
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
