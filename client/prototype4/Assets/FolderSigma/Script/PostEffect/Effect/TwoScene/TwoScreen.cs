using UnityEngine;
using System.Collections;

public class TwoScreen : PostEffect 
{
	private static Material mTwoScreenMaterial;
	
	private static Material TwoScreenMaterial
	{
		get
		{
			if ( null == mTwoScreenMaterial )
			{
				mTwoScreenMaterial = new Material( Shader.Find( "Custom/TwoScreenShader" ) )
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			
			return mTwoScreenMaterial;
		}
	}

	// Use this for initialization
	public override void Initial()
	{
	}
	
	public override RenderTexture PostRender(RenderTexture _source)
	{
		RenderTexture twoScreenTarget = RenderTexture.GetTemporary (Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		
		Graphics.Blit (_source, twoScreenTarget, TwoScreenMaterial);
		
		return twoScreenTarget;
	}
	
	public override void SetTexture(string _property, Texture _texture)
	{
		TwoScreenMaterial.SetTexture (_property, _texture);
	}
}
