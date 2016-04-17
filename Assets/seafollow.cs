using UnityEngine;
using System.Collections;

public class seafollow : MonoBehaviour {

    private float wibbleAmount = 0.05f;
    private float speed = 0.04f;
    private float midpoint;
    private float minpoint;
    private float maxpoint;
    private bool goingup = false;

	// Use this for initialization
	void Start () {
        midpoint = transform.position.y;
        maxpoint = midpoint + wibbleAmount;
        minpoint = midpoint - wibbleAmount;
	}
	
	// Update is called once per frame
	void Update () {
        float y = transform.position.y;        

        //gently wibble
        if(goingup)
        {
            y += Time.deltaTime * speed;
            if (y > maxpoint)
                goingup = false;
        }
        else
        {
            y -= Time.deltaTime * speed;
            if (y < minpoint)
                goingup = true;
        }

        this.transform.position = new Vector3(Camera.main.transform.position.x, y, Camera.main.transform.position.z);
    }
}
