using UnityEngine;
using System.Collections;

public class PlayerBullet : BezierLine 
{
	void OnDestroy()
	{
		Explosion ();
	}

	void Explosion()
	{
		Vector3 explosion = mUseCamera.WorldToScreenPoint (mBezierPt [3]);
		
		SceneController.SetCameraCommand(SceneControllerType.Enemy, 
		                                 CameraCommand.PostEffect_Shock, 
		                                 (System.Object)(explosion.x/Camera.main.pixelWidth),
		                                 (System.Object)(explosion.y/(Camera.main.pixelHeight * 2F)),
		                                 (System.Object)2F,
		                                 (System.Object)0F);
	}
}
