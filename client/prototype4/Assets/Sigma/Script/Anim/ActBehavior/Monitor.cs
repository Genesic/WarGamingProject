using UnityEngine;
using System.Collections;

public class Monitor : StateMachineBehaviour 
{
	public enum StateType
	{
		State,
		StateMachine,
	}
	
	[SerializeField]
	private StateType mStateType = StateType.State;
	
	public delegate void AnimationEventTiming(string _ani);
	/// <summary>
	/// 動作完成後的callback
	/// </summary>
	public AnimationEventTiming AnimationEventFunc = null;
	
	[SerializeField]
	private string mAnimationStartMessage = string.Empty;
	[SerializeField]
	private string mAnimationEndMessage = string.Empty;
	
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
		if (mStateType == StateType.State && AnimationEventFunc != null && string.IsNullOrEmpty (mAnimationStartMessage) == false)
			AnimationEventFunc (mAnimationStartMessage);	
	}
	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
		if (mStateType == StateType.State && AnimationEventFunc != null && string.IsNullOrEmpty (mAnimationEndMessage) == false)
			AnimationEventFunc (mAnimationEndMessage);
	}
	
	// OnStateMachineEnter is called when entering a statemachine via its Entry Node
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
		
		if (mStateType == StateType.StateMachine && AnimationEventFunc != null && string.IsNullOrEmpty (mAnimationStartMessage) == false)
			AnimationEventFunc (mAnimationStartMessage);	
	}
	
	// OnStateMachineExit is called when exiting a statemachine via its Exit Node
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
		
		if (mStateType == StateType.StateMachine && AnimationEventFunc != null && string.IsNullOrEmpty (mAnimationEndMessage) == false)
			AnimationEventFunc (mAnimationEndMessage);
	}
	
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
