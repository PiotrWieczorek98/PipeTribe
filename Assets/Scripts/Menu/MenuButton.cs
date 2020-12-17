using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
	public enum TypeOfButton { LevelBrowser, Settings, Exit, LevelEditor, Back, Up, Down, Play, Left, Right };
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
			case TypeOfButton.LevelBrowser:
				StartCoroutine(Transition(menuManager.mainMenu.transform, menuManager.levelBrowser.transform));
				menuManager.CurrentWindow = MenuManager.Windows.LevelBrowser;
				FindObjectOfType<LevelBrowser>().SetFirstMusic();
				break;

			case TypeOfButton.Settings:
				StartCoroutine(Transition(menuManager.mainMenu.transform, menuManager.settings.transform));
				menuManager.CurrentWindow = MenuManager.Windows.Settings;
				break;

			case TypeOfButton.Back:
				if (menuManager.CurrentWindow == MenuManager.Windows.Settings)
					StartCoroutine(Transition(menuManager.settings.transform, menuManager.mainMenu.transform, true));
				else if (menuManager.CurrentWindow == MenuManager.Windows.LevelBrowser)
					StartCoroutine(Transition(menuManager.levelBrowser.transform, menuManager.mainMenu.transform, true));
				menuManager.CurrentWindow = MenuManager.Windows.MainMenu;
				break;

			case TypeOfButton.LevelEditor:
				menuAnimator.PlayAnimation(menuAnimator.logo.GetComponent<Animator>(), menuAnimator.loading);
				yield return new WaitForSeconds(delay);
				SceneManager.LoadScene(2);
				break;

			case TypeOfButton.Up:
				FindObjectOfType<LevelBrowser>().MoveUp();
				break;

			case TypeOfButton.Down:
				FindObjectOfType<LevelBrowser>().MoveDown();
				break;

			case TypeOfButton.Left:
				FindObjectOfType<SettingsTabs>().MoveLeft();
				break;

			case TypeOfButton.Right:
				FindObjectOfType<SettingsTabs>().MoveRight();
				break;

			case TypeOfButton.Play:
				menuAnimator.PlayAnimation(menuAnimator.levelBrowser.GetComponent<Animator>(), menuAnimator.loading);
				yield return new WaitForSeconds(delay);
				SceneManager.LoadScene(1);
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

	IEnumerator Transition(Transform currentlyActive, Transform newActive, bool reverse = false)
	{
		MenuAnimator menuAnimator = FindObjectOfType<MenuAnimator>();
		// Set draw order
		//currentlyActive.SetAsLastSibling();
		// Activate appearing appear
		newActive.gameObject.SetActive(true);
		// Disable buttons for transition time
		foreach (MenuButton button in currentlyActive.GetComponentsInChildren<MenuButton>())
			button.SetActive(false);
		foreach (MenuButton button in newActive.GetComponentsInChildren<MenuButton>())
			button.SetActive(false);

		// Play transition animations
		if (reverse)
		{
			menuAnimator.PlayAnimation(newActive.GetComponent<Animator>(), menuAnimator.zoomOutReverse);
			menuAnimator.PlayAnimation(currentlyActive.GetComponent<Animator>(), menuAnimator.zoomInReverse);
		}
		else
		{
			menuAnimator.PlayAnimation(currentlyActive.GetComponent<Animator>(), menuAnimator.zoomOut);
			menuAnimator.PlayAnimation(newActive.GetComponent<Animator>(), menuAnimator.zoomIn);
		}

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
