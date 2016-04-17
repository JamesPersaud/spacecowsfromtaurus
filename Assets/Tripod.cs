using UnityEngine;
using System.Collections;

public class Tripod : MonoBehaviour {

    public GameObject missilePrafab;
    public missile currentMissile = null;

    public float cooldown = 15;
    public float maxcooldown = 15;

    private bool yfixed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (GameController.Instance.MainGameMode == GameController.MainGameModes.GAME)
        {
            //fire a missile maybe?
            cooldown -= Time.deltaTime;
            if (cooldown < 0 && (currentMissile == null || !currentMissile.enabled || currentMissile.boomtime))
            {
                float distoplay = Vector3.Distance(GameController.Instance.theship.transform.position, this.transform.position);

                if (distoplay <= 30)
                {
                    currentMissile = Instantiate(missilePrafab).GetComponent<missile>();
                    currentMissile.transform.position = this.transform.position;

                    float playerground = TerrainHost.Instance.GetHeightForGameObject(GameController.Instance.theship.transform.position.x, GameController.Instance.theship.transform.position.z);
                    currentMissile.Groundpoint = new Vector3(GameController.Instance.theship.transform.position.x, playerground, GameController.Instance.theship.transform.position.z);

                    Vector3 direc = (GameController.Instance.theship.transform.position - this.transform.position).normalized;
                    Vector3 halfish = this.transform.position + (direc * distoplay / 2);

                    currentMissile.Highpoint = new Vector3(halfish.x, 15, halfish.z);

                    cooldown = maxcooldown;
                }
            }
        }

        if (TerrainHost.Instance != null)
        {
            float y = TerrainHost.Instance.GetHeightForGameObject(this.transform.position.x, this.transform.position.z) + 4;
            this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
        }
	}
}
