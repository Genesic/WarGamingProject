using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour 
{
	public Camera UseCamera = null;

	public GameObject Bullet = null;

	int mWidthStart = 0;
	int mWidthEnd = 0;

	int mHeighthStart = 0;
	int mHeightEnd = 0;

	// Use this for initialization
	void Start () 
	{
		Debug.Log (string.Format("screen:({0},{1})",Screen.width, Screen.height));

		mWidthStart = Screen.width >> 2;
		mWidthEnd = mWidthStart * 3;

		mHeighthStart = Screen.height >> 3;
		mHeightEnd = mHeighthStart * 3;
	}

	void OnGUI() 
	{
		if (GUI.Button(new Rect(10, 10, 150, 100), "Player Shoot"))
			CreatePlayerBullet();

		if (GUI.Button(new Rect(10, 150, 150, 100), "Enemy Shoot"))
			CreateEnemyBullet();
		
	}

	void CreatePlayerBullet()
	{
		//GameObject createBullet = GameObject.Instantiate<GameObject> (Bullet);

		float sx = Random.Range (mWidthStart, mWidthEnd);
		float sy = Random.Range (mHeighthStart, mHeightEnd);

		float ex = Random.Range (mWidthStart, mWidthEnd);
		float ey = (Random.Range (mHeighthStart, mHeightEnd) + Screen.height >> 1);

		Debug.Log (string.Format("start:({0},{1}), end:({2},{3})",sx,sy,ex,ey));

		SceneController.SetRoleCommand (SceneControllerType.Player,
		                                RoleCommand.Action,
		                                (System.Object)RoleAction.Fight);	

		SceneController.SetCameraCommand (SceneControllerType.Bullet, CameraCommand.Bullet_PlayerShoot, 
		                                 (System.Object)sx,
		                                 (System.Object)sy,
		                                 (System.Object)ex,
		                                 (System.Object)ey);
	}

	void CreateEnemyBullet()
	{
		//GameObject createBullet = GameObject.Instantiate<GameObject> (Bullet);
		
		float ex = Random.Range (mWidthStart, mWidthEnd);
		float ey = Random.Range (mHeighthStart, mHeightEnd);
		
		float sx = Random.Range (mWidthStart, mWidthEnd);
		float sy = (Random.Range (mHeighthStart, mHeightEnd) + Screen.height >> 1);
		
		Debug.Log (string.Format("start:({0},{1}), end:({2},{3})",sx,sy,ex,ey));
		
		SceneController.SetRoleCommand (SceneControllerType.Enemy,
		                                RoleCommand.Action,
		                                (System.Object)RoleAction.Fight);	
		
		SceneController.SetCameraCommand (SceneControllerType.Bullet, CameraCommand.Bullet_EnemyShoot, 
		                                  (System.Object)sx,
		                                  (System.Object)sy,
		                                  (System.Object)ex,
		                                  (System.Object)ey);
	}
}
