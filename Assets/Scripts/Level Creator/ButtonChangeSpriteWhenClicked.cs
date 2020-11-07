using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeSpriteWhenClicked : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite clickedSprite;

    Image buttonImage;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;
    }

    private void OnMouseDown()
    {
        buttonImage.sprite = clickedSprite;
    }
    private void OnMouseUp()
    {
        buttonImage.sprite = defaultSprite;
    }
}
