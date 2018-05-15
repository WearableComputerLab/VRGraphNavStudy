using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskTracker : MonoBehaviour {

    public SteamVR_TrackedObject tracker;
    private SteamVR_Controller.Device vr_device;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        vr_device = SteamVR_Controller.Input((int)tracker.index);
        if(vr_device.angularVelocity.y > 0.02f) { 
            transform.rotation = Quaternion.Lerp(transform.rotation, tracker.transform.rotation, 20 * Time.deltaTime);
            transform.position = tracker.transform.position;
        }
    }
}
