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

    string levelName;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;

    }

    private void OnMouseOver()
    {
        buttonImage.sprite = hoverSprite;

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
