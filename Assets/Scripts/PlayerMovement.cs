using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject Floor;
    private Vector3 respawnPoint;

    [Header("Movement")]
    private float moveSpeed; //esta variable es la que determina y guarda el movimiento siendo modificada por el resto
    public float walkSpeed;
    public float sprintSpeed;
    public float wallrunSpeed;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    bool readyToJump = true;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Grounded")]
    public float groundDrag;
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput, verticalInput;

    Vector3 Direction;

    Rigidbody playerRb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        wallrunning,
        sprinting,
        air,
    }

    public bool sliding;
    public bool wallrunning;
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        respawnPoint = transform.position;

    }
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        //raycast para hallar el suelo
        MoveInputs();
        SpeedControl();
        StateHandler();

        if (grounded)
            playerRb.drag = groundDrag;
        else
            playerRb.drag = 0;
    }

    void MoveInputs()
    {
        //los inputs se separan del movimiento ya que el movimiento usando el rigidbody debe estar en el FixedUpdate
        //a diferencia de los inputs que ocurren constantemente y estar en el fixedUpdate puede hacerlos menos responsivos en los saltos
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded) //si he presionado el boton, puedo saltar y ademas estoy en el suelo.
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);//reinicia el salto dependiendo de cada cuanto lo llamas.
        }

    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }

        if (grounded && Input.GetKey(sprintKey)) //Sprinteando
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)  //si no estas presionando el boton de sprint, estas caminando
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else //si nada mas es llamado, significa que estas en el aire.
        {
            state = MovementState.air;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        Direction = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (Onslope() && !exitingSlope)
        {
            playerRb.AddForce(20f * moveSpeed * GetSlopeDirection(), ForceMode.Force);

            if (playerRb.velocity.y > 0)
                playerRb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (grounded) //cuando estoy en el suelo
            playerRb.AddForce(5f * moveSpeed * Direction.normalized, ForceMode.Force);

        else if (!grounded) //cuando no estoy en el suelo
            playerRb.AddForce(5f * airMultiplier * moveSpeed * Direction.normalized, ForceMode.Force);

        playerRb.useGravity = !Onslope();

    }
    void SpeedControl()
    {
        //limitar la velocidad en cuando hay 
        if (Onslope())
        {
            if (playerRb.velocity.magnitude > moveSpeed)
                playerRb.velocity = playerRb.velocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 flatVel = new(playerRb.velocity.x, 0f, playerRb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                playerRb.velocity = new Vector3(limitedVel.x, playerRb.velocity.y, limitedVel.z);
            }
        }

    }

    void Jump()
    {
        exitingSlope = true;

        playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
        //siempre saltara la misma distancia de esta manera.
        playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool Onslope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(Direction, slopeHit.normal).normalized;
    }

    //Respawn

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HurtF"))
        {
            transform.position = respawnPoint;
        }
        else if (other.CompareTag("Spawn"))
        {
            respawnPoint = transform.position;
        }
    }

}
