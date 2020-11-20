using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEditorButton : MonoBehaviour
{
    TimelineIndicator timelineIndicator;

    public enum TypeOfButton { Load, Save, ZoomIn, ZoomOut, ResetZoom, Exit };
    public TypeOfButton typeOfButton;

    public Sprite defaultSprite, clickedSprite, hoverSprite;
    Image buttonImage;

    KeyCode actionKey;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;

        timelineIndicator = FindObjectOfType<TimelineIndicator>();
    }

    private void Start()
    {
        actionKey = FindObjectOfType<GameSettings>().GetKeyBind(GameSettings.KeyMap.Action1);
    }

    private void OnMouseOver()
    {
        if(buttonImage.sprite == defaultSprite)
            buttonImage.sprite = hoverSprite;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionKey))
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(ClickedButtonAnimation());

            string levelName = FindObjectOfType<InputField>().text;
            if (levelName.Length == 0)
                levelName = "Untitled";

            float newZoom, zoomCenter;
            LevelDataPasser levelDataPasser = FindObjectOfType<LevelDataPasser>();
            switch (typeOfButton)
            {
                case TypeOfButton.Save:
                    levelDataPasser.SaveRecordingToDat(levelName, timelineIndicator);
                    levelDataPasser.SaveRecordingToTxt(levelName, timelineIndicator);
                    break;

                case TypeOfButton.Load:
                    // Clear timeline befor reloading
                    timelineIndicator.DestroyAllNoteObjects();
                    levelDataPasser.LoadRecording(levelName, timelineIndicator);
                    break;
                case TypeOfButton.ResetZoom:
                    timelineIndicator.ZoomTimeline(-1, Screen.width / 2, Screen.width);
                    break;
                case TypeOfButton.ZoomIn:
                    newZoom = (int)(timelineIndicator.CurrentZoom * 0.75);
                    newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
                    zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;
                    timelineIndicator.ZoomTimeline(1, zoomCenter, newZoom);
                    break;
                case TypeOfButton.ZoomOut:
                    newZoom = (int)(timelineIndicator.CurrentZoom * 1.25);
                    newZoom = Mathf.Clamp(newZoom, 0, Screen.width);
                    zoomCenter = timelineIndicator.ArrowIndicator.anchoredPosition.x + timelineIndicator.TimelineCanvasRadius;

                    timelineIndicator.ZoomTimeline(-1, zoomCenter, newZoom);
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

    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }


}
