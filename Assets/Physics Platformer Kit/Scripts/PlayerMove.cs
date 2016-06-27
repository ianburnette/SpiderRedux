using UnityEngine;
using System.Collections;

//handles player movement, utilising the CharacterMotor class
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour 
{

    #region PreexistingFields
    //setup
    public bool sidescroller;					//if true, won't apply vertical input
	public Transform mainCam, floorChecks;		//main camera, and floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
	public Animator animator;					//object with animation controller on, which you want to animate
	public AudioClip jumpSound;					//play when jumping
	public AudioClip landSound;					//play when landing on ground
	
	//movement
	public float accel = 70f;					//acceleration/deceleration in air or on the ground
	public float airAccel = 18f;			
	public float decel = 7.6f;
	public float airDecel = 1.1f;
	[Range(0f, 5f)]
	public float rotateSpeed = 0.7f, airRotateSpeed = 0.4f;	//how fast to rotate on the ground, how fast to rotate in the air
	public float maxSpeed = 9;								//maximum speed of movement in X/Z axis
	public float slopeLimit = 40, slideAmount = 35;			//maximum angle of slopes you can walk on, how fast to slide down slopes you can't
	public float movingPlatformFriction = 7.7f;				//you'll need to tweak this to get the player to stay on moving platforms properly
	
	//jumping
	public Vector3 jumpForce =  new Vector3(0, 13, 0);		//normal jump force
	public Vector3 secondJumpForce = new Vector3(0, 13, 0); //the force of a 2nd consecutive jump
	public Vector3 thirdJumpForce = new Vector3(0, 13, 0);	//the force of a 3rd consecutive jump
	public float jumpDelay = 0.1f;							//how fast you need to jump after hitting the ground, to do the next type of jump
	public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
	[HideInInspector]
	public int onEnemyBounce;					
	
	private int onJump;
	private bool grounded;
	private Transform[] floorCheckers;
	private Quaternion screenMovementSpace;
	private float airPressTime, groundedCount, curAccel, curDecel, curRotateSpeed, slope;
	private Vector3 direction, moveDirection, screenMovementForward, screenMovementRight, movingObjSpeed;
	
	private CharacterMotor characterMotor;
	private EnemyAI enemyAI;
	private DealDamage dealDamage;
	private Rigidbody rigid;
	private AudioSource aSource;
    #endregion

    #region CustomFields
   
    [SerializeField]
    private Vector3 slopeNormal;
    [SerializeField]
    private float slopeCorrectionAmt, slopeMargin;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private PlayerInput playerInput;
    #endregion

    #region PublicProperties
    public string GroundTag { get; set; }
    public Vector3 GroundHitPosition { get; set; }
    public Vector3 MoveDirection{get{return moveDirection;}set{moveDirection = value;}}
    public Vector3 Direction { get { return direction; } set { direction = value; } }
    public PlayerInput PlayerInput{get{return playerInput;}set{playerInput = value;}}
    public bool InWater { get; set; }
    public CharacterMotor CharacterMotor{get{return characterMotor;}set{characterMotor = value;}}
    #endregion

    #region UnityFunctions
    //setup
    void Awake()
	{
		//create single floorcheck in centre of object, if none are assigned
		if(!floorChecks)
		{
			floorChecks = new GameObject().transform;
			floorChecks.name = "FloorChecks";
			floorChecks.parent = transform;
			floorChecks.position = transform.position;
			GameObject check = new GameObject();
			check.name = "Check1";
			check.transform.parent = floorChecks;
			check.transform.position = transform.position;
			Debug.LogWarning("No 'floorChecks' assigned to PlayerMove script, so a single floorcheck has been created", floorChecks);
		}
		//assign player tag if not already
		if(tag != "Player")
		{
			tag = "Player";
			Debug.LogWarning ("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
		}
		//usual setup
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		dealDamage = GetComponent<DealDamage>();
		characterMotor = GetComponent<CharacterMotor>();
		rigid = GetComponent<Rigidbody>();
		aSource = GetComponent<AudioSource>();
		//gets child objects of floorcheckers, and puts them in an array
		//later these are used to raycast downward and see if we are on the ground
		floorCheckers = new Transform[floorChecks.childCount];
		for (int i=0; i < floorCheckers.Length; i++)
			floorCheckers[i] = floorChecks.GetChild(i);
	}
	
	//get state of player, values and input
	void Update()
	{	
		//stops rigidbody "sleeping" if we don't move, which would stop collision detection
		rigid.WakeUp();
		//handle jumping
		JumpCalculations ();
		//adjust movement values if we're in the air or on the ground
		curAccel = (grounded) ? accel : airAccel;
		curDecel = (grounded) ? decel : airDecel;
		curRotateSpeed = (grounded) ? rotateSpeed : airRotateSpeed;
				
        /*
		//get movement axis relative to camera
		screenMovementSpace = Quaternion.Euler (0, mainCam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
		
		//get movement input, set direction to move in
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		
		//only apply vertical input to movemement, if player is not sidescroller
		if(!sidescroller)
			direction = (screenMovementForward * v) + (screenMovementRight * h);
		else
			direction = Vector3.right * h;
		moveDirection = transform.position + direction;
        */
	}
	
	//apply correct player movement (fixedUpdate for physics calculations)
	void FixedUpdate() 
	{
        //correct for input disabled
        if (playerInput.enabled == false)
        {
            //direction = ;
            moveDirection = transform.position;
        }
		//are we grounded
		grounded = IsGrounded ();
        //move, rotate, manage speed
        if (Mathf.Abs(slopeNormal.x) > slopeMargin || Mathf.Abs(slopeNormal.z) > slopeMargin)
            CorrectForSlope(slopeNormal);
        
        characterMotor.MoveTo (moveDirection, curAccel, 0.7f, true);
		if (rotateSpeed != 0 && direction.magnitude != 0)
			characterMotor.RotateToDirection (moveDirection , curRotateSpeed * 5, true);
		characterMotor.ManageSpeed (curDecel, maxSpeed + movingObjSpeed.magnitude, true);
		//set animation values
		if(animator)
		{
			animator.SetFloat("DistanceToTarget", characterMotor.DistanceToTarget);
			animator.SetBool("Grounded", grounded);
			animator.SetFloat("YVelocity", GetComponent<Rigidbody>().velocity.y);
		}
	}

    void OnDisable()
    {
        moveDirection = Vector3.zero;
        characterMotor.MoveTo(moveDirection, curAccel, 0.7f, true);
        characterMotor.ManageSpeed(100f, 0f, true);
    }
	
	//prevents rigidbody from sliding down slight slopes (read notes in characterMotor class for more info on friction)
	void OnCollisionStay(Collision other)
	{
		//only stop movement on slight slopes if we aren't being touched by anything else
		if (other.collider.tag != "Untagged" || grounded == false)
			return;
		//if no movement should be happening, stop player moving in Z/X axis
		if(direction.magnitude == 0 && slope < slopeLimit && rigid.velocity.magnitude < 2)
		{
			//it's usually not a good idea to alter a rigidbodies velocity every frame
			//but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it should be ok
			rigid.velocity = Vector3.zero;
		}
	}
    #endregion

    #region PreexistingFunctions
    //returns whether we are on the ground or not
    //also: bouncing on enemies, keeping player on moving platforms and slope checking
    private bool IsGrounded()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y;
        //float dist = GetComponent<Collider>().bounds.extents.y;
        //check whats at players feet, at each floorcheckers position
        int connectingRays = 0;
        Vector3 totalNormal = Vector3.zero;
        int loopCount = 0;
        foreach (Transform check in floorCheckers)
        {
            RaycastHit hit;
            if (Physics.Raycast(check.position, Vector3.down, out hit, dist + 0.05f, mask))
            {
                //Debug.DrawRay(check.position, Vector3.down, Color.green);
                if (!hit.transform.GetComponent<Collider>().isTrigger)
                {
                    //slope control
                    //slopeNormal = hit.normal.normalized;

                    totalNormal += hit.normal.normalized; ;
                    slope = Vector3.Angle(hit.normal, Vector3.up);
                    //slide down slopes
                    if (slope > slopeLimit && hit.transform.tag != "Pushable")
                    {
                        Vector3 slide = new Vector3(0f, -slideAmount, 0f);
                        rigid.AddForce(slide, ForceMode.Force);
                    }
                    //enemy bouncing
                    if (hit.transform.tag == "Enemy" && rigid.velocity.y < 0)
                    {
                        enemyAI = hit.transform.GetComponent<EnemyAI>();
                        enemyAI.BouncedOn();
                        onEnemyBounce++;
                        dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
                    }
                    else
                        onEnemyBounce = 0;
                    //moving platforms
                    if (hit.transform.tag == "MovingPlatform" || hit.transform.tag == "Pushable")
                    {
                        movingObjSpeed = hit.transform.GetComponent<Rigidbody>().velocity;
                        movingObjSpeed.y = 0f;
                        //9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
                        rigid.AddForce(movingObjSpeed * movingPlatformFriction * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {
                        movingObjSpeed = Vector3.zero;
                    }
                    //yes our feet are on something
                    //return true;
                    GroundTag = hit.transform.tag;
                    
                    GroundHitPosition += hit.point;
                    connectingRays++;

                }
            }
            else
            {
                Debug.DrawRay(check.position, Vector3.down, Color.red);
            }
            loopCount++;
        }

        float gX = Mathf.Abs(GroundHitPosition.x) > 0 ? GroundHitPosition.x / connectingRays : 0;
        float gY = Mathf.Abs(GroundHitPosition.y) > 0 ? GroundHitPosition.y / connectingRays : 0;
        float gZ = Mathf.Abs(GroundHitPosition.z) > 0 ? GroundHitPosition.z / connectingRays : 0;
        //print("sum position is " + GroundHitPosition);
        GroundHitPosition = new Vector3(gX, gY, gZ);


       
        //print("divided position is " + GroundHitPosition);

        float nX = Mathf.Abs(totalNormal.x) > 0 ? totalNormal.x / connectingRays : 0;
        float nY = Mathf.Abs(totalNormal.y) > 0 ? totalNormal.y / connectingRays : 0;
        float nZ = Mathf.Abs(totalNormal.z) > 0 ? totalNormal.z / connectingRays : 0;

        slopeNormal = new Vector3(nX, nY, nZ);



        movingObjSpeed = Vector3.zero;
        //no none of the floorchecks hit anything, we must be in the air (or water)
        if (connectingRays == 0)
            return false;
        else
            return true;
    }

    //jumping
    private void JumpCalculations()
	{
		//keep how long we have been on the ground
		groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;
		
		//play landing sound
		if(groundedCount < 0.25 && groundedCount != 0 && !GetComponent<AudioSource>().isPlaying && landSound && GetComponent<Rigidbody>().velocity.y < 1)
		{
			aSource.volume = Mathf.Abs(GetComponent<Rigidbody>().velocity.y)/40;
			aSource.clip = landSound;
			aSource.Play ();
		}
		//if we press jump in the air, save the time
		//if (Input.GetButtonDown ("Jump") && !grounded)
        if (playerInput.JumpPressed && !grounded)
			airPressTime = Time.time;
		
		//if were on ground within slope limit
		if ((grounded || InWater) && slope < slopeLimit)
		{
			//and we press jump, or we pressed jump justt before hitting the ground
			if (playerInput.JumpPressed || airPressTime + jumpLeniancy > Time.time)
			{	
				//increment our jump type if we haven't been on the ground for long
				onJump = (groundedCount < jumpDelay) ? Mathf.Min(2, onJump + 1) : 0;
				//execute the correct jump (like in mario64, jumping 3 times quickly will do higher jumps)
				if (onJump == 0)
						Jump (jumpForce);
				else if (onJump == 1)
						Jump (secondJumpForce);
				else if (onJump == 2){
						Jump (thirdJumpForce);
						onJump --;
				}
			}
		}
	}
	
	//push player at jump force
	public void Jump(Vector3 jumpVelocity)
	{
		if(jumpSound)
		{
			aSource.volume = 1;
			aSource.clip = jumpSound;
			aSource.Play ();
		}
		rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
		rigid.AddRelativeForce (jumpVelocity, ForceMode.Impulse);
		airPressTime = 0f;
	}

    #endregion

    #region CustomFunctions
    void CorrectForSlope(Vector3 slopeNormal)
    {
        Vector3 slopeTangent = SlopeTangentAnalysis(slopeNormal);
        Vector3 slopeCorrection = Vector3.Cross(slopeNormal, slopeTangent);
        rigid.AddForce(slopeCorrection * slopeCorrectionAmt);
        Vector3 test = Quaternion.AngleAxis(90, slopeNormal).eulerAngles;
    }
    Vector3 SlopeTangentAnalysis(Vector3 incomingNormal)
    {
        Vector3 tangentResult = Vector3.zero;
        tangentResult = new Vector3(-incomingNormal.z, 0, incomingNormal.x);
        return tangentResult;
    }
    #endregion
}