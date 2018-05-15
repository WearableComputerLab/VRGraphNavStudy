using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
public class Camera_System : MonoBehaviour {

    ScreenOverlay overlay;
    private bool fade = false;
    private int fadeState = 0;
    public Vector3 fadePosition;
    public float dist;
    public int getVector = 0;
    private Vector3 pos;
	private LaserPointer laser;

	// Use this for initialization
	void Awake () {
        overlay = GetComponent<ScreenOverlay>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (fade) {
            dist = Vector3.Distance(transform.position, fadePosition);
            if (fadeState == 0) {
                overlay.intensity += 40 * Time.deltaTime;
                if(overlay.intensity > 20) {
                    if(getVector == 0) {
                         pos = (fadePosition - transform.position).normalized;
                         getVector = 1;
                    }
                    transform.parent.position += pos * (10 * Time.deltaTime);
                    if (dist <= 0.7f) {
                        fadeState = 1;
                    } 

//print(dist);
                }
            }
            if(fadeState == 1) {
                if (overlay.intensity > 0) {
                    overlay.intensity -= 40 * Time.deltaTime;
                } else {
                    fade = false;
                    fadeState = 0;
                    getVector = 0;
					if (laser != null) {
						laser.resetLaserPointer ();
					}
                }
            }
        }

	}

    public void FadeOutIn(Vector3 fadePosition) {
        fade = true;
        this.fadePosition = fadePosition;
        fadeState = 0;
    }

	public void setLaserPointer(LaserPointer laser){
		this.laser = laser;
	}


}
