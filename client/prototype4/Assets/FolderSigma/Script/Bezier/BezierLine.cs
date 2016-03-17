using UnityEngine;
using System.Collections;

public class BezierLine : MonoBehaviour 
{
	[System.Serializable]
	public class BezierParam
	{
		public float BezierWeight1 = 0.5F;
		public float BezierWeight2 = 0.5F;
	}

	[SerializeField]
	BezierParam mBezierParam = null;

	protected Vector3[] mBezierPt = new Vector3[4];

	protected Camera mUseCamera = null;

	bool mIsInitial = false;

	float mTime = 0F;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (mIsInitial == false)
			return;

		if (1F <= mTime)
		{
			//Explosion();

			Destroy(this.gameObject);
		}
		else
		{
			mTime += Time.fixedDeltaTime;

			this.transform.position = ComputeBezier(mTime);
		}
	}

	public void SetBezier(Camera _useCamera, Vector3 _start, Vector3 _end)
	{
		mUseCamera = _useCamera;

		mBezierPt [0] = _start;
		mBezierPt [3] = _end;

		Vector3 forward = (_end - _start).normalized;
		Vector3 normal = _end.x < _start.x ? Vector3.Cross (forward, Vector3.forward) : Vector3.Cross (forward, -Vector3.forward);

		mBezierPt [1] = _start + (forward * 0.25F) + (normal * mBezierParam.BezierWeight1);
		mBezierPt [2] = _start + (forward * 0.75F) + (normal * mBezierParam.BezierWeight2);

		mIsInitial = true;

		mTime = 0F;
	}

	Vector3 ComputeBezier(float _time)
	{
		_time = Mathf.Clamp (_time, 0F, 1F);

		float   ax, bx, cx;
		float   ay, by, cy;
		float   tSquared, tCubed;

		/*計算多項式係數*/
		
		cx = 3F * (mBezierPt[1].x - mBezierPt[0].x);
		bx = 3F * (mBezierPt[2].x - mBezierPt[1].x) - cx;
		ax = mBezierPt[3].x - mBezierPt[0].x - cx - bx;
		
		cy = 3F * (mBezierPt[1].y - mBezierPt[0].y);
		by = 3F * (mBezierPt[2].y - mBezierPt[1].y) - cy;
		ay = mBezierPt[3].y - mBezierPt[0].y - cy - by;

		/*計算位於參數值t的曲線點*/
		
		tSquared = _time * _time;
		tCubed = tSquared * _time;

		float   rx, ry, rz;
		
		rx = (ax * tCubed) + (bx * tSquared) + (cx * _time) + mBezierPt[0].x;
		ry = (ay * tCubed) + (by * tSquared) + (cy * _time) + mBezierPt[0].y;
		rz = mBezierPt [0].z + (mBezierPt [3].z - mBezierPt [0].z) * _time;

		return new Vector3(rx, ry, rz);
	}

	/*void Explosion()
	{
		Vector3 explosion = mUseCamera.WorldToScreenPoint (mBezierPt [3]);

		SceneController.SetCameraCommand(SceneControllerType.Enemy, 
		                                 CameraCommand.PostEffect_Shock, 
		                                 (System.Object)(explosion.x/Camera.main.pixelWidth),
		                                 (System.Object)(explosion.y/(Camera.main.pixelHeight * 2F)),
		                                 (System.Object)2F,
		                                 (System.Object)0F);
	}*/
}
