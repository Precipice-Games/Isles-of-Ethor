using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right; //direction of movement
    public float moveDistance = 15f; // how far it travels
    public float speed = 12f; // speed

    public bool isActive = false;

    private Vector3 startPos;
    private Vector3 lastPos;

    [SerializeField]
    private CharacterController charController;

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
    }

    void Update()
    {
        if (!isActive) return;

        float movement = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + moveDirection.normalized * movement;
    }

    // Move the player along with platform after platform has moved this frame
    void LateUpdate()
    {
        Vector3 delta = transform.position - lastPos;
        if (delta != Vector3.zero)
        {
            if (charController != null)
            {
                charController.Move(delta);
            }
        }

        lastPos = transform.position;
    }

    public void ActivatePlatform() //Attach player to platform
    {
        isActive = true;
    }

    // Note: parenting the moving platform to the object is not ideal because it interferes with characterController behavior
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            charController = other.gameObject.GetComponent<CharacterController>();
        }
    }

    private void OnCollisionExit(Collision other) //Detach player when leaving
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (charController != null)
            {
                charController = null;
            }
        }
    }

    public void Set(CharacterController controller)
    {
        charController = controller;
    }

    public void Clear(CharacterController controller)
    {
        if (charController == controller)
        {
            charController = null;
        }
    }
}