using UnityEngine;

public class TapRecorder : MonoBehaviour
{
    KeyCode actionKey;
    KeyCode startKey;
    KeyCode stopKey;

    float timeHeld = 0;
    float timeWhenPressed = 0;

    bool isRecording = false;
    AudioSource audioSource;
    TimelineIndicator timelineIndicator;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        timelineIndicator = GetComponentInChildren<TimelineIndicator>();
    }

    private void Start()
    {
        GameSettings gameSettings = FindObjectOfType<GameSettings>();
        actionKey = gameSettings.GetKeyBind(GameSettings.KeyMap.Action2);
        startKey = gameSettings.GetKeyBind(GameSettings.KeyMap.Start);
        stopKey = gameSettings.GetKeyBind(GameSettings.KeyMap.Stop);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(startKey) && !isRecording)
        {
            isRecording = true;
            audioSource.Play();
        }

        else if (Input.GetKeyDown(stopKey) && isRecording)
        {
            isRecording = false;
            audioSource.Pause();
        }


        if (Input.GetKeyDown(actionKey))
        {
            timeWhenPressed = audioSource.time;
        }
        else if (Input.GetKeyUp(actionKey))
        {
            timeHeld = audioSource.time - timeWhenPressed;
            if (timeHeld < 0.5)
                timeHeld = 0;

            timelineIndicator.addNoteObject(timeWhenPressed, timeHeld);
            Debug.Log(timeWhenPressed + " / " + timeHeld);
        }
    }
}
