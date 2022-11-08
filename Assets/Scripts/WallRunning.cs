using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask Wall;
    public LayerMask Ground;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.Q;
    public KeyCode downwardsRunKey = KeyCode.E;
    private bool upwardsRunning;
    private bool downwardsRunning;
     
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    public bool exitingwall;
    public float exitWallTime;
    public float exitWallTimer;

    [Header("Reference")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody PlayerRb;

    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, Wall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, Wall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        //state 1 Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingwall)
        {
            if (!pm.wallrunning)
                StartWallrun();

            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        // State 2 Exiting
        else if (exitingwall)
        {
            if(pm.wallrunning) StopWallRun();

            if(exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if(exitWallTimer <= 0)
                exitingwall = false;
        }
        //state 3 - None
        else
        {
            if(pm.wallrunning)
                StopWallRun();
        }
    }
    private void StartWallrun()
    {
        pm.wallrunning = true;
    }

    private void WallRunningMovement()
    {
        PlayerRb.useGravity = false;
        PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward; //si la orientacion se vuevle negativa, el wallforward se vuevle negativo
                                        //de esta forma no ira en reverso al detectarlo en derecho
        
        PlayerRb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if(upwardsRunning)
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, wallClimbSpeed, PlayerRb.velocity.z);
        if (downwardsRunning)
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, -wallClimbSpeed, PlayerRb.velocity.z);


        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            PlayerRb.AddForce(-wallNormal * 100, ForceMode.Force);
    }
    private void StopWallRun()
    { 
        pm.wallrunning = false;
    }

    private void WallJump()
    {
        exitingwall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 forcetoApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //reset
        PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, 0f, PlayerRb.velocity.z);
        PlayerRb.AddForce(forcetoApply, ForceMode.Impulse);
    }
}
