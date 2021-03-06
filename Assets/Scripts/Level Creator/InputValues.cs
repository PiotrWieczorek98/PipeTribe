﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class InputValues : MonoBehaviour
{
	public enum ValueType { MusicName, OffsetValue, BPMValue };
	public ValueType valueType;

	public InputField levelNameInputField;
	public InputField offsetInputField;
	public InputField bpmInputField;

	LevelCreatorManager levelCreatorManager;

	private void Awake()
	{
		levelCreatorManager = FindObjectOfType<LevelCreatorManager>();
	}

	public void UpdateManagerValues()
	{
		switch (valueType)
		{
			case ValueType.OffsetValue:
				try
				{
					levelCreatorManager.OffsetValue = float.Parse(offsetInputField.text, CultureInfo.InvariantCulture);
				}
				catch
				{
					levelCreatorManager.OffsetValue = 0;
				}

				if (levelCreatorManager.MusicSource.clip != null)
					FindObjectOfType<TimelineIndicator>().SetBeatIndicators(levelCreatorManager.BeatsTotal, levelCreatorManager.OffsetValue);
				break;
			case ValueType.MusicName:
				levelCreatorManager.MusicName = levelNameInputField.text;
				break;
			case ValueType.BPMValue:
				try
				{
					levelCreatorManager.BPMValue = float.Parse(bpmInputField.text, CultureInfo.InvariantCulture);
				}
				catch
				{
					levelCreatorManager.BPMValue = 0;
				}

				if (levelCreatorManager.MusicSource.clip != null)
				{
					// calculate total amount of beats in music
					float bps = levelCreatorManager.BPMValue / 60;
					levelCreatorManager.BeatsTotal = (int)(levelCreatorManager.MusicSource.clip.length * bps);
					FindObjectOfType<TimelineIndicator>().SetBeatIndicators(levelCreatorManager.BeatsTotal, levelCreatorManager.OffsetValue);
				}

				break;
		}
	}

	public void UpdateFieldValues()
	{
		offsetInputField.text = levelCreatorManager.OffsetValue.ToString();
		bpmInputField.text = levelCreatorManager.BPMValue.ToString();
	}

}
