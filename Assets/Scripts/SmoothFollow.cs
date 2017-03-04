using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    BehaviourManager behaMngr;
    public Transform target;
    public float distance = 3.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping = 2.0f;
    public float rotationDamping = 2.0f;

	// Use this for initialization
	void LateUpdate ()
    {
        if (!target)
            return;

        float wantedheight = target.position.y + height;
        float currentHeight = transform.position.y;

        // damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedheight, heightDamping*Time.deltaTime);

        // set the position of the camera
        Vector3 wantedPosition = target.position - target.forward * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);

        // adjust the hieght of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationDamping);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
