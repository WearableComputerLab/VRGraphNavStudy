using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper_Plane : MonoBehaviour {

	public void stopAnimation() {
        GetComponent<Animator>().SetBool("fly",false);
        transform.parent.GetChild(4).gameObject.SetActive(false);
    }
}
