using UnityEngine;

public class BeatEffects : MonoBehaviour
{
	public enum BeatDetectorType { Menu, InLevel }
	public BeatDetectorType detectorType;
	public int sampleSize = 1024;



	MenuAnimator animator;
	// Start is called before the first frame update
	void Start()
	{
		string filePath = Application.streamingAssetsPath + FindObjectOfType<GameSettings>().LevelDir + "/";
		filePath += FindObjectOfType<MenuManager>().MusicName;
	}

}
