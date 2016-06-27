using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    PlayerMove moveScript;
    Transform mainCam;
    private Quaternion screenMovementSpace;
    private Vector3 direction, screenMovementForward, screenMovementRight;
    #endregion

    #region PublicProperties
    public bool JumpPressed { get; set; }
    #endregion

    #region UnityFunctions
    void Start()
        {
        mainCam = Camera.main.transform;
        moveScript.PlayerInput = this;
        }

        void Update()
        {
            MovementInput();
            JumpPressed = JumpInput();

        }

    void OnDisable()
    {
        direction = (screenMovementForward * 0) + (screenMovementRight * 0);
        moveScript.Direction = direction;
        moveScript.MoveDirection = transform.position + direction;
    }
    #endregion

    #region CustomFunctions


    bool JumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    void MovementInput()
    {
        screenMovementSpace = Quaternion.Euler(0, mainCam.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;

        //get movement input, set direction to move in
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        direction = (screenMovementForward * v) + (screenMovementRight * h);
        moveScript.Direction = direction;
        moveScript.MoveDirection = transform.position + direction;
    }
    #endregion
}