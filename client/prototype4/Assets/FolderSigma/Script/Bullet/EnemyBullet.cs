using UnityEngine;
using System.Collections;

public class EnemyBullet : BezierLine 
{
	void OnDestroy()
	{
		Explosion ();
	}
	
	void Explosion()
	{
		Vector3 explosion = mUseCamera.WorldToScreenPoint (mBezierPt [3]);
		
		SceneController.SetCameraCommand(SceneControllerType.Player, 
		                                 CameraCommand.PostEffect_Shock, 
		                                 (System.Object)(explosion.x/Camera.main.pixelWidth),
		                                 (System.Object)(explosion.y/(Camera.main.pixelHeight)),
		                                 (System.Object)2F,
		                                 (System.Object)0F);
	}
}
