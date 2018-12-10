using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(AudioSource))]
public class Sweep : MonoBehaviour {

    public float minimumSpeed = 2f; // Minimum speed needed to spawn a beam
    public float minimumDistanceInPixels = 30f; // Minimum distance needed to spawn a beam
    public float delay = 1.0f; // Delay for destroying a beam
    public float stabilizeVelocity = 0.1f; // Stabilizing value for spawned beam for it not to launch too fast

    public Rigidbody beam; // Game object that will spawn under some conditions

    private float angle; // Angle between mouse first pressed position and current position
    private float velocity; // Speed of the mouse after being pressed

	private Vector3 startPosition; // Starting position of the mouse when pressed (LMB)
    private Vector3 distance; // Distance travelled after mouse left click pressed
    private Vector3 mouseDelta; // Current mouse position

    private Quaternion direction; // Rotation of the spawned beam
    
    private bool isPressed = false; // Boolean checking whether LMB is pressed  

    /* Variables for audio */
    public AudioClip[] slash;
    private AudioSource audioSource;
    private AudioClip slashClip;


    /* Variables for VR */
    public SteamVR_Input_Sources thisHand;
    public GameObject trackedObject;
    public float distanceMultiplier = 50.0f;

    private Vector3 startTransform;
    private Vector3 currentTransform;
    private Vector3 VRdistance;

    private float VRangle;

    private Quaternion VRdirection;
    private Rigidbody rb;

    SteamVR_Behaviour_Pose trackedObj;

    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
		trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
		rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		/* Vive Controller Input */
		if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(thisHand))
		{
			startTransform = trackedObject.transform.position;

			isPressed = true;
		}

		if (SteamVR_Input._default.inActions.GrabPinch.GetState(thisHand))
		{
			currentTransform = trackedObject.transform.position - startTransform;
			CalculateDirection();
			CalculateVelocity();
			DistanceTraveled();

            if(isPressed)
            {
                if(rb.maxAngularVelocity >= minimumSpeed && (VRdistance.magnitude * distanceMultiplier) >= minimumDistanceInPixels)
                {
                    UseSound();
                    SpawnBeam();
                    isPressed = false;
                }
            }
		}

		//Debug.Log(startTransform + " and " + currentTransform);

		/* Mouse Input */
		if (Input.GetMouseButtonDown(0))
		{
			startPosition = Input.mousePosition; // Sets the starting mouse position after left mouse button has been pressed

			isPressed = true; // Set to true after LMB has been pressed
		}

		if (Input.GetMouseButton(0))
		{
			mouseDelta = Input.mousePosition - startPosition; // Calculates current mouse position each frame
			CalculateDirection();
			CalculateVelocity();
			DistanceTraveled();

			if (isPressed)
			{
				if (velocity >= minimumSpeed && distance.magnitude >= minimumDistanceInPixels)
				{
					UseSound(); // Use a slash sound
					SpawnBeam(); // Spawn a beam
					isPressed = false; // Set it false so that beam is spawned only once after click
				}
			}
		}
	}

    void CalculateVelocity() // works
    {
        /* Calculates the velocity of the mouse cursor in any direction */

        float velocityX = Mathf.Abs(Input.GetAxis("Mouse X") / Time.deltaTime);
        float velocityY = Mathf.Abs(Input.GetAxis("Mouse Y") / Time.deltaTime);
		
		velocity = Mathf.Sqrt(Mathf.Pow(velocityX, 2) + Mathf.Pow(velocityY, 2));


		// VR part
		var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
		if (origin != null)
		{
			rb.velocity = origin.TransformVector(trackedObj.GetVelocity());
			rb.angularVelocity = origin.TransformVector(trackedObj.GetAngularVelocity());
		}
		else
		{
			rb.velocity = trackedObj.GetVelocity();
			rb.angularVelocity = trackedObj.GetAngularVelocity();
		}

		rb.maxAngularVelocity = rb.angularVelocity.magnitude;
		
		//Debug.Log("velocity = " + rb.maxAngularVelocity);
	}

	void CalculateDirection()
    {
        /* Calculates the angle of the mouse cursor after being pressed and dragged */

        if(mouseDelta.sqrMagnitude < 0.1f)
        {
            return;
        }

        angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;
        if(angle < 0)
        {
            angle += 360; 
        }

        direction = Quaternion.Euler(0, 0, angle - 90);


        // VR part
        VRangle = Quaternion.FromToRotation(Vector3.up, currentTransform - startTransform).eulerAngles.z;

        VRdirection = Quaternion.Euler(0, 0, VRangle);

		//Debug.Log("angle = " + VRangle);
	}

    void DistanceTraveled() // works
    {
        /* Calculates the distance from mouse first left press to current position */

        distance = Input.mousePosition - startPosition;


        // VR part
        VRdistance = (trackedObject.transform.position - startTransform); 

        //Debug.Log("The mouse traveled " + VRdistance.magnitude * distanceMultiplier + " pixels");
    }

    void SpawnBeam()
    {
        Rigidbody clone;

		/* Spawn using mouse */
        // clone = Instantiate(beam, transform.position + transform.forward * 2, direction); // Instantiates (spawns) a beam
        // clone.GetComponent<Rigidbody>().velocity = rb.maxAngularVelocity * transform.forward * stabilizeVelocity;


        /* VR part */
        clone = Instantiate(beam, transform.position + transform.forward * (VRdistance.magnitude / 2), VRdirection); // Instantiate beam
        clone.GetComponent<Rigidbody>().velocity = rb.maxAngularVelocity * transform.forward; // Apply force to Z axis (launches forwards)

        if (clone)
        {
            Destroy(clone.gameObject, delay); // Destroys the spawned beam
        }
    }

    void UseSound()
    {
        // Play a random slashing sound
        int index = Random.Range(0, slash.Length);
        slashClip = slash[index];
        audioSource.clip = slashClip;
        audioSource.Play();
    }
}
