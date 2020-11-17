using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum KeyMap { Action};

    Dictionary<KeyMap, KeyCode> keyBindings;

    private void Awake()
    {
        keyBindings = new Dictionary<KeyMap, KeyCode>();
        keyBindings.Add(KeyMap.Action, KeyCode.Space);
    }

    public KeyCode GetKeyBind(KeyMap bind)
    {
        return keyBindings[bind];
    }
    public Dictionary<KeyMap, KeyCode> KeyBindings { get { return keyBindings; } }
}
