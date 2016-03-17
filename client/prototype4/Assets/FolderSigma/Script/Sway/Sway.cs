using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Sway : MonoBehaviour 
{
	private Material mSwayMaterial;	
	private Material SwayMaterial
	{
		get
		{
			if ( null == mSwayMaterial )
			{
				mSwayMaterial = new Material( Shader.Find( "Custom/SwayShader" ) )
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			
			return mSwayMaterial;
		}
	}

	public struct SwayData
	{
		public Mesh[] Meshes;
		public Vector3 Position;
		public Quaternion Direction;
	}

	Queue<SwayData> mSwayData = new Queue<SwayData>();
	Queue<SwayData> mSwayDataPool = new Queue<SwayData>();

	float mTime = 0F;
		
	[SerializeField]
	bool mIsCollection = false;
	public bool IsCollection { set { mIsCollection = value; } }
	
	const float COLLECTION_FREQUENCY = 0F;//0.2F;
	
	const int SWAY_COUNT = 8;
	
	[SerializeField]
	MeshFilter[] mMeshFilters = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mMeshFilters == null || mMeshFilters.Length == 0)
			return;
	
		mTime -= Time.deltaTime;
		
		if (mTime <= 0F)
		{
			if (mIsCollection == true)
			{
				Collect ();
			}
			else
			{
				Recycle ();
			}

			mTime = COLLECTION_FREQUENCY;
		}				
		
		DrawSway ();
	}

	void Collect()
	{
		while (SWAY_COUNT < mSwayData.Count)
			Recycle ();

		SwayData? sway = null;

		if (mSwayDataPool.Count != 0)
			sway = mSwayDataPool.Dequeue();

		if (sway == null)
		{
			SwayData newSway = new SwayData();
			newSway.Meshes = new Mesh[mMeshFilters.Length];
			
			for (int Indx = 0; Indx < newSway.Meshes.Length; ++Indx)
			{
				newSway.Meshes[Indx] = new Mesh();
			}
			
			sway = newSway;
		}

		SwayData resSway = sway.Value;
		
		resSway.Position = this.transform.position;
		resSway.Direction = this.transform.rotation;
		
		for (int Indx = 0; Indx < mMeshFilters.Length; ++Indx)
		{
			resSway.Meshes[Indx] = mMeshFilters[Indx].sharedMesh;
		}
		
		mSwayData.Enqueue(resSway);
	}

	void Recycle()
	{
		if (mSwayData.Count == 0)
			return;

		SwayData sway = mSwayData.Dequeue();
		
		for(int meshIndx = 0; meshIndx < sway.Meshes.Length; ++meshIndx)
		{
			//sway.Meshes[meshIndx].Clear();
			sway.Meshes[meshIndx] = null;
		}
		
		mSwayDataPool.Enqueue(sway);
	}

	void DrawSway()
	{
		if (mSwayData.Count != 0)
		{
			SwayData[] swayList = mSwayData.ToArray ();
			
			for (int Indx = 0; Indx < swayList.Length; ++Indx)
			{
				for (int meshIndx = 0; meshIndx < swayList[Indx].Meshes.Length; ++meshIndx)
				{
					Graphics.DrawMesh(swayList[Indx].Meshes[meshIndx], swayList[Indx].Position, swayList[Indx].Direction, SwayMaterial, 0);
				}
			}
		}
	}
}
