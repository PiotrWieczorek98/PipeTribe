using UnityEngine;
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using System.Runtime.InteropServices;

public class GazePointer : MonoBehaviour
{
	public Texture2D cursorTexture;
	public bool cursorVisible = true;
	public bool useEyeTracker = true;
	GazePoint gaze;
	Point2D coords;

	public class GazePoint : IGazeListener
	{
		Point2D gazePoint;

		public GazePoint()
		{
			try
			{
				// Connect client
				GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0);

				// Register this class for events
				GazeManager.Instance.AddGazeListener(this);

			}
			catch (System.Exception e)
			{
				System.IO.File.AppendAllText(@"C:\Users\Piotrek\Desktop\xd.txt", e.InnerException.Message);
			}
		}

		public void OnGazeUpdate(GazeData gazeData)
		{
			if (gazeData.StateToString() != "STATE_TRACKING_GAZE | STATE_TRACKING_EYES | STATE_TRACKING_PRESENCE")
				return;

			Point2D leftPupil = gazeData.LeftEye.PupilCenterCoordinates;
			Point2D rightPupil = gazeData.RightEye.PupilCenterCoordinates;
			Point2D newGazePoint = gazeData.SmoothedCoordinates;


			if (leftPupil.X == 0 || leftPupil.Y == 0 ||
				rightPupil.X == 0 || rightPupil.Y == 0 ||
				newGazePoint.X == 0 || newGazePoint.Y == 0)
				return;

			gazePoint = newGazePoint;
		}

		public Point2D GetGazePoint() { return gazePoint; }
	}


	[DllImport("user32.dll")]
	static extern bool SetCursorPos(int X, int Y);
	// Start is called before the first frame update
	void Awake()
	{
		gaze = new GazePoint();

		Vector2 cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
		Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.ForceSoftware);
		Cursor.visible = cursorVisible;
	}

	// Update is called once per frame
	void Update()
	{
		if (useEyeTracker)
		{
			coords = gaze.GetGazePoint();

			if (coords.X != 0 && coords.Y != 0 && Application.isFocused && !Input.GetKey(KeyCode.LeftAlt))
			{
				SetCursorPos((int)coords.X, (int)coords.Y);//Call this when you want to set the mouse position
			}
		}
	}
}
