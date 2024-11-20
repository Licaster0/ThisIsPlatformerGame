using UnityEngine;
public class JumpState : State
{
    public JumpState(CharacterController2D character) : base(character) { }
    public bool canDoubleJump = true;
    private float doubleJumpForceMultiplier = 0.8f; // �kinci z�plaman�n g�c�n� belirleyen �arpan

    public override void EnterState()
    {
        Debug.Log("Jump State Entered");

        // Karakteri z�plat
        character.Jump();
        character.animator.SetBool("isJumping", true);
        //character.DoubleJumpState.canDoubleJump = true;
        canDoubleJump = true;
    }

    public override void UpdateState()
    {

        // E�er karakter yere d�nerse, Idle durumuna ge�
        if (character.IsGrounded())
        {
            character.TransitionToState(character.IdleState);
        }
        // E�er z�plarken sa�a sola hareket etmek istiyorsak
        else if (Input.GetAxis("Horizontal") != 0)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            character.Move(horizontalInput);
            //character.TransitionToState(character.DoubleJumpState);
        }
        // E�er d��me durumuna ge�ilecek bir kontrol eklemek istersek
        else if (character.GetVerticalVelocity() < 0)
        {
            // FallState eklenirse buraya ge�i� yap�labilir
        }
        if (character.IsGrounded())
        {
            character.TransitionToState(character.IdleState);
        }
        else if (Input.GetButtonDown("Jump") && !character.IsGrounded() && canDoubleJump)
        {
            character.Jump(doubleJumpForceMultiplier); // Daha zay�f z�plama i�in kuvvet �arpan� g�nder
            canDoubleJump = false;

            // Takla animasyonu
            character.animator.SetTrigger("doFlip");

            // �rnek: Takla s�ras�nda bir g�rsel efekt tetiklemek
            character.flipEffect.Play(); // FlipEffect, karakterin �zel bir ParticleSystem bile�eni olabilir.
        }

    }

    public override void ExitState()
    {
        character.animator.SetBool("isJumping", false);
        character.jumpParticle.Play();

        Debug.Log("Exiting Jump State");
    }
}
