using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnityChanController : MonoBehaviour, IRole
{
	Animator mAnim = null;

	int AttackTypeId = 0;
	int ActionTypeId = 0;
	int triggerActionId = 0;


	[System.Serializable]
	class UnityChanMotionData
	{
		public SceneControllerType ControllerType = SceneControllerType.Player;
		public Transform StandPos = null;
		public Transform AttackPos_Hand = null;
		public Transform AttackPos_Foot = null;
	}

	[SerializeField]
	UnityChanMotionData mMotionData = new UnityChanMotionData();

	Renderer[] mRenderers = null;

	Action<IRole, string> mAnimEventCallback = null;
	public Action<IRole, string> AnimEventCallback { set { mAnimEventCallback = value; } }

	Action<Animator> mAnimUpdateCallback = null;

	Queue<RoleAction> mActQueue = new Queue<RoleAction>();

	bool mIsIdle = true;

	// Use this for initialization
	void Start () 
	{
		mIsIdle = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mIsIdle == true && mActQueue.Count != 0) 
		{
			SetAction(mActQueue.Dequeue());
		}	
	}

	void OnAnimatorMove ()
	{
		if (mAnim == null)
			return;

		this.transform.position += new Vector3 (0F,mAnim.deltaPosition.y,0F);
		this.transform.rotation *= mAnim.deltaRotation;
	}

	void AnimEventHandle(string _msg)
	{
		switch(_msg)
		{
		case AnimEventMessage.IDLE_IN:
			mIsIdle = true;
			break;

		case AnimEventMessage.IDLE_OUT:
			mIsIdle = false;
			break;

		case AnimEventMessage.DIVE_IN:
			break;

		case AnimEventMessage.DIVE_OUT:
			break;

		case AnimEventMessage.ATTACK_IN:
			break;

		case AnimEventMessage.ATTACK_OUT:

			this.transform.position = mMotionData.StandPos.position;
			this.transform.rotation = mMotionData.StandPos.rotation;
			break;

			/*
		case AnimEventMessage.CREATE_HAND_SHOCK:

			if(mMotionData.AttackPos_Hand != null) 
			{
				Vector3 screenPos = Camera.main.WorldToScreenPoint(mMotionData.AttackPos_Hand.position);

				SceneController.SetCameraCommand(mMotionData.ControllerType, 
				                                 CameraCommand.PostEffect_Shock, 
				                                 (System.Object)(screenPos.x/Camera.main.pixelWidth),
				                                 (System.Object)(screenPos.y/Camera.main.pixelHeight),
				                                 (System.Object)2F,
				                                 (System.Object)0F);
			}
			break;

		case AnimEventMessage.CREATE_FOOT_SHOCK:

			if(mMotionData.AttackPos_Foot != null) 
			{
				Vector3 screenPos = Camera.main.WorldToScreenPoint(mMotionData.AttackPos_Foot.position);
				
				SceneController.SetCameraCommand(mMotionData.ControllerType, 
				                                 CameraCommand.PostEffect_Shock, 
				                                 (System.Object)(screenPos.x/Camera.main.pixelWidth),
				                                 (System.Object)(screenPos.y/Camera.main.pixelHeight),
				                                 (System.Object)2F,
				                                 (System.Object)0F);
			}
			break;
			*/
		}

		if (mAnimEventCallback != null)
			mAnimEventCallback (this, _msg);
	}

	void InitialAnim()
	{
		mAnim = GetComponent<Animator>();	
		Monitor[] monitors = mAnim.GetBehaviours<Monitor>();
		for (int Indx = 0; Indx < monitors.Length; ++Indx)
			monitors [Indx].AnimationEventFunc = AnimEventHandle;

		AttackTypeId = Animator.StringToHash ("AttackType");
		ActionTypeId = Animator.StringToHash ("ActionType");
		triggerActionId = Animator.StringToHash ("triggerAction");
	}

	void InitialRender()
	{
		mRenderers = GetComponentsInChildren<Renderer>();
	}

	void SetAction(RoleAction _act)
	{
		switch(_act)
		{
		case RoleAction.Dive:
			SetDive();
			break;

		case RoleAction.Fight:
			SetAttack();
			break;
		}
	}

	void SetDive()
	{
		mAnim.SetInteger(ActionTypeId, 1);
		mAnim.SetTrigger(triggerActionId);
	}

	void SetAttack()
	{
		mAnim.SetInteger(AttackTypeId, UnityEngine.Random.Range(0,5));
		mAnim.SetInteger(ActionTypeId, 3);
		mAnim.SetTrigger(triggerActionId);
	}

	public void Display(bool _isShow)
	{
		for(int Indx = 0; Indx < mRenderers.Length; ++Indx)
		{
			mRenderers[Indx].enabled = _isShow;
		}
	}
	
	void AddAction(RoleAction _act)
	{
		mActQueue.Enqueue (_act);
	}

	#region IRole
	public void Initial()
	{
		InitialAnim ();
		
		InitialRender ();
	}

	public void SetCommand(RoleCommand _command, params System.Object[] _params)
	{
		switch(_command)
		{
		case RoleCommand.Display:
			Display((bool)_params[0]);
			break;

		case RoleCommand.Action:
			AddAction((RoleAction)_params[0]);
			break;
		}
	}
	#endregion
}
