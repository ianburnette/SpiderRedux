using UnityEngine;
using System.Collections;

public class PlayerBoatControls : CharacterMotor {

    #region PrivateFields
    [SerializeField]
    private GameObject boat;
    [SerializeField]
    private float turnSpeed, velMargin;
    #endregion

    #region PublicProperties
          
    #endregion

    #region UnityFunctions
        void Start()
        {
        rigid = boat.GetComponent<Rigidbody>();
        }

        void Update()
        {
            
        }
    #endregion

    #region CustomFunctions
    public override bool MoveTo(Vector3 destination, float acceleration, float stopDistance, bool ignoreY)
    {
        //print("moving to " + destination);
        Vector3 relativePos = (destination - transform.position);
        if (ignoreY)
            relativePos.y = 0;

        DistanceToTarget = relativePos.magnitude;
        if (DistanceToTarget <= stopDistance)
            return true;
        else
            rigid.AddForce(relativePos.normalized * acceleration * Time.deltaTime, ForceMode.VelocityChange);
        return false;
    }

    //rotates rigidbody to face its current velocity
    public override void RotateToVelocity(float turnSpeed, bool ignoreY)
    {
        Vector3 dir;
        if (ignoreY)
            dir = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        else
            dir = rigid.velocity;

        if (dir.magnitude > velMargin)
        {
            Quaternion dirQ = Quaternion.LookRotation(dir);
            Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, dir.magnitude * turnSpeed * Time.deltaTime);
            rigid.MoveRotation(slerp);
        }
    }

    //rotates rigidbody to a specific direction
    public override void RotateToDirection(Vector3 lookDir, float turnSpeed, bool ignoreY)
    {
        RotateToVelocity(turnSpeed, true);
        /*
        Vector3 characterPos = transform.position;
        if (ignoreY)
        {
            characterPos.y = 0;
            lookDir.y = 0;
        }

        Vector3 newDir = lookDir - characterPos;
        Quaternion dirQ = Quaternion.LookRotation(newDir);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, turnSpeed * Time.deltaTime);
        rigid.MoveRotation(slerp);*/
    }

    // apply friction to rigidbody, and make sure it doesn't exceed its max speed
    public override void ManageSpeed(float deceleration, float maxSpeed, bool ignoreY)
    {
        currentSpeed = rigid.velocity;
        if (ignoreY)
            currentSpeed.y = 0;

        if (currentSpeed.magnitude > 0)
        {
            rigid.AddForce((currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
            if (rigid.velocity.magnitude > maxSpeed)
                rigid.AddForce((currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
    #endregion
}