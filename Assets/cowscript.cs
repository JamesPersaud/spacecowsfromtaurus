using UnityEngine;
using System.Collections;

public class cowscript : MonoBehaviour {

    public float floatBy = 0.1f;
    public int chunkX =0;
    public int chunkZ =0;
    public float innerX =0;
    public float innerZ =0;
    public float baseheight =0;

    public float cowmovespeed = 1f;
    public Vector3 nextpoint = Vector3.zero;

    public bool alive = true;

    public bool resting = false;

    public float timeuntilrestend = 0;
    public float timeuntilrestbegin = 0;

    public float minrest = 2f;
    public float maxrest = 12f;

    public float maxactive = 20f;
    public float minactive = 5f;

    // Use this for initialization
    void Start () {

        //set initial pos
        float x = Random.Range(-64, 128);
        float z = Random.Range(-64, 128);
        float y = TerrainHost.Instance.GetHeightForGameObject(x, z);
        transform.position = new Vector3(x, y, z);

        refreshTarget();
	}

    private void refreshTarget()
    {
        float x = Random.Range(-64, 128);
        float z = Random.Range(-64, 128);
        float y = TerrainHost.Instance.GetHeightForGameObject(x, z);
        nextpoint = new Vector3(x,y,z);
    }
	
	// Update is called once per frame
	void Update () {
        if (alive)
        {
            if (resting)
            {
                timeuntilrestend -= Time.deltaTime;
                if(timeuntilrestend <0)
                {
                    resting = false;
                    timeuntilrestbegin = Random.Range(minactive, maxactive);
                    refreshTarget();//fickle beasts
                }
            }
            else
            {
                timeuntilrestbegin -= Time.deltaTime;

                if(timeuntilrestbegin <0)
                {
                    resting = true;
                    timeuntilrestend = Random.Range(minrest, maxrest);
                }

                if (Vector3.Distance(this.transform.position, nextpoint) < cowmovespeed)
                {
                    refreshTarget();
                }

                //alive and not resting, so move.
                transform.Translate((nextpoint - transform.position).normalized * cowmovespeed * Time.deltaTime, Space.World);
                

                //set height according to terrain
                this.transform.position = new Vector3(this.transform.position.x, TerrainHost.Instance.GetHeightForGameObject(this.gameObject.transform.position, ref chunkX, ref chunkZ, ref innerX, ref innerZ, ref baseheight) + floatBy, this.transform.position.z);
            }
        }
	}
}
