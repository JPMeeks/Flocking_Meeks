using UnityEngine;
using System.Collections;

// James P Meeks
// jpm4447@g.rit.edu
// this script is used to control and switch between the various cameras in the scene

public class CameraControl : MonoBehaviour 
{
	// 


	// Camera array that holds a reference to every camera in the scene
	public Camera[] cameras;

	// current camera
	public int currentCameraIndex;

	// Use this for initialization
	void Start () 
	{
		currentCameraIndex = 0;

		// turn all cameras off, except for the first one
		for (int x =1; x < cameras.Length; x++) 
		{
			cameras[x].gameObject.SetActive(false);
		}

		// if any cameras were added to the controller, enable the first one
		if (cameras.Length > 0)
		{
			cameras[0].gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Press the 'C' key to cycle through cameras in the array
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			// cycle to the next camera
			currentCameraIndex ++;

			// if currentCameraIndex is in bounds, set this camera active and the last one inactive
			if(currentCameraIndex < cameras.Length)
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
			else
			{
				cameras[currentCameraIndex-1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
		}
	}

	// creates GUI box in top left corner of screen to be filled with camera info
	void OnGUI()
	{
		GUI.Box (new Rect (10,10,200,50), (string)("Press 'c' to change camera views\n" + "Camera " + currentCameraIndex + "\n" + GetCameraText(currentCameraIndex)));
	}

	// method that spits out what the camera is looking at
	public string GetCameraText(int cCI)
	{
		if (cCI == 0) 
		{
			return "Overview of Terrain";
		} 
		else if (cCI == 1) 
		{
			return "Sideview of Terrain";
		} 
		else if (cCI == 2) 
		{
			return "Gaussian Heights CLose-up";
		} 
		else if (cCI == 3) 
		{
			return "Horde Close-up";
		} 
		else if (cCI == 4) 
		{
			return "Horde Overhead";
		} 
		else if (cCI == 5) 
		{
			return "FPS COntroller";
		} 
		else
		{
			return "";
		}
	}
}
