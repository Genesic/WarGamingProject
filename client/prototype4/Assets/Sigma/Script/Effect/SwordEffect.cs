using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEffect : MonoBehaviour 
{
	#region Data Struct
	public class SwordEffectPt
	{
		public Vector3 HeadPt;
		public Vector3 TailPt;
		public float LifeTime;
	}

	[System.Serializable]
	public class SwordPt
	{
		public Transform HeadPt;
		public Transform TailPt;
	}

	public enum SwordEffectCollectionType
	{
		FrameInterval = 0,
		TimeInterval,
	}
	#endregion

	#region Common
	const int SWORD_EFFECT_POINT_MIN_COUNT = 4;

	[SerializeField]
	SwordEffectCollectionType mCollectionType = SwordEffectCollectionType.FrameInterval;
	public SwordEffectCollectionType CollectionType { get { return mCollectionType; } set { mCollectionType = value; } }

	Queue<SwordEffectPt> mSwordEffectList = new Queue<SwordEffectPt>();
	public int SwordEffectPtCount { get { return mSwordEffectList.Count; }}

	[SerializeField]
	SwordPt mSwordPt = null;
	public Transform Head { get { return mSwordPt == null ? null : mSwordPt.HeadPt; } set { if(mSwordPt != null) mSwordPt.HeadPt = value; } }
	public Transform Tail { get { return mSwordPt == null ? null : mSwordPt.TailPt; } set { if(mSwordPt != null) mSwordPt.TailPt = value; } }


	[SerializeField]
	Material mMatEffect = null;
	public Material MatEffect { get { return mMatEffect; } set { mMatEffect = value; } }

	[SerializeField]
	bool mUseSmooth = false;
	public bool UseSmooth { get { return mUseSmooth; } set { mUseSmooth = value; } }

	[SerializeField]
	public bool IsCollection = true;

	SwordEffectPt mLastCollect = null;
	#endregion

	#region FrameInterval
	int mMaxSwordEffectPt = 32;
	public int MaxSwordEffectPt { get { return mMaxSwordEffectPt; } set { mMaxSwordEffectPt = value; } }
	#endregion

	#region FrameInterval
	[SerializeField]
	float mDiscardLength = 0.1F;
	public float DiscardLength { get { return mDiscardLength; } set { mDiscardLength = value; } }
	#endregion

	// Use this for initialization
	void OnDisable()
	{
		mSwordEffectList.Clear ();
		mLastCollect = null;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if (mSwordPt == null || mSwordPt.HeadPt == null || mSwordPt.TailPt == null)
			return;

		SwordEffectPt effectPt = null;

		switch(mCollectionType)
		{
		case SwordEffectCollectionType.FrameInterval:

			if (mSwordEffectList.Count < mMaxSwordEffectPt && IsCollection == true) 
			{
				effectPt = new SwordEffectPt();
			}
			else if(0 < mSwordEffectList.Count)
			{
				effectPt = mSwordEffectList.Dequeue();
			}

			if (IsCollection == true)
			{
				if (mSwordEffectList.Count == 0)
				{
					effectPt.HeadPt = mSwordPt.HeadPt.position;
					effectPt.TailPt = mSwordPt.TailPt.position;
				}
				else
				{
					effectPt.HeadPt = Smooth(mLastCollect.HeadPt, mSwordPt.HeadPt.position);
					effectPt.TailPt = Smooth(mLastCollect.TailPt, mSwordPt.TailPt.position);
				}

				effectPt.LifeTime = Time.time;
				
				mLastCollect = effectPt;
				mSwordEffectList.Enqueue (effectPt);
			}
			break;

		case SwordEffectCollectionType.TimeInterval:

			while(mSwordEffectList.Count != 0)
			{
				if(mDiscardLength <= Time.time - mSwordEffectList.Peek().LifeTime) 
					mSwordEffectList.Dequeue();
				else
					break;
			}

			if (IsCollection == true)
			{
				if (mSwordEffectList.Count == 0)
				{
					if (effectPt == null) effectPt = new SwordEffectPt();

					effectPt.HeadPt = mSwordPt.HeadPt.position;
					effectPt.TailPt = mSwordPt.TailPt.position;
					effectPt.LifeTime = Time.time;

					mLastCollect = effectPt;
					mSwordEffectList.Enqueue (effectPt);
				}
				else if( 0.1F < Vector3.Distance(mSwordPt.HeadPt.position, mLastCollect.HeadPt) || 0.1F < Vector3.Distance(mSwordPt.TailPt.position, mLastCollect.TailPt))
				{
					if (effectPt == null) effectPt = new SwordEffectPt();

					effectPt.HeadPt = Smooth(mLastCollect.HeadPt, mSwordPt.HeadPt.position);
					effectPt.TailPt = Smooth(mLastCollect.TailPt, mSwordPt.TailPt.position);
					effectPt.LifeTime = Time.time;

					mLastCollect = effectPt;
					mSwordEffectList.Enqueue (effectPt);
				}
				else
				{
					mLastCollect.LifeTime = Time.time;
				}
			}
			break;
		}
	}

	void OnRenderObject()
	{
		if (mSwordEffectList.Count < SWORD_EFFECT_POINT_MIN_COUNT)
			return;

		DrawSwordEffect ();
	}

	void DrawSwordEffect()
	{
		GL.PushMatrix();
		
		mMatEffect.SetPass (0);


		GL.Begin(GL.QUADS);		

		SwordEffectPt[] list = mSwordEffectList.ToArray ();

		float step = 1F / (list.Length - 1);

		for (int Indx = 1; Indx < list.Length; ++Indx) 
		{
			float Tile = 1 - step * (Indx - 1);

			GL.TexCoord(new Vector3(Tile, 0, 0));
			GL.Vertex3(list[Indx - 1].TailPt.x, 
			           list[Indx - 1].TailPt.y, 
			           list[Indx - 1].TailPt.z);

			GL.TexCoord(new Vector3(Tile, 1, 0));
			GL.Vertex3(list[Indx - 1].HeadPt.x, 
			           list[Indx - 1].HeadPt.y, 
			           list[Indx - 1].HeadPt.z);


			Tile = 1 - step * Indx;
			
			GL.TexCoord(new Vector3(Tile, 1, 0));
			GL.Vertex3(list[Indx].HeadPt.x, 
			           list[Indx].HeadPt.y, 
			           list[Indx].HeadPt.z);
			
			GL.TexCoord(new Vector3(Tile, 0, 0));
			GL.Vertex3(list[Indx].TailPt.x, 
			           list[Indx].TailPt.y, 
			           list[Indx].TailPt.z);
		}		
		
		GL.End();
		
		GL.PopMatrix();
	}

	Vector3 Smooth(Vector3 _start, Vector3 _end)
	{
		return mUseSmooth ? Vector3.Lerp (_start, _end, Mathf.PingPong (Time.time, 1F)) : _end;
	}
}
