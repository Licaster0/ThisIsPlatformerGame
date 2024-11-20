using UnityEngine;
public class RunState : State
{
    public RunState(CharacterController2D character) : base(character) { }

    public override void EnterState()
    {
        Debug.Log("Run State Entered");
        character.animator.SetBool("isRunning", true);
        character.animator.SetBool("isIdle", false);
    }

    public override void UpdateState()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        character.Move(horizontalInput);

        if (horizontalInput == 0)
            character.TransitionToState(character.IdleState);

        if (Input.GetButtonDown("Jump") && character.IsGrounded())
            character.TransitionToState(character.JumpState);
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Run State");
        character.animator.SetBool("isRunning", false);
    }
}
