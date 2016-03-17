using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour, ICamera
{
	public RenderTexture TexRender { get { return null; } }
	
	Animator mAnim = null;
	
	PostEffectManager mPostEffect = null;
	
	void AnimEventHandle(string _msg)
	{
	}
	
	void InitialPostEffect()
	{
		mPostEffect = GetComponent<PostEffectManager>();
	}
	
	#region ICamera
	public void Initial()
	{	
		InitialPostEffect ();
	}
	
	public void SetCommand(CameraCommand _command, params System.Object[] _params)
	{
		switch(_command)
		{			
		case CameraCommand.PostEffect_TwoScreen:
			
			if(mPostEffect != null)
			{
				PostEffect effect = mPostEffect.AddPostEffect(PostEffectType.TwoScreen);
				if(effect != null) 
				{
					effect.SetTexture("_UpperTex", (Texture)_params[0]);
					effect.SetTexture("_DownTex", (Texture)_params[1]);
				}
			}
			break;

		case CameraCommand.PostEffect_BulletCurtain:

			if(mPostEffect != null)
			{
				PostEffect effect = mPostEffect.AddPostEffect(PostEffectType.BulletCurtain);
				if(effect != null) 
				{
					effect.SetTexture("_BulletTex", (Texture)_params[0]);
				}
			}
			break;
		}
	}
	#endregion
}