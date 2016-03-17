using UnityEngine;
using System.Collections;

public class RhythmLocate : MonoBehaviour 
{
	public enum Pivot
	{
		Center,
		CenterUpper,
		CenterDown,
		Left,
		LeftUpper,
		LeftDown,
		Right,
		RightUpper,
		RightDown,
	}

	[SerializeField]
	Camera mCameraUI = null;

	[SerializeField]
	Vector2 mScreenPosition = new Vector2(0F,0F);

	[SerializeField]
	Pivot mPivot = Pivot.Center;

	// Use this for initialization
	void Start () 
	{
		Locate ();	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void Locate()
	{
		if (mCameraUI == null)
			return;

		float ScreenX = mScreenPosition.x;
		float ScreenY = mScreenPosition.y;

		switch(mPivot)
		{
		case Pivot.Center:
			ScreenX += (Screen.width >> 1);
			ScreenY += (Screen.height >> 1);
			break;

		case Pivot.CenterDown:
			ScreenX += (Screen.width >> 1);
			break;

		case Pivot.CenterUpper:
			ScreenX += (Screen.width >> 1);
			ScreenY += (Screen.height);
			break;

		case Pivot.Left:
			ScreenY += (Screen.height >> 1);
			break;

		case Pivot.LeftDown:
			break;

		case Pivot.LeftUpper:
			ScreenY += (Screen.height);
			break;

		case Pivot.Right:
			ScreenX += (Screen.width);
			ScreenY += (Screen.height >> 1);
			break;

		case Pivot.RightDown:
			ScreenX += (Screen.width);
			break;

		case Pivot.RightUpper:
			ScreenX += (Screen.width);
			ScreenY += (Screen.height);
			break;
		}

		Vector3 worldPos = mCameraUI.ScreenToWorldPoint (new Vector3(ScreenX, ScreenY, mCameraUI.nearClipPlane));

		this.transform.position = worldPos;
	}
}
