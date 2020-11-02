using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string levelName;
    public AudioClip music;
    public float MusicNotesOffset;
    // Minimum time between notes to allow change of segment the note is placed on
    public float minDeltaTimeToSwitch;

    public float loadingTime;
    float timeSinceLoaded = 0;
    bool finishedLoading = false;

    AudioSource audioSource;
    UIComponents uIComponents;
    Text UItimer;

    PipeSystem pipeSystem;
    Player player;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        uIComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
        UItimer = uIComponents.timer;

        audioSource = GetComponent(typeof(AudioSource)) as AudioSource;
    }

    void Start()
    {
        pipeSystem = GameObject.FindGameObjectWithTag("PipeSystemManager").GetComponent(typeof(PipeSystem)) as PipeSystem;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(Player)) as Player;

        StartCoroutine(Loading(loadingTime));
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceLoaded += Time.deltaTime;
        UItimer.text = timeSinceLoaded.ToString();
    }

    // This method is required to avoid desync and lag at the beggining of play when entities still load;
    public IEnumerator Loading(float delay)
    {
        // Disable player to stop movement of the pipe system
        player.enabled = false;
        yield return new WaitForSeconds(delay);
        // Set everything ready to start
        player.enabled = true;
        audioSource.PlayOneShot(music);
        timeSinceLoaded = 0;
        finishedLoading = true;
    }

    public bool FinishedLoading { get { return finishedLoading; } }
    public float TimeSinceLoaded { get { return timeSinceLoaded; } }
}
