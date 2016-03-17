using UnityEngine;
using System.Collections;

public class SceneFlow_Ready : ISceneFlow 
{
	const float STAY_TIME = 0.5F;

	float mTime = 0;
	
	public void Initial()
	{
	}
	
	public void EnterFlow()
	{		
		mTime = STAY_TIME;
		
		SceneController.SetCameraCommand (SceneControllerType.Player, CameraCommand.CameraType_Ready);
		
		SceneController.SetCameraCommand (SceneControllerType.Enemy, CameraCommand.CameraType_Ready);
	}
	
	public void LeaveFlow()
	{
	}
	
	public void UpdateFlow()
	{
		mTime -= Time.deltaTime;
		if (mTime <= 0F)
			SceneController.SetSceneFlow(SceneFlowType.Fight);
	}
	
	public void AnimEventHandle(IRole _sender, string _msg)
	{
	}
}
