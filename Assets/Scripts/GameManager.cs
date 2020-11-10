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
    public float healthRegainOnHit;
    public float healthLossOnFail;

    public float loadingTime;
    float timeSinceLoaded = 0;
    bool finishedLoading = false;

    AudioSource audioSource;
    UIComponents uiComponents;
    Text uiTimer;

    PipeSystem pipeSystem;
    Player player;
    RingManager ringManager;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        uiComponents = GameObject.FindGameObjectWithTag("UI").GetComponent(typeof(UIComponents)) as UIComponents;
        ringManager = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren(typeof(RingManager)) as RingManager;
        pipeSystem = GameObject.FindGameObjectWithTag("PipeSystemManager").GetComponent(typeof(PipeSystem)) as PipeSystem;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(Player)) as Player;
        uiTimer = uiComponents.Timer;

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(Loading(loadingTime));
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceLoaded += Time.deltaTime;
        uiTimer.text = timeSinceLoaded.ToString("F2") + " / " + music.length.ToString("F2");
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
    public UIComponents UIcomponents { get { return uiComponents; } }
    public RingManager RingManager { get { return ringManager; } }

    public bool FinishedLoading { get { return finishedLoading; } }
    public float TimeSinceLoaded { get { return timeSinceLoaded; } }
}
