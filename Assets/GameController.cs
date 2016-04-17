using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public Habitat thehab;

    public float hurtCooldown = 2;
    public float maxhurtcooldown = 2;

    public int life =100;
    public int score =0;
    public bool carrying = false;    

    private int levelOneCowAmount = 32;
    private int levelOneTripodAmount = 3;

    public List<cowscript> CowsInPlay = new List<cowscript>();
    public List<Tripod> TripodsInPlay = new List<Tripod>();

    public GUIStyle MainStyle;
    public GUIStyle BigStyle;
    public GUIStyle LittleStyle;
    public GUISkin ButtonSkin;

    public Font spacefont;

    public GameObject maincow;
    public GameObject maintripod;

    public GameObject PrefabTripod;
    public GameObject PrafabCow;

    public ship theship;

    private int seed;

    public int GetSeed()
    {
        return seed;
    }

    public enum MainGameModes
    {
        TEST,TITLE,GAME,LOADING,INITGAME,GAMEOVER,YOUWIN
    }

    public MainGameModes MainGameMode = MainGameModes.LOADING;

    public static GameController Instance;

    public float HeightMultiplier = 4f;

    void Awake()
    {
        Instance = this;        
        seed = Random.Range(0, 9000000);
    }

	// Use this for initialization
	void Start () {
	
	}
	
    public void HurtThePlayer(int lifelost)
    {
        if (hurtCooldown < 0)
        {
            life -= lifelost;
            if (life <= 0)
            {
                MainGameMode = MainGameModes.GAMEOVER;
            }

            hurtCooldown = maxhurtcooldown;
            theship.GetComponent<ParticleSystem>().Play();
        }
    }

	// Update is called once per frame
	void Update () {

        hurtCooldown -= Time.deltaTime;

        if(MainGameMode == MainGameModes.TITLE)
        {
            if(maincow != null)
            {
                GameObject.DestroyImmediate(maincow);  //debugcow

                //move camera to nice position
                //11.84 	10.31	3.4
                //15.5	57.5	0
                Camera.main.transform.position = new Vector3(11.84f,10.31f,3.4f);
                Camera.main.transform.eulerAngles = new Vector3(15.5f, 57.5f, 0);
            }
        }
        else if (MainGameMode == MainGameModes.INITGAME)
        {
            GameObject.DestroyImmediate(maintripod);  //debugpod

            // make some cows
            for (int i =0; i < levelOneCowAmount; i++)
            {
                cowscript cow = Instantiate(PrafabCow).GetComponent<cowscript>();
                cow.gameObject.name = "cow " + i.ToString();                 
                CowsInPlay.Add(cow);
            }

            //make some sweet tripods
            for (int i = 0; i < levelOneTripodAmount; i++)
            {
                SpawnTripod();
            }

            MainGameMode = MainGameModes.GAME;
            score = 0;
            carrying = false;
        }
        if(MainGameMode == MainGameModes.GAME)
        {
            //yummy cows
            if (Input.GetKey(KeyCode.R) && carrying)
            {
                carrying = false;
                life += 10;
                SpawnTripod();
                SpawnTripod();
            }

            if (Input.GetKey(KeyCode.I))
                hidegui = true;
            //collection of cows
            if(!carrying)
            {
                cowscript carried = null;
                foreach(cowscript c in CowsInPlay)
                {
                    if(Vector3.Distance(c.transform.position,theship.transform.position) <3)
                    {
                        carried = c;
                        break;
                    }
                }

                if(carried != null)
                {
                    CowsInPlay.Remove(carried);
                    carried.gameObject.SetActive(false);
                    GameObject.Destroy(carried.gameObject);
                    carrying = true;
                    score += 10 * levelOneCowAmount - CowsInPlay.Count;
                }
            }

            //dropping off of cows
            if(carrying)
            {
                if(Vector3.Distance(thehab.transform.position,theship.transform.position) < 5)
                {
                    carrying = false;
                    score += 20 * levelOneCowAmount - CowsInPlay.Count;
                    SpawnTripod();
                }
            }

            if(CowsInPlay.Count <1)
            {
                MainGameMode = MainGameModes.YOUWIN;
            }
        }
	}

    public void SpawnTripod()
    {
        int counter = 0;
        bool redo = false;
        do
        {
            counter++;
            redo = false;

            float x = Random.Range(-64, 128);
            float z = Random.Range(-64, 128);
            float y = TerrainHost.Instance.GetHeightForGameObject(x, z);

            Vector3 suggestion = new Vector3(x, y, z);
            if (counter < 10)
            {
                if (Vector3.Distance(thehab.transform.position, suggestion) <= 10)
                {
                    redo = true;
                }
                else
                {
                    foreach (Tripod t in TripodsInPlay)
                    {
                        if (Vector3.Distance(t.transform.position, suggestion) <= 5)
                        {
                            redo = true;
                        }
                    }
                }
            }
            else
            {
                redo = false;
            }

            if (!redo)
            {
                Tripod trippy = Instantiate(PrefabTripod).GetComponent<Tripod>();
                trippy.transform.position = suggestion;
                TripodsInPlay.Add(trippy);
                trippy.gameObject.name = "Tripod " + TripodsInPlay.Count.ToString();
            }

        } while (redo);
    }

    private bool hidegui = false;

    public void OnGUI()
    {
        if(MainGameMode == MainGameModes.TITLE)
        {
            //draw game title
            GUI.Label(new Rect(10, 10, Screen.width/3, 100), "Space cows from Taurus", BigStyle);

            GUI.skin = ButtonSkin;
            if(GUI.Button(new Rect(10, Screen.height - 110, Screen.width / 3, 100),"Play Now"))
            {
                MainGameMode = MainGameModes.INITGAME;
            }
        }
        if(MainGameMode == MainGameModes.GAME)
        {
            if (!hidegui)
            {
                GUI.Label(new Rect(5, 5, 100, 20), "W,A,S,D to fly & steer", LittleStyle);
                GUI.Label(new Rect(5, 25, 100, 20), "Q, E to ascend/descend", LittleStyle);
                GUI.Label(new Rect(5, 45, 100, 20), "R to eat a cow!", LittleStyle);
                GUI.Label(new Rect(5, 65, 100, 20), "I to hide this message", LittleStyle);

                GUI.Label(new Rect(5, 105, 100, 20), "Get all cows to the habitat to win", LittleStyle);
            }

            GUI.Label(new Rect(Screen.width - ((Screen.width / 3) *2), 10, Screen.width / 3, 50), "Life: " + life.ToString(), MainStyle);

            GUI.Label(new Rect(Screen.width - (Screen.width/3), 10, Screen.width / 3, 50), "Score: " + score.ToString(), MainStyle);
            if(carrying)
            {
                GUI.Label(new Rect(Screen.width - (Screen.width / 3), 60, Screen.width / 3, 100), "Cargo: 1 Silicide spcaecow", MainStyle);
            }
            else
            {
                GUI.Label(new Rect(Screen.width - (Screen.width / 3), 60, Screen.width / 3, 100), "Cargo: Nothing! Grab those cows", MainStyle);
            }
        }

        if(MainGameMode == MainGameModes.GAMEOVER)
        {
            GUI.Label(new Rect(Screen.width - ((Screen.width / 3) * 2), 100, Screen.width / 3, 50), "Game Over :)", MainStyle);
            GUI.Label(new Rect(Screen.width - ((Screen.width / 3) * 2), 150, Screen.width / 3, 50), "You Scored " + score.ToString(), MainStyle);
        }

        if (MainGameMode == MainGameModes.YOUWIN)
        {
            GUI.Label(new Rect(Screen.width - ((Screen.width / 3) * 2), 100, Screen.width / 3, 50), "You Win :(", MainStyle);
            GUI.Label(new Rect(Screen.width - ((Screen.width / 3) * 2), 150, Screen.width / 3, 50), "You Scored " + score.ToString(), MainStyle);
        }
    }
}
