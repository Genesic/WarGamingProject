using UnityEngine;
using System.Collections;

public class BulletCameraController : MonoBehaviour, ICamera
{
	RenderTexture mTexRender = null;
	public RenderTexture TexRender { get { return mTexRender; } }

	Camera mCamera = null;

	void OnDestroy()
	{
		if (mTexRender != null)
			mTexRender.Release ();
		
		mTexRender = null;
	}

	void AnimEventHandle(string _msg)
	{
	}

	void InitialCamera()
	{
		mCamera = GetComponent<Camera>();
	}

	void InitialRenderTexture()
	{
		mTexRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
		mTexRender.Create();
		mTexRender.name = "RenderTarget";
		
		Camera cam = GetComponent<Camera>();
		if(cam != null) cam.targetTexture = mTexRender;
	}

	void CreatePlayerBullet(float _sx, float _sy, float _ex, float _ey)
	{
		Vector3 start = mCamera.ScreenToWorldPoint (new Vector3((float)_sx, (float)_sy, mCamera.transform.position.z + mCamera.nearClipPlane));
		Vector3 end = mCamera.ScreenToWorldPoint (new Vector3((float)_ex, (float)_ey, mCamera.transform.position.z + mCamera.nearClipPlane));

		GameObject bulletGO = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("PlayerBullet"));

		BezierLine bullet = bulletGO.GetComponent<BezierLine>();
		bullet.SetBezier (mCamera, start, end);
	}

	void CreateEnemyBullet(float _sx, float _sy, float _ex, float _ey)
	{
		Vector3 start = mCamera.ScreenToWorldPoint (new Vector3((float)_sx, (float)_sy, mCamera.transform.position.z + mCamera.nearClipPlane));
		Vector3 end = mCamera.ScreenToWorldPoint (new Vector3((float)_ex, (float)_ey, mCamera.transform.position.z + mCamera.nearClipPlane));
		
		GameObject bulletGO = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("EnemyBullet"));
		
		BezierLine bullet = bulletGO.GetComponent<BezierLine>();
		bullet.SetBezier (mCamera, start, end);
	}
	
	#region ICamera
	public void Initial()
	{
		InitialCamera ();

		InitialRenderTexture ();
	}
	
	public void SetCommand(CameraCommand _command, params System.Object[] _params)
	{
		switch(_command)
		{
		case CameraCommand.Bullet_PlayerShoot:

			CreatePlayerBullet((float)_params[0],
			                   (float)_params[1],
			                   (float)_params[2],
			                   (float)_params[3]);
			break;

		case CameraCommand.Bullet_EnemyShoot:

			CreateEnemyBullet ((float)_params[0],
			                   (float)_params[1],
			                   (float)_params[2],
			                   (float)_params[3]);
			break;
		}
	}
	#endregion
}
