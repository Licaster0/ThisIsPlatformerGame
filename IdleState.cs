using UnityEngine;
public class IdleState : State
{
    public IdleState(CharacterController2D character) : base(character) { }

    public override void EnterState()
    {
        Debug.Log("Idle State Entered");
        character.animator.SetBool("isRunning", false);

    }

    public override void UpdateState()
    {
        // Hareket ve zýplama kontrolleri
        if (Input.GetAxis("Horizontal") != 0)
            character.TransitionToState(character.RunState);

        if (Input.GetButtonDown("Jump") && character.IsGrounded())
            character.TransitionToState(character.JumpState);
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle State");
    }
}
