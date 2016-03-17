using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour, ICamera
{
	RenderTexture mTexRender = null;
	public RenderTexture TexRender { get { return mTexRender; } }

	Animator mAnim = null;

	PostEffectManager mPostEffect = null;

	int CameraTypeId = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnDestroy()
	{
		if (mTexRender != null)
			mTexRender.Release ();

		mTexRender = null;
	}

	void AnimEventHandle(string _msg)
	{
	}

	void InitialRenderTexture()
	{
		mTexRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
		mTexRender.Create();
		mTexRender.name = "RenderTarget";

		Camera cam = GetComponent<Camera>();
		if(cam != null) cam.targetTexture = mTexRender;
	}

	void InitialAnim()
	{
		mAnim = GetComponent<Animator>();

		if(mAnim != null)
		{
			Monitor[] monitors = mAnim.GetBehaviours<Monitor>();
			for (int Indx = 0; Indx < monitors.Length; ++Indx)
				monitors [Indx].AnimationEventFunc = AnimEventHandle;
			
			CameraTypeId = Animator.StringToHash ("CameraType");
		}
	}

	void InitialPostEffect()
	{
		mPostEffect = GetComponent<PostEffectManager>();
	}

	#region ICamera
	public void Initial()
	{
		InitialAnim ();

		InitialPostEffect ();

		InitialRenderTexture ();
	}

	public void SetCameraType(int _type)
	{
		if (mAnim == null)
			return;

		mAnim.SetInteger (CameraTypeId, _type);
	}

	public void SetCommand(CameraCommand _command, params System.Object[] _params)
	{
		switch(_command)
		{
		case CameraCommand.CameraType_Dive:
			SetCameraType(1);
			break;

		case CameraCommand.CameraType_Fight:
			SetCameraType(2);
			break;

		case CameraCommand.CameraType_Ready:
			SetCameraType(0);
			break;



		case CameraCommand.PostEffect_Shock:

			if(mPostEffect != null)
			{
				PostEffect effect = mPostEffect.AddPostEffect(PostEffectType.Shock);
				if(effect != null) 
				{
					Vector4 screenPos = new Vector4((float)_params[0],(float)_params[1],(float)_params[2],(float)_params[3]);
					effect.SetVector("_ShockCenter", screenPos);
				}
			}
			break;
		}
	}
	#endregion
}
