using UnityEngine;
using System.Collections;

public class ship : MonoBehaviour {

    private float height = 1f;
    private float maxheight = 9;

    private float divespeed = 4;
    private float gospeed = 7;
    private float rotspeed = 12;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (GameController.Instance.MainGameMode == GameController.MainGameModes.GAME)
        {
            //movement
            float translation = Input.GetAxis("Vertical") * gospeed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotspeed + Time.deltaTime;

            float divewanted = 0;
            if(Input.GetKey(KeyCode.Q))
            {
                //dive down
                divewanted = Time.deltaTime * -divespeed;
            }
            else if(Input.GetKey(KeyCode.E))
            {
                divewanted = Time.deltaTime * divespeed;
            }


            transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + rotation) % 360,transform.eulerAngles.z);
            transform.Translate(Vector3.forward * translation, Space.Self);

            if (TerrainHost.Instance != null)
            {
                height += divewanted;
                height = Mathf.Min(height, maxheight);
                float y = TerrainHost.Instance.GetHeightForGameObject(this.transform.position.x, this.transform.position.z) + 1;
                if (y > height)
                    height = y;
                this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
            }
        }       
    }
}
