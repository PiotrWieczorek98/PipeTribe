using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTabs : MonoBehaviour
{
	public GameObject Volume;
	public GameObject Keybinds;
	public GameObject Difficulty;
	List<RectTransform> tabs;

	int selectedTab = 0;

	float movementSpeed = 6;
	Vector3 gloabalDestination;

	private void Awake()
	{
		tabs = new List<RectTransform>()
		{
			Volume.GetComponent<RectTransform>(),
			Keybinds.GetComponent<RectTransform>(),
			Difficulty.GetComponent<RectTransform>()
		};

		gloabalDestination = new Vector3();

		tabs[0].anchoredPosition = new Vector3(0, 0, 1);
		tabs[1].anchoredPosition = new Vector3(tabs[0].sizeDelta.x, 0, 1);
		tabs[2].anchoredPosition = new Vector3(tabs[0].sizeDelta.x * 2, 0, 1);

		// Activate only main tab
		for (int i = 0; i < tabs.Count; i++)
		{
			if (selectedTab == i)
			{
				foreach (Transform child in tabs[i].transform)
				{
					child.gameObject.SetActive(true);
				}
			}
			else
			{
				foreach (Transform child in tabs[i].transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}

	private void Update()
	{
		if (Input.mouseScrollDelta.y > 0)
		{
			MoveLeft();
		}
		else if (Input.mouseScrollDelta.y < 0)
		{
			MoveRight();
		}

		for (int i = 0; i < tabs.Count; i++)
		{
			Vector3 localDestination = new Vector3(i * tabs[0].sizeDelta.x + gloabalDestination.x, 0, 1);
			tabs[i].anchoredPosition = Vector3.Lerp(tabs[i].anchoredPosition, localDestination, Time.deltaTime * movementSpeed);
		}
	}

	public void MoveRight()
	{
		if (selectedTab < tabs.Count - 1)
		{
			selectedTab++;
			gloabalDestination = new Vector3(gloabalDestination.x - tabs[0].sizeDelta.x, 0, 1);
			StartCoroutine(UpdateActiveStatus());
		}
	}
	public void MoveLeft()
	{
		if (selectedTab > 0)
		{
			selectedTab--;
			gloabalDestination = new Vector3(gloabalDestination.x + tabs[0].sizeDelta.x, 0, 1);
			StartCoroutine(UpdateActiveStatus());
		}
	}

	IEnumerator UpdateActiveStatus()
	{
		foreach (Transform child in tabs[selectedTab].transform)
		{
			child.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(0.5f);
		for(int i = 0; i< tabs.Count; i++)
		{
			if (selectedTab != i)
			{
				foreach (Transform child in tabs[i].transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}
}
