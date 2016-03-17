using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwordEffect))]
public class SwordEffectEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		bool needSetDirty = false;

		SwordEffect Target = (SwordEffect)target;

		//SwordEffect.SwordEffectCollectionType type = Target.CollectionType;

		Target.Head = (Transform)EditorGUILayout.ObjectField("軌跡起點", Target.Head, typeof(Transform), true);
		Target.Tail = (Transform)EditorGUILayout.ObjectField("軌跡終點", Target.Tail, typeof(Transform), true);
		Target.MatEffect = (Material)EditorGUILayout.ObjectField("使用材質", Target.MatEffect, typeof(Material), true);

		SwordEffect.SwordEffectCollectionType resType = (SwordEffect.SwordEffectCollectionType)EditorGUILayout.EnumPopup ("軌跡收集方式：", Target.CollectionType);
		if(Target.CollectionType != resType)
		{
			Target.CollectionType = resType ;
			needSetDirty = true;
		}

		if (Target.CollectionType == SwordEffect.SwordEffectCollectionType.FrameInterval) 
		{
			int resMax =  EditorGUILayout.IntField("最大軌跡數量：", Target.MaxSwordEffectPt);
			if(Target.MaxSwordEffectPt != resMax)
			{
				Target.MaxSwordEffectPt = resMax ;
				needSetDirty = true;
			}
		}
		else if (Target.CollectionType == SwordEffect.SwordEffectCollectionType.TimeInterval)
		{
			float disLen =  EditorGUILayout.FloatField("拋棄頻率(秒)：", Target.DiscardLength);
			if(Target.DiscardLength != disLen)
			{
				Target.DiscardLength = disLen ;
				needSetDirty = true;
			}
		}

		bool needSmooth =  EditorGUILayout.Toggle("開啟 Smooth", Target.UseSmooth);
		if(Target.UseSmooth != needSmooth)
		{
			Target.UseSmooth = needSmooth ;
			needSetDirty = true;
		}

		bool isCollectoion =  EditorGUILayout.Toggle("開啟收集結點", Target.IsCollection);
		if(Target.IsCollection != isCollectoion)
		{
			Target.IsCollection = isCollectoion ;
			needSetDirty = true;
		}

		EditorGUILayout.LabelField ("累積節點數：" + Target.SwordEffectPtCount);

		if (needSetDirty)
			EditorUtility.SetDirty (Target);
	}
}
