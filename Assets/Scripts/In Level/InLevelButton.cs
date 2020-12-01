using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InLevelButton : MonoBehaviour
{
    public enum TypeOfButton { Restart, Exit };
    public TypeOfButton typeOfButton;

    public Sprite defaultSprite;
    public Sprite hoverSprite;
    public Sprite clickedSprite;
    Image buttonImage;

    KeyCode actionButton;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;
    }
    private void Start()
    {
        actionButton = FindObjectOfType<GameSettings>().GetBindedKey(GameSettings.KeyType.Action1);
    }

    private void OnMouseOver()
    {
        if (buttonImage.sprite == defaultSprite)
            buttonImage.sprite = hoverSprite;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionButton))
        {
            buttonImage.sprite = clickedSprite;
            GetComponent<AudioSource>().Play();
            StartCoroutine(delayAction(1f));
        }
    }
    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }

    IEnumerator delayAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        buttonImage.sprite = defaultSprite;

        switch (typeOfButton)
        {
            case TypeOfButton.Restart:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            case TypeOfButton.Exit:
                SceneManager.LoadScene(0);
                break;
        }
    }

}
