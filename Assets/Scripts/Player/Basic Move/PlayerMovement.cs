using System;
using System.Collections;
using System.Collections.Generic;
using SpriteGlow;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Members

    #region For Movement

    public CharacterController controller;
    private Vector3 move;

    public float speed = 12f;
    public float airSpeedReduction = 7f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    #endregion
    
    #region For Dash
    
    [SerializeField] private AnimationCurve _dashSpeedCurve;
    private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    private bool _inDash = false;
    private float _dashContador = 0;
    [SerializeField] private GameObject _amaGameObject;
    
    #endregion

    #region For Jump

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool _isGrounded;
    private bool inTheAir = false;

    #endregion

    #region For Smothness
    
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myInput;
    [SerializeField] private float smoothInputSpeed;
    
    #endregion

    #region For WallJump

    private bool wallJumpFlag;
    private bool _allowWallJumpInput;
    private GameObject _currentWall;

    #endregion
    
    #endregion
    
    void Update()
    {
        #region Rotation in Y axis

        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        
        #endregion
        
        #region Wall Jump

        if (_isGrounded) { _currentWall = null; }
        
        if (wallJumpFlag && velocity.y < -2.5f && inTheAir) //Cuando la velocity.y es menor a -2.5 es porque el jugador está cayendo: -3,-4.-10
        {
            velocity.y = 0f;
            _allowWallJumpInput = true;
            _wallJumpTimerRoutine = null;
            _wallJumpTimerRoutine = WallJumpTimer(1.5f);
            StartCoroutine(_wallJumpTimerRoutine);
        }
        if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame() && _allowWallJumpInput)
        {
            WallJump();                
        }
        
        #endregion

        #region Jump
        
        else if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame() == true && _isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            speed -= airSpeedReduction;
            inTheAir = true;
        }
        
        #endregion
        
        #region Dash
        
        if (_dashContador <= _dashCooldown) { _dashContador += Time.deltaTime; }
        if (ActionMapReference.playerMap.MovimientoAvanzado.Dash.WasPerformedThisFrame() && _dashContador >= _dashCooldown && _amaGameObject.activeInHierarchy)
        {
            StartCoroutine(Dash());
        }
        
        #endregion

        #region GroundCheck
        
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (inTheAir)
            {
                speed += airSpeedReduction;
                inTheAir = false;
            }
        }
        
        #endregion
        
        #region Movimiento
        
        myInput.x = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x;
        myInput.z = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y;

        currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
        move = transform.right * currentInputVector.x + transform.forward * currentInputVector.z;

        if (_allowWallJumpInput == false)
        {
            controller.Move(move * speed * Time.deltaTime); //Movimiento horizontal (Caminar)
            controller.Move(velocity * Time.deltaTime); //Movimiento vertical (Salto)
        }

        if (_allowWallJumpInput == false && _inDash == false) { velocity.y += gravity * Time.deltaTime; }
        
        #endregion
    }

    private IEnumerator Dash()
    {
        float _dashSpeedOverTime = 0f;
        float startTime = Time.time;
        
        while (Time.time <= startTime + _dashTime && move != new Vector3(0f,0f,0f))
        {
            _inDash = true;
            velocity.y = 0;
            _dashSpeed = _dashSpeedCurve.Evaluate(_dashSpeedOverTime);
            _dashSpeedOverTime+=0.01f;
            controller.Move(move * _dashSpeed * Time.deltaTime);
            _dashContador = 0;

            #region Raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.transform.CompareTag("Crossable"))
                {
                    hit.collider.isTrigger = true; 
                    yield return new WaitForSeconds(_dashTime); 
                    hit.collider.isTrigger = false;
                }
            }
            #endregion
            
            yield return null;
        }
        _inDash = false;
    }

    private void WallJump()
    {
        wallJumpFlag = false;
        _allowWallJumpInput = false;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //Hacer un salto
        StopCoroutine(_wallJumpTimerRoutine);
    }

    private IEnumerator _wallJumpTimerRoutine;
    private IEnumerator WallJumpTimer(float time)
    {
        yield return new WaitForSeconds(time);
        wallJumpFlag = false;
        _allowWallJumpInput = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("WallJump") && _amaGameObject.activeInHierarchy)
        {
            if (_currentWall == collision.collider.gameObject) { return; }
            _currentWall = collision.collider.gameObject;
            
            wallJumpFlag = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("WallJump") && _amaGameObject.activeInHierarchy)
        {
            wallJumpFlag = false;
            _allowWallJumpInput = false;
        }
    }
}
