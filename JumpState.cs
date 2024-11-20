using UnityEngine;
public class JumpState : State
{
    public JumpState(CharacterController2D character) : base(character) { }
    public bool canDoubleJump = true;
    private float doubleJumpForceMultiplier = 0.8f; // Ýkinci zýplamanýn gücünü belirleyen çarpan

    public override void EnterState()
    {
        Debug.Log("Jump State Entered");

        // Karakteri zýplat
        character.Jump();
        character.animator.SetBool("isJumping", true);
        //character.DoubleJumpState.canDoubleJump = true;
        canDoubleJump = true;
    }

    public override void UpdateState()
    {

        // Eðer karakter yere dönerse, Idle durumuna geç
        if (character.IsGrounded())
        {
            character.TransitionToState(character.IdleState);
        }
        // Eðer zýplarken saða sola hareket etmek istiyorsak
        else if (Input.GetAxis("Horizontal") != 0)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            character.Move(horizontalInput);
            //character.TransitionToState(character.DoubleJumpState);
        }
        // Eðer düþme durumuna geçilecek bir kontrol eklemek istersek
        else if (character.GetVerticalVelocity() < 0)
        {
            // FallState eklenirse buraya geçiþ yapýlabilir
        }
        if (character.IsGrounded())
        {
            character.TransitionToState(character.IdleState);
        }
        else if (Input.GetButtonDown("Jump") && !character.IsGrounded() && canDoubleJump)
        {
            character.Jump(doubleJumpForceMultiplier); // Daha zayýf zýplama için kuvvet çarpaný gönder
            canDoubleJump = false;

            // Takla animasyonu
            character.animator.SetTrigger("doFlip");

            // Örnek: Takla sýrasýnda bir görsel efekt tetiklemek
            character.flipEffect.Play(); // FlipEffect, karakterin özel bir ParticleSystem bileþeni olabilir.
        }

    }

    public override void ExitState()
    {
        character.animator.SetBool("isJumping", false);
        character.jumpParticle.Play();

        Debug.Log("Exiting Jump State");
    }
}
