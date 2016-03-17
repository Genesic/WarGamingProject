using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class PostEffectManager : MonoBehaviour 
{
	static public PostEffectManager Instance { private set; get; }

	List<PostEffect> mPostEffectList = new List<PostEffect>();

	// Use this for initialization
	void Start () 
	{
		Instance = this;
	}

	void OnDestroy()
	{
		if(Instance == this) Instance = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		PostEffect[] freeEffect = mPostEffectList.Where (_ => _.IsEnable == false).ToArray ();

		for (int Indx = 0; Indx < freeEffect.Length; ++Indx)
			mPostEffectList.Remove (freeEffect[Indx]);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		RenderTexture texIn = src;

		for (int Indx = 0; Indx < mPostEffectList.Count; ++Indx) 
		{
			RenderTexture texOut = mPostEffectList[Indx].PostRender(texIn);

			if (texIn != src && texIn != texOut && texOut != null)
				RenderTexture.ReleaseTemporary(texIn);

			if (texOut != null)
				texIn = texOut;
		}

		Graphics.Blit (texIn, dest);

		if (texIn != src) RenderTexture.ReleaseTemporary(texIn);
	}

	public PostEffect AddPostEffect(PostEffectType _type)
	{
		PostEffect effect = null;

		switch (_type) 
		{
		case PostEffectType.Shock:
			effect = new Shock();
			break;

		case PostEffectType.TwoScreen:
			effect = new TwoScreen();
			break;

		case PostEffectType.BulletCurtain:
			effect = new BulletCurtain();
			break;

		default:
			break;
		}

		if (effect != null) 
		{
			effect.Initial();
			mPostEffectList.Add(effect);
		}

		return effect;
	}
}
