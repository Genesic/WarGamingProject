using UnityEngine;
using System.Collections;

public class BulletCurtain : PostEffect 
{
	private static Material mBulletCurtainMaterial;
	
	private static Material BulletCurtainMaterial
	{
		get
		{
			if ( null == mBulletCurtainMaterial )
			{
				mBulletCurtainMaterial = new Material( Shader.Find( "Custom/BulletCurtainShader" ) )
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			
			return mBulletCurtainMaterial;
		}
	}
	
	// Use this for initialization
	public override void Initial()
	{
	}
	
	public override RenderTexture PostRender(RenderTexture _source)
	{
		RenderTexture bulletCurtainTarget = RenderTexture.GetTemporary (Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		
		Graphics.Blit (_source, bulletCurtainTarget, BulletCurtainMaterial);
		
		return bulletCurtainTarget;
	}
	
	public override void SetTexture(string _property, Texture _texture)
	{
		BulletCurtainMaterial.SetTexture (_property, _texture);
	}
}
