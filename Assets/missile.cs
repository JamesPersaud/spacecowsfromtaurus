using UnityEngine;
using System.Collections;

public class missile : MonoBehaviour {

    public Vector3 Highpoint;
    public Vector3 Groundpoint;
    public float speed = 12;

    private bool reachedHighpoint = false;
    public bool boomtime = false;

	// Use this for initialization
	void Start () {
	
	}	    

	// Update is called once per frame
	void Update () {
        	        
        if(!reachedHighpoint)
        {
            if(Vector3.Distance(Highpoint,transform.position) <= speed * Time.deltaTime)
            {
                reachedHighpoint = true;
            }
            transform.Translate((Highpoint - transform.position).normalized * speed * Time.deltaTime, Space.World);
        }
        else if(!boomtime)
        {
            if (Vector3.Distance(Groundpoint, transform.position) <= speed * Time.deltaTime)
            {
                boomtime = true;
            }
            transform.Translate((Groundpoint - transform.position).normalized * speed * Time.deltaTime, Space.World);
        }

        if(Vector3.Distance(GameController.Instance.theship.transform.position,this.transform.position) <6)
        {
            GameController.Instance.HurtThePlayer(10);
        }
	}
}
