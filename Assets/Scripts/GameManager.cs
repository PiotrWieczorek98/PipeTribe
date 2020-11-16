using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string levelName;
    public AudioClip music;
    public AudioClip levelComplete;
    public AudioClip levelFailed;
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
    Text currentTimeUI;
    Text totalTimeUI;
    RectTransform currentTimeBar;

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

        currentTimeUI = uiComponents.Timer.GetChild(0).GetComponent<Text>();
        totalTimeUI = uiComponents.Timer.GetChild(1).GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        currentTimeBar = uiComponents.TimerBar;
    }

    void Start()
    {
        StartCoroutine(FinishLoading(loadingTime));
        totalTimeUI.text = music.length.ToString("F0") + " sec";
    }


    // Update is called once per frame
    void Update()
    {
        float timePassedPercent = timeSinceLoaded / music.length;
        currentTimeBar.localScale = new Vector3(timePassedPercent,1,1);
        
        timeSinceLoaded += Time.deltaTime;
        currentTimeUI.text = timeSinceLoaded.ToString("F0");

        if(timeSinceLoaded > music.length)
            EndGameScreen();
    }

    // This method is required to avoid desync and lag at the beggining of play when entities still load;
    public IEnumerator FinishLoading(float delay)
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

    public void EndGameScreen(bool failed = false)
    {
        //player.enabled = false;
        if (failed)
        {
            audioSource.PlayOneShot(levelFailed);
        }
        else
        {
            audioSource.PlayOneShot(levelComplete);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public UIComponents UIcomponents { get { return uiComponents; } }
    public RingManager RingManager { get { return ringManager; } }

    public bool FinishedLoading { get { return finishedLoading; } }
    public float TimeSinceLoaded { get { return timeSinceLoaded; } }
}
