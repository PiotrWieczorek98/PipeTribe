using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelCreatorButton : MonoBehaviour
{
    TimelineIndicator timelineIndicator;
    LevelDataPasser levelDataPasser;

    public enum TypeOfButton { Load, Save, ZoomIn, ZoomOut, ResetZoom, Exit };
    public TypeOfButton typeOfButton;
    public Sprite defaultSprite, clickedSprite, hoverSprite;
    Image buttonImage;

    KeyCode actionKey;
    LevelCreatorManager levelCreatorManager;
    private void Awake()
    {
        levelCreatorManager = FindObjectOfType<LevelCreatorManager>();
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;

        timelineIndicator = FindObjectOfType<TimelineIndicator>();
        levelDataPasser = FindObjectOfType<LevelDataPasser>();
    }

    private void Start()
    {
        actionKey = FindObjectOfType<GameSettings>().GetKeyBind(GameSettings.KeyMap.Action1);
    }

    private void OnMouseOver()
    {
        if(buttonImage.sprite == defaultSprite)
            buttonImage.sprite = hoverSprite;

        // If mouse or key was clicked
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionKey))
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(ClickedButtonAnimation());

            string levelName = levelCreatorManager.MusicName;
            if (levelName == null || levelName == "")
                return;
            else
                levelName = levelName.ToLower();
            
            switch (typeOfButton)
            {
                case TypeOfButton.Save:
                    if (levelCreatorManager.MusicLoaded)
                    {
                        levelDataPasser.SaveRecordingToDat(levelName, timelineIndicator.NotesObjects);
                        levelDataPasser.SaveRecordingToTxt(levelName, timelineIndicator.NotesObjects);
                    }
                    break;
                case TypeOfButton.Load:
                    StartCoroutine(LoadMusic(levelName));
                    break;
                case TypeOfButton.ResetZoom:
                    if (levelCreatorManager.MusicLoaded)
                        timelineIndicator.ZoomTimeline(-1, Screen.width / 2, Screen.width);
                    break;
                case TypeOfButton.ZoomIn:
                    if (levelCreatorManager.MusicLoaded)
                        ZoomIn();
                    break;
                case TypeOfButton.ZoomOut:
                    if (levelCreatorManager.MusicLoaded)
                        ZoomOut();
                    break;
                case TypeOfButton.Exit:
                    SceneManager.LoadScene(0);
                    break;
            }
        }
    }

    IEnumerator ClickedButtonAnimation()
    {
        buttonImage.sprite = clickedSprite;
        yield return new WaitForSeconds(.5f);
        buttonImage.sprite = defaultSprite;
    }

    IEnumerator LoadMusic(string levelName)
    {
        WaveformDrawer waveformDrawer = FindObjectOfType<WaveformDrawer>();

        // Load music requires coroutine
        StartCoroutine(levelCreatorManager.LoadMp3File(levelName));
        // Wait for music to load
        if (!levelCreatorManager.MusicLoaded)
            yield return new WaitForSeconds(.1f);
        // Check if error occured
        if (levelCreatorManager.MusicSource.clip == null)
            yield break;

        // Clear timeline if music is loaded not for the first time
        if (timelineIndicator.enabled)
        {
            timelineIndicator.DestroyAllNoteObjects();
            timelineIndicator.ChangeCurrentAudioClipTime(0);
        }
        // Initialize timeline if music is loaded for the first time
        else
            waveformDrawer.InitializeTimelineSettings();

        // Draw timeline
        waveformDrawer.DrawTimeline();

        // Set bpm bars
        timelineIndicator.SetBeatIndicators(levelCreatorManager.BeatsTotal, levelCreatorManager.OffsetValue);

        // load data if such exist
        //List<(float, float)> noteTuples = levelDataPasser.LoadRecordingFromDat(levelName);
        List<(float, float)> noteTuples = levelDataPasser.LoadRecordingFromTxt(levelName);
        if (noteTuples == null)
            yield break;
        foreach ((float, float) noteTuple in noteTuples)
        {
            timelineIndicator.addNoteObject(noteTuple.Item1, noteTuple.Item2);
        }
    }

    void ZoomOut()
    {
        float newZoom = (int)(timelineIndicator.CurrentZoom * 1.25);
        newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
        float zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;

        timelineIndicator.ZoomTimeline(-1, zoomCenter, newZoom);
    }

    void ZoomIn()
    {
        float newZoom = (int)(timelineIndicator.CurrentZoom * 0.75);
        newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
        float zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;

        timelineIndicator.ZoomTimeline(1, zoomCenter, newZoom);
    }

    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }


}
