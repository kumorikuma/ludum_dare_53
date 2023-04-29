using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiseBehavior : StateMachineBehaviour {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Desired speed is the cruising speed of the car
        GameObject carObj = animator.gameObject;
        CarData carData = CarManager.Instance.GetCharData(carObj);
        CarData followingCar = CarManager.Instance.GetCarInFrontOf(carData.position, carData.lane);

        float distanceToCar = followingCar.position.y - carData.position.y;
        // TODO: Implement variable follow distance based on distance and time
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
