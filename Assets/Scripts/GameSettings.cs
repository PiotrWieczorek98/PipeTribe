using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum KeyMap { Action1, Action2, Start, Stop};

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        KeyBindings = new Dictionary<KeyMap, KeyCode>();
        KeyBindings.Add(KeyMap.Action1, KeyCode.Space);
        KeyBindings.Add(KeyMap.Action2, KeyCode.C);
        KeyBindings.Add(KeyMap.Start, KeyCode.P);
        KeyBindings.Add(KeyMap.Stop, KeyCode.S);

        LevelDir = Application.streamingAssetsPath + "/levels";
    }

    public KeyCode GetKeyBind(KeyMap bind)
    {
        return KeyBindings[bind];
    }
    public Dictionary<KeyMap, KeyCode> KeyBindings { get; private set; }
    public string LevelDir { get; private set; }
    public float MusicNotesOffset { get; private set; } = 0;
}
