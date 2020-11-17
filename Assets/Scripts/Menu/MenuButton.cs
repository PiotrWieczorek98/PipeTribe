using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public enum TypeOfButton { Play, Settings, Exit };
    public TypeOfButton typeOfButton;
    public Sprite defaultSprite;
    public Sprite hoverSprite;

    Image buttonImage;
    KeyCode actionButton;

    string levelName;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;
        actionButton = FindObjectOfType<GameSettings>().GetKeyBind(GameSettings.KeyMap.Action);
    }

    private void OnMouseOver()
    {
        buttonImage.sprite = hoverSprite;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionButton))
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(delayAction(1f));
        }


    }
    IEnumerator delayAction(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (typeOfButton)
        {
            case TypeOfButton.Play:

                break;

            case TypeOfButton.Settings:

                break;
            case TypeOfButton.Exit:

                break;
        }
    }

    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }

}
