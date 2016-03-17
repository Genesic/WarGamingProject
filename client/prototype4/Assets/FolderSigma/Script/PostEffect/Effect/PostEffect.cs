using UnityEngine;
using System.Collections;

public enum PostEffectType
{
	Mosiac,
	ColorCorrection,
	Blur,
	Bloom,
	Shake,
	Shock,
	Impact,

	TwoScreen,
	BulletCurtain,

	Total,
}

public abstract class PostEffect
{
	static public RenderBuffer DepthBuffer { get; set; }

	public bool IsEnable = true;

	public virtual void Initial()
	{
	}

	public virtual RenderTexture PostRender(RenderTexture _source)
	{
		return null;
	}

	public virtual void SetTexture(string _property, Texture _texture)
	{
	}

	public virtual void SetVector(string _property, Vector4 _vector)
	{
	}

	public virtual void SetParameter(params object[] _args)
	{
	}
}
