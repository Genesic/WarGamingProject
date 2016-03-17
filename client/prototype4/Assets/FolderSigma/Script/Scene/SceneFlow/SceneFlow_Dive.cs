using UnityEngine;
using System.Collections;

public class SceneFlow_Dive : ISceneFlow 
{
	int mDiveInCount = 0;

	public void Initial()
	{
		mDiveInCount = 0;
	}

	public void EnterFlow()
	{
		SceneController.SetRoleCommand(SceneControllerType.Player, RoleCommand.Action, (System.Object)RoleAction.Dive);

		SceneController.SetRoleCommand(SceneControllerType.Enemy, RoleCommand.Action, (System.Object)RoleAction.Dive);

		SceneController.SetCameraCommand (SceneControllerType.Player, CameraCommand.CameraType_Dive);

		SceneController.SetCameraCommand (SceneControllerType.Enemy, CameraCommand.CameraType_Dive);
	}
	
	public void LeaveFlow()
	{
	}
	
	public void UpdateFlow()
	{
	}
	
	public void AnimEventHandle(IRole _sender, string _msg)
	{
		switch (_msg) 
		{
		case AnimEventMessage.DIVE_OUT:

			++mDiveInCount;

			if(1 < mDiveInCount)
				SceneController.SetSceneFlow(SceneFlowType.Ready);
			break;
		}
	}
}
