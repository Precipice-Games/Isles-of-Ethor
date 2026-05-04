using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerControlsInputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;

	/// <summary>
	/// Takes the player's keyboard input in context as a Vector2 and
	/// assigns it to the move variable, which is used in the Move() method.
	/// </summary>
	/// <param name="context"></param>
	public void PlayerMove(InputAction.CallbackContext context)
	{
		move = context.ReadValue<Vector2>();
	}
	
	/// <summary>
	/// Takes in the Player's mouse pointer (delta) input in as a
	/// Vector2 and assigns it to lookX, which defines the turning
	/// direction. Then we multiply that value by the mouseSensitivity
	/// and apply it to turnInput. This comes to fruition in the
	/// FixedUpdate() method to physically rotate the Player.
	/// </summary>
	/// <param name="context"></param>
	public void PlayerLook(InputAction.CallbackContext context)
	{
		look = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// Takes the player's jump input in the context parameter
	/// then checks if context was just performed and that the
	/// player has a rigidbody variable and if both are true
	/// the players vertical velocity gets boosted by jumpPower
	/// </summary>
	/// <param name="context"></param>
	public void PlayerJump(InputAction.CallbackContext context)
	{
		// if (context.performed && isGrounded)
		// {
		//     Debug.Log("PlayerMovement.cs >> Jump performed.");
		// }

		if (context.performed)
		{
			jump = true;
			Debug.Log("PlayerMovement.cs >> Jump performed.");
		}
		else if (context.canceled)
		{
			jump = false;
			Debug.Log("PlayerMovement.cs >> Jump canceled.");
		}
        
		// Normally, we would run the following:
		// if (context.performed && isGrounded && jumpsRemaining > 0)
		// {
		//     rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
		// }
		// However, because I'm trying to use the character controller, I'm going to see
		// if that can be handled in the JumpAndGravity() method instead.
	}
}