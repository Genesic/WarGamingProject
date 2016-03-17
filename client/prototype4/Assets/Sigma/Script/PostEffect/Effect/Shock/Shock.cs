using UnityEngine;
using System.Collections;

public class Shock : PostEffect 
{
	private static Material mShockMaterial;
	
	private static Material ShockMaterial
	{
		get
		{
			if ( null == mShockMaterial )
			{
				mShockMaterial = new Material( Shader.Find( "Custom/ShockShader" ) )
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			
			return mShockMaterial;
		}
	}

	private float mTime = 1F;

	public override void Initial()
	{
		mTime = 0;
	}
	
	public override RenderTexture PostRender(RenderTexture _source)
	{
		if (mTime < 1) 
		{
			mTime += Time.deltaTime;

			ShockMaterial.SetFloat("_ShockTime", mTime);

			RenderTexture shockTarget = RenderTexture.GetTemporary (Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		
			Graphics.Blit (_source, shockTarget, ShockMaterial);
		
			return shockTarget;
		}
		else
		{
			IsEnable = false;
		}

		return _source;
	}
	
	public override void SetVector(string _property, Vector4 _vector)
	{
		ShockMaterial.SetVector (_property, _vector);
	}
}
