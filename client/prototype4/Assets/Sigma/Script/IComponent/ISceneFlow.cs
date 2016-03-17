using UnityEngine;
using System.Collections;

[System.Serializable]
public class SceneFlowData
{
	public GameObject PlayerGO = null;
	public GameObject EnemyGO = null;

	public GameObject PlayerCameraGO = null;
	public GameObject EnemyCameraGO = null;

	public GameObject BulletCameraGO = null;

	public GameObject MainCameraGO = null;

	public IRole Player = null;
	public IRole Enemy = null;

	public ICamera PlayerCam = null;
	public ICamera EnemyCam = null;

	public ICamera BulletCam = null;

	public ICamera MainCam = null;

	public void Initial()
	{
		if(PlayerGO != null)		Player		= PlayerGO.GetComponent<IRole>();
		if(EnemyGO != null)			Enemy		= EnemyGO.GetComponent<IRole> ();
		if(PlayerCameraGO != null)	PlayerCam	= PlayerCameraGO.GetComponent<ICamera> ();
		if(EnemyCameraGO != null)	EnemyCam    = EnemyCameraGO.GetComponent<ICamera> ();
		if(BulletCameraGO != null)	BulletCam	= BulletCameraGO.GetComponent<ICamera> ();
		if(MainCameraGO != null)	MainCam		= MainCameraGO.GetComponent<ICamera> ();
	}
}

public enum SceneFlowType
{
	None,
	Dive,
	Ready,
	Fight,
}

public enum SceneControllerType
{
	Player,
	Enemy,
	Bullet,
}

public enum RoleCommand
{
	Action,
	Display,
}

public enum CameraCommand
{
	CameraType_Dive,
	CameraType_Fight,
	CameraType_Ready,

	PostEffect_Shock,
	PostEffect_TwoScreen,
	PostEffect_BulletCurtain,

	Bullet_PlayerShoot,
	Bullet_EnemyShoot,
}

public interface ISceneFlow
{
	void Initial();

	void EnterFlow();

	void LeaveFlow();

	void UpdateFlow();

	void AnimEventHandle(IRole _sender, string _msg);
}
