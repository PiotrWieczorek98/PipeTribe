using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyInputField : MonoBehaviour
{
    public GameSettings.KeyType keyType;
    public Sprite defaultSprite;
    public Sprite clickedSprite;
    public Sprite hoverSprite;

    Image image;
    Text text;

    bool waitingForInput = false;
    bool actionKeyUpdated = false;
    KeyCode action1;
    GameSettings gameSettings;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<Text>();
        gameSettings = FindObjectOfType<GameSettings>();
    }

    private void Start()
    {
        action1 = gameSettings.GetBindedKey(GameSettings.KeyType.Action1);
        text.text = gameSettings.GetBindedKey(keyType).ToString();

    }

    private void OnMouseOver()
    {
        if (waitingForInput)
            return;

        if(image.sprite == defaultSprite)
            image.sprite = hoverSprite;
        
        // Activate edit when key was pressed
        if (Input.GetKeyDown(action1) || Input.GetMouseButtonDown(0))
        {
            // This if statement is checked here because OnMouseOver is called after OnGUI 
            // and as a result, waitingForInput would be set to true the same frame it was set to false
            // so I set this bool to skip this frame
            if (actionKeyUpdated)
            {
                actionKeyUpdated = false;
            }
            else
            {
                waitingForInput = true;
                image.sprite = clickedSprite;
            }

        }

    }

    private void OnMouseExit()
    {
        if (!waitingForInput)
            image.sprite = defaultSprite;
    }

    private void OnGUI()
    {
        if (!waitingForInput)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            waitingForInput = false;
            image.sprite = defaultSprite;
            return;
        }

        Event keyPressed = Event.current;
        if (keyPressed.type == EventType.KeyDown)
        {
            KeyCode newKey = keyPressed.keyCode;
            gameSettings.SetKey(keyType, newKey);

            // Update if changed action key
            if (keyType == GameSettings.KeyType.Action1)
            {
                action1 = gameSettings.GetBindedKey(GameSettings.KeyType.Action1);
                actionKeyUpdated = true;
            }

            // update GUI
            text.text = newKey.ToString();
            image.sprite = defaultSprite;
            waitingForInput = false;
        }
    }

}
