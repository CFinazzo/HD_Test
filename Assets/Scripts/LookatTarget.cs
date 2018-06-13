using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatTarget : MonoBehaviour {
    public GameObject goTarget;

    private float desiredDistance;
    public float minDistance = 4;
    public float maxDistance = 8;
    private float currentDistance;
    public float zoomDampening = 0.5f;
    public Vector3 targetOffset = Vector3.zero;
    public Vector3 position;

    public float xSpeed = 100f;
    public float ySpeed = 100f;
    public float yMinLimit = 3f;
    public float yMaxLimit = 70f;
    private float xDeg, yDeg;

    public bool isTransitioning = false;
    
    private Quaternion desiredRotation, currentRotation, rotation;
    private float currentX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (!isTransitioning)
        {
            // Zooms in and out with scroll wheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * zoomDampening * Mathf.Abs(desiredDistance);

            // Checks if left click is down
            if (Input.GetMouseButton(0))
            {
                //Gets mouse axis to set desired rotation, and clamps within limits set
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            }
            

            // Converts rotation to Euler and slowly transitions to view.
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening);
            transform.rotation = rotation;

            // Keeps camera at expected distance
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * zoomDampening);

            //Updates position based on expected distance
            position = goTarget.transform.position - (gameObject.transform.rotation * Vector3.forward * currentDistance) + targetOffset;
            transform.position = position;

        } else
        {
            // Runs while transitioning to keep values updated and prevent snapping backwards

            // Keeps the camera looking at the target
            gameObject.transform.LookAt(goTarget.transform);
        }
    }

    void Update()
    {
        /*currentRotation = transform.localEulerAngles;
        currentRotation.x = ClampAngle(currentRotation.x, yMin, yMax);
        //currentRotation.x = Quaternion.ToEulerAngles
        //transform.localRotation = Quaternion.Euler(currentRotation);
        Debug.Log(Quaternion.Euler(currentRotation) + " - " + transform.rotation);
        gameObject.transform.rotation = Quaternion.Euler(currentRotation);
        Debug.Log("After: " + transform.rotation);*/
    }


    private void tweenFinished()
    {

        // Sets the desired distance and current distance to whatever distance out marker is at, to prevent snapping back
        desiredDistance = Vector3.Distance(gameObject.transform.position, goTarget.transform.position);
        currentDistance = desiredDistance;

        // Sets the X and Y degrees to whatever the camera is at when its done transitioning.
        xDeg = transform.localEulerAngles.y;
        yDeg = transform.localEulerAngles.x;
        isTransitioning = false;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, yMinLimit, yMaxLimit);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
