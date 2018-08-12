using System;
using System.Collections; 
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class Palm : MonoBehaviour {
	SerialPort sp = new SerialPort("\\\\.\\COM17", 9600); // Change to reflect which serial port your Arduino is plugged into

    // All of the hand pieces
	Transform index_Root;
	Transform index_1;
	Transform index_2;
	Transform index_3;
	Transform middle_Root;
	Transform middle_1;
	Transform middle_2;
	Transform middle_3;
	Transform ring_Root;
	Transform ring_1;
	Transform ring_2;
	Transform ring_3;
	Transform pinky_Root;
	Transform pinky_1;
	Transform pinky_2;
	Transform pinky_3;
	Transform thumb_Root;
	Transform thumb_1;
	Transform thumb_2;
	Transform palm;

    // Used to safely quit the serial thread
	bool threadActive = true;

    // Keep track of which parts of the hand are touching a virtual object and what type of object is being touched.
	int indexVib = 0;
	int middleVib = 0;
	int ringVib = 0;
	int pinkVib = 0;
	int palmVib = 0;

    // Keep track of hand position values from Arduino
	float bendindex;
	float bendMiddle;
	float bendRing;
	float bendPinky;
	float bendThumb;
	float PalmX;
	float PalmY;
	float PalmZ;
	float Palm_ROT_X;
	float Palm_ROT_Y;
	float Palm_ROT_Z;
    
    // Only update positions if we got a new reading from Arudino
    bool newReading = false;

    // How far away objects can be from the hand when grabbed
	public float magnetDistance = 2.0f;

    // Is the user pinching / grabbing?
	protected bool pinching_;

    // THe object being grabbed
	protected Collider grabbed_;

	void Awake () {
		palm = GameObject.Find ("Palm").transform;
		index_Root = GameObject.Find ("Palm/Finger/Index").transform;
		index_1 = index_Root.Find("Index_1");
		index_2 = index_1.Find("Index_2");
		index_3 = index_2.Find("Index_3");
		middle_Root = GameObject.Find ("Palm/Finger/Middle").transform;
		middle_1 = middle_Root.Find("Middle_1");
		middle_2 = middle_1.Find("Middle_2");
		middle_3 = middle_2.Find("Middle_3");
		ring_Root = GameObject.Find ("Palm/Finger/Ring").transform;
		ring_1 = ring_Root.Find("Ring_1");
		ring_2 = ring_1.Find("Ring_2");
		ring_3 = ring_2.Find("Ring_3");
		pinky_Root = GameObject.Find ("Palm/Finger/Pinky").transform;
		pinky_1 = pinky_Root.Find("Pinky_1");
		pinky_2 = pinky_1.Find("Pinky_2");
		pinky_3 = pinky_2.Find("Pinky_3");
		thumb_Root = GameObject.Find ("Palm/Finger/Thumb").transform;
		thumb_1 = thumb_Root.Find("Thumb_1");
		thumb_2 = thumb_1.Find("Thumb_2");

		Thread serialReader = new Thread (SerialReadThread);
		serialReader.Start ();

        pinching_ = false;
        grabbed_ = null;
    } 

	void Update() {
		Quaternion indexRotation = Quaternion.Euler (0f, 0f, bendindex);
		index_Root.localRotation = indexRotation;
		index_1.localRotation = indexRotation;
		index_2.localRotation = indexRotation;
		index_3.localRotation = indexRotation;

		Quaternion middleRotation = Quaternion.Euler (0f, 0f, bendMiddle);
		middle_Root.localRotation = middleRotation;
		middle_1.localRotation = middleRotation;
		middle_2.localRotation = middleRotation;
		middle_3.localRotation = middleRotation;

		Quaternion ringRotation = Quaternion.Euler (0f, 0f, bendRing);
		ring_Root.localRotation = ringRotation;
		ring_1.localRotation = ringRotation;
		ring_2.localRotation = ringRotation;
		ring_3.localRotation = ringRotation;

		Quaternion pinkyRotation = Quaternion.Euler (0f, 0f, bendPinky);
		pinky_Root.localRotation = pinkyRotation;
		pinky_1.localRotation = pinkyRotation;
		pinky_2.localRotation = pinkyRotation;
		pinky_3.localRotation = pinkyRotation;

		Quaternion thumbRotation = Quaternion.Euler (0f, 0f, bendThumb);
		thumb_Root.localRotation = thumbRotation;
		thumb_1.localRotation = thumbRotation;
		thumb_2.localRotation = thumbRotation;
        
        if (newReading) {
            palm.eulerAngles = new Vector3(Palm_ROT_X, -Palm_ROT_Z, Palm_ROT_Y);
            palm.Translate(new Vector3(PalmX, PalmY, PalmZ));
            newReading = false;
        }
        
        // Check to see if more than two fingers are more than half bent. If so, we are pinching/grabbing.
        int fingerBentCount = 0;

        if(bendindex > 20) { fingerBentCount++; }
        if (bendMiddle > 20) { fingerBentCount++; }
        if (bendRing > 20) { fingerBentCount++; }
        if (bendPinky > 20) { fingerBentCount++; }
        if (bendThumb > 20) { fingerBentCount++; }

        if (fingerBentCount > 2) {
            OnPinch(middle_3.position);
        } else {
            OnRelease();
        }
	}

    void OnApplicationQuit()
    {
        threadActive = false;
    }

    void SerialReadThread() {
		while (threadActive) {
			if (sp.IsOpen) { // Check to see if the serial port is open 
				try {
                    string[] output_array = sp.ReadLine().Split (','); // Get the string output of the serial port             

                    // Customize the values here to reflect the values you are getting from your flex sensors
                    // ((value - lowest value) / range between lowest and highest value) * how much each joint should bend
                    bendThumb = ((float.Parse(output_array[0]) - 1002f) / 21f) * 40f;
                    bendindex = ((float.Parse (output_array [1]) - 991f) / 32f) * 40f;
                    bendMiddle = ((float.Parse(output_array[2]) - 4f) / 70f) * 40f;
                    bendRing = ((float.Parse(output_array[3]) - 781f) / 93f) * 40f;
                    bendPinky = ((float.Parse(output_array[4]) - 1023f) / -2f) * 40f;

                    // Movement distance
					PalmX = float.Parse (output_array [5]) * -1 + 0.5f;
                    PalmY = float.Parse(output_array[6]) - 0.5f;
					PalmZ = float.Parse (output_array [7]) - 0.5f;

                    // Rotation
                    Palm_ROT_X = float.Parse (output_array [8]);
					Palm_ROT_Y = float.Parse (output_array [9]);
					Palm_ROT_Z = float.Parse (output_array [10]);
                    
                    // Let the Unity thread know that we got a new reading from Arduino
                    newReading = true;

                    // Send the current touch vibration info to Arduino. Make sure to end with "~" as that's the trigger character.
                    sp.Write(indexVib.ToString() + middleVib.ToString() + ringVib.ToString() + pinkVib.ToString() + palmVib.ToString() + "~");
					
                    // Reset vibrations
					indexVib = 0;
					middleVib = 0;
					ringVib = 0;
					pinkVib = 0;
					palmVib = 0;
					Thread.Sleep(300);
				} catch (System.Exception) {
					// Do nothing if there is an exception
				}
			} else {
				sp.Open(); // Open the serial port
				sp.ReadTimeout = 250; // Timeout for reading 
			}
		}
	}

    // Set up any vibrations due to touching a collider
	public void ReceiveTriggers (string colName, string fingerName) {
		if (colName.Equals("waterfall")) {
			switch(fingerName) {
			case "Index":
				indexVib = 2;
				break;
			case "Middle":
				middleVib = 2;
				break;
			case "Ring":
				ringVib = 2;
				break;
			case "Pinky":
				pinkVib = 2;
				break;
			case "Palm":
				palmVib = 2;
				break;
			}
		} else {
            switch (fingerName) {
            case "Index":
                indexVib = 1;
                break;
            case "Middle":
                middleVib = 1;
                break;
            case "Ring":
                ringVib = 1;
                break;
            case "Pinky":
                pinkVib = 1;
                break;
            case "Palm":
                palmVib = 1;
                break;
            }
        }
	}

	void OnPinch(Vector3 pinch_position) {
		pinching_ = true;

		// Check if we pinched a movable object and grab the closest one that's not part of the hand.
		Collider[] close_things = Physics.OverlapSphere(pinch_position, magnetDistance);
		Vector3 distance = new Vector3(magnetDistance, 0.0f, 0.0f);

		for (int j = 0; j < close_things.Length; ++j) {
			Vector3 new_distance = pinch_position - close_things[j].transform.position;
			if (close_things[j].GetComponent<Rigidbody>() != null &&
                new_distance.magnitude < distance.magnitude &&
				!close_things[j].transform.IsChildOf(palm)
            ) {
				grabbed_ = close_things[j];
				distance = new_distance;
			}
		}

		if (grabbed_ != null) {
			grabbed_.transform.root.position = transform.position;
		}
	}

	// Clear the pinch state.
	void OnRelease() {
		grabbed_ = null;
		pinching_ = false;
	}
}