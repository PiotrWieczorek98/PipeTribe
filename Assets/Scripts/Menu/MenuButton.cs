using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public enum TypeOfButton { Play, Settings, Exit, LevelEditor, Back };
    public TypeOfButton typeOfButton;

    public Sprite defaultSprite;
    public Sprite hoverSprite;

    Image buttonImage;
    KeyCode actionKey;

    bool isActive = true;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = defaultSprite;
    }

    private void Start()
    {
        actionKey = FindObjectOfType<GameSettings>().GetBindedKey(GameSettings.KeyType.Action1);
    }

    private void OnMouseExit()
    {
        buttonImage.sprite = defaultSprite;
    }

    private void OnMouseOver()
    {
        if (!isActive)
            return;

        buttonImage.sprite = hoverSprite;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(actionKey))
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(buttonAction(1f));
        }


    }
    IEnumerator buttonAction(float delay)
    {
        MenuAnimator menuAnimator = FindObjectOfType<MenuAnimator>();
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        switch (typeOfButton)
        {
            case TypeOfButton.Play:
                menuAnimator.PlayAnimation(menuAnimator.logo.GetComponent<Animator>(), menuAnimator.loading);
                yield return new WaitForSeconds(delay);
                break;

            case TypeOfButton.Settings:
                StartCoroutine(Transition(menuManager.menu.transform, menuManager.settings.transform));
                break;

            case TypeOfButton.Back:
                StartCoroutine(Transition(menuManager.settings.transform, menuManager.menu.transform));
                break;

            case TypeOfButton.LevelEditor:
                menuAnimator.PlayAnimation(menuAnimator.logo.GetComponent<Animator>(), menuAnimator.loading);
                yield return new WaitForSeconds(delay);
                SceneManager.LoadScene(2);
                break;

            case TypeOfButton.Exit:
                // Application.Quit() does not work in the editor so UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                #else
                                    Application.Quit();
                #endif
                break;
        }
    }

    IEnumerator Transition(Transform currentlyActive, Transform newActive)
    {
        MenuAnimator menuAnimator = FindObjectOfType<MenuAnimator>();
        // Set draw order
        currentlyActive.SetAsLastSibling();
        // Activate appearing appear
        newActive.gameObject.SetActive(true);
        // Disable buttons for transition time
        foreach(MenuButton button in currentlyActive.GetComponentsInChildren<MenuButton>())
            button.SetActive(false);
        foreach (MenuButton button in newActive.GetComponentsInChildren<MenuButton>())
            button.SetActive(false);

        // Play transition animations
        menuAnimator.PlayAnimation(currentlyActive.GetComponent<Animator>(), menuAnimator.zoomOut);
        menuAnimator.PlayAnimation(newActive.GetComponent<Animator>(), menuAnimator.zoomIn);
        yield return new WaitForSeconds(menuAnimator.zoomIn.length);
        
        // Disable disappearing object
        currentlyActive.gameObject.SetActive(false);
        // Enable buttons
        foreach (MenuButton button in newActive.GetComponentsInChildren<MenuButton>())
            button.SetActive(true);
    }

    public void SetActive(bool value)
    {
        isActive = value;
    }

}
