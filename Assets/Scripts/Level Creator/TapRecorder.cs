using UnityEngine;

public class TapRecorder : MonoBehaviour
{
    KeyCode tapKey;
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
        timelineIndicator = FindObjectOfType<TimelineIndicator>();
    }

    private void Start()
    {
        GameSettings gameSettings = FindObjectOfType<GameSettings>();
        tapKey = gameSettings.GetBindedKey(GameSettings.KeyType.Tap);
        startKey = gameSettings.GetBindedKey(GameSettings.KeyType.Start);
        stopKey = gameSettings.GetBindedKey(GameSettings.KeyType.Stop);
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


        if (Input.GetKeyDown(tapKey))
        {
            timeWhenPressed = audioSource.time;
        }
        else if (Input.GetKeyUp(tapKey))
        {
            timeHeld = audioSource.time - timeWhenPressed;
            if (timeHeld < 0.5)
                timeHeld = 0;

            timelineIndicator.addNoteObject(timeWhenPressed, timeHeld);
            Debug.Log(timeWhenPressed + " / " + timeHeld);
        }
    }
}
