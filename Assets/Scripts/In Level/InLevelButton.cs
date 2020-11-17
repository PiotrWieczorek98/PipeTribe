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

    string levelName;
    KeyCode actionButton;

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
            buttonImage.sprite = clickedSprite;
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
