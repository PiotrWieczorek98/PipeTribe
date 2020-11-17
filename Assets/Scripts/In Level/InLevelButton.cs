using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InLevelButton : MonoBehaviour
{
    public enum TypeOfButton { Restart, Exit };
    public TypeOfButton typeOfButton;

    public AudioClip clickSound;

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
        GetComponent<AudioSource>().Play();
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionButton))
        {
            buttonImage.sprite = clickedSprite;
            switch (typeOfButton)
            {
                case TypeOfButton.Restart:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;

                case TypeOfButton.Exit:
                    SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
                    break;
            }
        }
    }
    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }

}
