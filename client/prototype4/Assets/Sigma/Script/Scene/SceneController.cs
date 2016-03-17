using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour 
{
	[SerializeField]
	SceneFlowData mSceneFlowData = new SceneFlowData();

	ISceneFlow mCurSceneFlow = null;

	public static SceneController Instance { private set; get; }
	
	// Use this for initialization
	void Start () 
	{
		Instance = this;

		mSceneFlowData.Initial ();	

		InitialRole (mSceneFlowData.Player);
		InitialRole (mSceneFlowData.Enemy );

		InitialCamera (mSceneFlowData.PlayerCam);
		InitialCamera (mSceneFlowData.EnemyCam);

		InitialCamera (mSceneFlowData.BulletCam);

		InitialMainCamera ();

		SetNextSceneFlow (SceneFlowType.Dive);
	}

	void OnDestroy ()
	{
		if(Instance == this)
			Instance = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mCurSceneFlow == null)
			return;
	
		mCurSceneFlow.UpdateFlow ();
	}

	void InitialRole(IRole _role)
	{
		if (_role == null)
			return;

		_role.Initial ();
		_role.AnimEventCallback = OnAnimEventHandle;
	}

	void InitialCamera(ICamera _cam)
	{
		if (_cam == null)
			return;

		_cam.Initial ();
	}

	void InitialMainCamera()
	{
		if (mSceneFlowData.MainCam == null)
			return;

		mSceneFlowData.MainCam.Initial ();

		if (mSceneFlowData.PlayerCam != null && mSceneFlowData.EnemyCam != null)
		{
			mSceneFlowData.MainCam.SetCommand (CameraCommand.PostEffect_TwoScreen,
			                                  (System.Object)mSceneFlowData.EnemyCam.TexRender,
			                                  (System.Object)mSceneFlowData.PlayerCam.TexRender);
		}

		if (mSceneFlowData.BulletCam != null)
		{
			mSceneFlowData.MainCam.SetCommand (CameraCommand.PostEffect_BulletCurtain,
			                                   (System.Object)mSceneFlowData.BulletCam.TexRender);
		}
	}

	void OnAnimEventHandle(IRole _sender, string _msg)
	{
		if (mCurSceneFlow == null)
			return;

		mCurSceneFlow.AnimEventHandle (_sender, _msg);
	}

	void SetNextSceneFlow(SceneFlowType _type)
	{
		if (mCurSceneFlow != null)
		{
			mCurSceneFlow.LeaveFlow ();
			mCurSceneFlow = null;
		}

		switch(_type)
		{
		case SceneFlowType.Dive:
			mCurSceneFlow = new SceneFlow_Dive();
			break;

		case SceneFlowType.Ready:
			mCurSceneFlow = new SceneFlow_Ready();
			break;

		case SceneFlowType.Fight:
			mCurSceneFlow = new SceneFlow_Fight();
			break;

		default:
			break;
		}

		if (mCurSceneFlow != null) 
		{
			mCurSceneFlow.Initial ();
			mCurSceneFlow.EnterFlow ();
		}
	}

	static public void SetSceneFlow(SceneFlowType _type)
	{
		if (Instance == null)
			return;

		if (Instance.mCurSceneFlow != null)
		{
			Instance.mCurSceneFlow.LeaveFlow ();
			Instance.mCurSceneFlow = null;
		}
		
		switch(_type)
		{
		case SceneFlowType.Dive:
			Instance.mCurSceneFlow = new SceneFlow_Dive();
			break;
			
		case SceneFlowType.Ready:
			Instance.mCurSceneFlow = new SceneFlow_Ready();
			break;
			
		case SceneFlowType.Fight:
			Instance.mCurSceneFlow = new SceneFlow_Fight();
			break;
			
		default:
			break;
		}
		
		if (Instance.mCurSceneFlow != null) 
		{
			Instance.mCurSceneFlow.Initial ();
			Instance.mCurSceneFlow.EnterFlow ();
		}
	}

	static public void SetRoleCommand(SceneControllerType _roleType, RoleCommand _command, params System.Object[] _params)
	{
		if (Instance == null)
			return;

		IRole role = null;

		switch(_roleType)
		{
		case SceneControllerType.Player:
			role = Instance.mSceneFlowData.Player;
			break;
			
		case SceneControllerType.Enemy:
			role = Instance.mSceneFlowData.Enemy;
			break;
		}

		if (role == null)
			return;

		role.SetCommand (_command, _params);
	}

	static public void SetCameraCommand(SceneControllerType _cameraType, CameraCommand _command, params System.Object[] _params)
	{
		if (Instance == null)
			return;

		ICamera cam = null;

		switch(_cameraType)
		{
		case SceneControllerType.Player:
			cam = Instance.mSceneFlowData.PlayerCam;
			break;

		case SceneControllerType.Enemy:
			cam = Instance.mSceneFlowData.EnemyCam;
			break;

		case SceneControllerType.Bullet:
			cam = Instance.mSceneFlowData.BulletCam;
			break;
		}

		if (cam == null)
			return;

		cam.SetCommand (_command, _params);
	}
}
