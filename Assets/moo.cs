using UnityEngine;
using System.Collections;

public class moo : MonoBehaviour {

    public bool mooing = false;
    public float timeuntilmoo = 0;
    public float maxtimeuntilmoo = 10;
    public float mintimeuntilmoo = 2;
    public float mooduration = 2;
    public float mooingfor = 0;

    MeshRenderer mooRender;

	// Use this for initialization
	void Start () {
        mooRender = this.GetComponent<MeshRenderer>();
        mooRender.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+180, transform.eulerAngles.z);        

        if(!mooing)
        {
            timeuntilmoo -= Time.deltaTime;
            if (timeuntilmoo <0)
            {
                mooRender.enabled = true;                
                mooingfor = 0;
                mooing = true;
            }
        }
        else
        {
            mooingfor += Time.deltaTime;
            if(mooingfor > mooduration)
            {
                mooingfor = 0;
                mooing = false;
                timeuntilmoo = Random.Range(mintimeuntilmoo, maxtimeuntilmoo);
                mooRender.enabled = false;
            }
        }
    }
}
