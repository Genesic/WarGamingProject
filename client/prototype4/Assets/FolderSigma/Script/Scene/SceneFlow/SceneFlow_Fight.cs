using UnityEngine;
using System.Collections;

public class SceneFlow_Fight : ISceneFlow 
{		
	public void Initial()
	{
	}
	
	public void EnterFlow()
	{		
		SceneController.SetCameraCommand (SceneControllerType.Player, CameraCommand.CameraType_Fight);
		
		SceneController.SetCameraCommand (SceneControllerType.Enemy, CameraCommand.CameraType_Fight);
	}
	
	public void LeaveFlow()
	{
	}
	
	public void UpdateFlow()
	{
	}
	
	public void AnimEventHandle(IRole _sender, string _msg)
	{
		/*
		if (mData == null) return;
		
		switch (_msg) 
		{
		case AnimEventMessage.ATTACK_OUT:
			
			_sender.SetCommand(RoleCommand.Action,(Object)RoleAction.Fight);
			break;
		}
		*/
	}
}
