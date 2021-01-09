using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTimelineIndicator : MonoBehaviour
{
	public float TimelinePosition { get; set; }
	public float Timestamp { get; set; }
	public float TimeHeld { get; set; }

	float deltaX;
	float mouseAnchoredPosition;
	RectTransform rectTransform;
	TimelineIndicator timelineIndicator;
	AudioSource audioSource;

	KeyCode action1, action2;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		timelineIndicator = transform.parent.GetComponent<TimelineIndicator>();
		audioSource = FindObjectOfType<LevelCreatorManager>().MusicSource;

		GameSettings gameSettings = FindObjectOfType<GameSettings>();
		action1 = gameSettings.GetBindedKey(GameSettings.KeyType.Action1);
		action2 = gameSettings.GetBindedKey(GameSettings.KeyType.Action2);
	}

	void OnMouseDrag()
	{
		TimelinePosition = Input.mousePosition.x - deltaX;
		TimelinePosition = CrossSceneData.Remap(TimelinePosition, 0, Screen.width, timelineIndicator.LeftBorder, timelineIndicator.RightBorder);
		Timestamp = CrossSceneData.Remap(TimelinePosition, 0, Screen.width, 0, audioSource.clip.length);

		timelineIndicator.updateSingleNotePosition(transform);
	}

	private void OnMouseOver()
	{
		// Anchor position on action1 key
		if (Input.GetKeyDown(action1))
		{
			mouseAnchoredPosition = CrossSceneData.Remap(Input.mousePosition.x, 0, Screen.width,
				-timelineIndicator.TimelineCanvasRadius, timelineIndicator.TimelineCanvasRadius);

			deltaX = mouseAnchoredPosition - rectTransform.anchoredPosition.x;
		}
		// Delete object on action2 key
		else if (Input.GetKeyDown(action2) || Input.GetMouseButtonDown(1))
		{
			Destroy(transform.gameObject);
		}
	}
}
