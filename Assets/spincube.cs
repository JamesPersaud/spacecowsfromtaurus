using UnityEngine;
using System.Collections;

public class spincube : MonoBehaviour {

    public float spinspeed = 360;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y + (Time.deltaTime*spinspeed) % 360, this.transform.eulerAngles.z);        
	}
}
