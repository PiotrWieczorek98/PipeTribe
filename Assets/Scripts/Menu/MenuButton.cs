using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public enum TypeOfButton { Play, Settings, Exit, LevelEditor };
    public TypeOfButton typeOfButton;
    public Sprite defaultSprite;
    public Sprite hoverSprite;

    Image buttonImage;
    KeyCode actionKey;

    string levelName;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;
    }

    private void Start()
    {
        actionKey = FindObjectOfType<GameSettings>().GetKeyBind(GameSettings.KeyMap.Action1);
    }

    private void OnMouseOver()
    {
        buttonImage.sprite = hoverSprite;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionKey))
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(delayAction(1f));
        }


    }
    IEnumerator delayAction(float delay)
    {
        FindObjectOfType<MenuAnimator>().PlayLoadingAnimation();
        yield return new WaitForSeconds(delay);

        switch (typeOfButton)
        {
            case TypeOfButton.Play:

                break;

            case TypeOfButton.Settings:

                break;
            case TypeOfButton.Exit:
                #if UNITY_EDITOR
                    // Application.Quit() does not work in the editor so
                    // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;
            case TypeOfButton.LevelEditor:
                SceneManager.LoadScene(2);
                break;
        }
    }

    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }

}
