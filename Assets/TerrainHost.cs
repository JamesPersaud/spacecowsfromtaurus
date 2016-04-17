using UnityEngine;
using System.Collections.Generic;

public class TerrainHost : MonoBehaviour {

    //work out which chunk the camera's position should be in, render that chunk and the surrounding chunks
    //remove chunks that have become too far away    

    public GUIStyle loaderStyle;

    public static bool dynamicsize = true;
    public static bool dontresize = true;

    public const int DeleteDistance = 3;
    public const int CreateDistance = 2;
    public const float ChunkSize = 64;

    public Texture progressTex;

    public Material TerrainMaterial;    

    public bool CreatedTestTerrain = false;
    public bool InitTerrain = false;

    public TerrainChunk testChunk00;
    public TerrainChunk testChunk10;

    public List<TerrainChunk> ChunksInScene = new List<TerrainChunk>();    

    public int lastCameraX = -1000000;
    public int lastCameraZ = -1000000;

    private float chunktimer = 0.5f;
    private float chunkmax = 0.5f;

    private List<int> xtodo = new List<int>();
    private List<int> ytodo = new List<int>();

    private int totalToDo = 0;

    private float lastpercent = 0;

    private List<string> loadingtexts = new List<string>();
    private int loadingtextIndex = 0;

    public static TerrainHost Instance;

    private void cycleLoadingTexts()
    {
        loadingtextIndex = (loadingtextIndex+1) % loadingtexts.Count;
    }

    private string LoadingText
    {
        get
        {
            return loadingtexts[loadingtextIndex];
        }
    }
	
    private void RemoveChunks(int minx, int maxx, int minz, int maxz)
    {
        for (int i = ChunksInScene.Count -1; i >=0; i--)
        {
            TerrainChunk c = ChunksInScene[i];
            if (c.offsetX < minx || c.offsetX > maxx || c.offsetY < minz || c.offsetY > maxz)
            {
                Debug.LogWarning("Killing Chunk "+ c.offsetX.ToString() + "," + c.offsetY.ToString());
                ChunksInScene.Remove(c);
                GameObject.DestroyImmediate(c);
            }
        }
    }

    public float GetHeightForGameObject(float x, float z)
    {
        //what chunk is it in
        int xchunk = Mathf.FloorToInt(x / ChunkSize);
        int zchunk = Mathf.FloorToInt(z / ChunkSize);        

        foreach (TerrainChunk c in ChunksInScene)
        {
            if (Mathf.FloorToInt(c.offsetX) == xchunk && Mathf.FloorToInt(c.offsetY) == zchunk)
            {

                //in this chunk
                float apparentX = x - ChunkSize * xchunk;
                float apparentZ = z - ChunkSize * zchunk;                
                float baseheight = c.Map.GetSceneHeight(apparentX, apparentZ, GameController.Instance.HeightMultiplier);
                return Mathf.Max(baseheight, 2);
            }
        }

        return 0;
    }

    public float GetHeightForGameObject(Vector3 pos, ref int x, ref int z, ref float innerx, ref float innerz, ref float baseheight)
    {        
        //what chunk is it in
        int xchunk = Mathf.FloorToInt(pos.x / ChunkSize);
        int zchunk = Mathf.FloorToInt(pos.z / ChunkSize);

        x = xchunk;
        z = zchunk;        

        foreach(TerrainChunk c in ChunksInScene)
        {
            if(Mathf.FloorToInt(c.offsetX) == xchunk && Mathf.FloorToInt(c.offsetY) == zchunk)
            {
                //why is it getting the wrong chunk?

                //in this chunk
                float apparentX = pos.x - ChunkSize*xchunk;
                float apparentZ = pos.z - ChunkSize*zchunk;
                innerx = apparentX;
                innerz = apparentZ;
                baseheight = c.Map.GetSceneHeight(apparentX, apparentZ, GameController.Instance.HeightMultiplier);
                return Mathf.Max(baseheight, 2);
            }
        }

        return 0;
    }

    private bool chunkExists(int x, int z)
    {
        if (x == 0 && z == 0) return false;

        foreach(TerrainChunk c in ChunksInScene)
        {
            if (Mathf.FloorToInt(c.offsetX) == x && Mathf.FloorToInt(c.offsetY) == z)
                return true;
        }

        return false;
    }

    private void AddChunk(float x, float z)
    {
        Debug.LogWarning("Adding Chunk " + x.ToString() + "," + z.ToString());

        TerrainChunk c = new GameObject().AddComponent<TerrainChunk>();
        MeshRenderer r = c.gameObject.AddComponent<MeshRenderer>();
        c.gameObject.AddComponent<MeshFilter>();

        r.material = TerrainMaterial;

        HeightmapSettings hms = new HeightmapSettings();
        hms.Seed = GameController.Instance.GetSeed();
        Heightmap hm = Heightmap.CreateProceduralHeightmap(hms, x, z);
        c.Map = hm;
        c.Refresh();

        c.name = "Chunk " + x.ToString() + "," + z.ToString();
        c.offsetX = x;
        c.offsetY = z;
        ChunksInScene.Add(c);

        c.transform.position = new Vector3(x*ChunkSize,0,z*ChunkSize);

        if(debugHeightGetting)
        {
            Debug.LogWarning("10,10 vheight by terrainhost is " + c.Map.GetSceneHeight(10,10,GameController.Instance.HeightMultiplier));
        }
    }

    private static bool debugHeightGetting = true;

    void UpdateTerrain()
    {
        if(xtodo.Count>0 && ytodo.Count>0)
        {
            int x = xtodo[0];
            int y = ytodo[0];

            AddChunk(x, y);

            xtodo.RemoveAt(0);
            ytodo.RemoveAt(0);

            return;
        }

        int cameraX = Mathf.FloorToInt(Camera.main.transform.position.x / ChunkSize);
        int cameraZ = Mathf.FloorToInt(Camera.main.transform.position.x / ChunkSize);

        if (dynamicsize)
        {
            if (lastCameraX != cameraX || lastCameraZ != cameraZ)
            {
                int minx = cameraX - DeleteDistance;
                int maxx = cameraX + DeleteDistance;
                int minz = cameraZ - DeleteDistance;
                int maxz = cameraZ + DeleteDistance;

                RemoveChunks(minx, maxx, minz, maxz);

                minx = cameraX - CreateDistance;
                maxx = cameraX + CreateDistance;
                minz = cameraZ - CreateDistance;
                maxz = cameraZ + CreateDistance;

                //add chunks
                for (int x = minx; x <= maxx; x++)
                {
                    for (int z = minz; z <= maxz; z++)
                    {
                        if (!chunkExists(x, z))
                        {
                            //AddChunk(x, z);
                            xtodo.Add(x);
                            ytodo.Add(z);
                            totalToDo++;
                        }
                    }
                }                

                if (dontresize)
                    dynamicsize = false;
            }            
        }        

        lastCameraX = cameraX;
        lastCameraZ = cameraZ;
    }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        loadingtexts = new List<string>();
        loadingtexts.Add("Calibrating terrain");
        loadingtexts.Add("Sharpening pointy bits");
        loadingtexts.Add("Doing inefficient maths");
        loadingtexts.Add("Doing inefficient maths");
        loadingtexts.Add("Doing REALLY inefficient maths");
        loadingtexts.Add("Doing REALLY inefficient maths");
        loadingtexts.Add("Hey this is Ludum Dare");
        loadingtexts.Add("Hey this is Ludum Dare");
        loadingtexts.Add("It's not the time for textbook code");
        loadingtexts.Add("It's not the time for textbook code");
        loadingtexts.Add("Articulating Splines");
        loadingtexts.Add("Articulating Splines");
        loadingtexts.Add("Yeah that always sounds good");
        loadingtexts.Add("Yeah that always sounds good");
        loadingtexts.Add("Mucking out silicon cows");
        loadingtexts.Add("Polishing tripods");
        loadingtexts.Add("Procrastinating");
        loadingtexts.Add("Procrastinating");
        loadingtexts.Add("Procrastinating");
        loadingtexts.Add("Procrastinating");
        loadingtexts.Add("If I told you what it was doing now");
        loadingtexts.Add("If I told you what it was doing now");
        loadingtexts.Add("You wouldn't believe me");
        loadingtexts.Add("You wouldn't believe me");
        loadingtexts.Add("Wheeeeeeeeeeeee!");
    }

    void MakeTestTerrain()
    {
        //0,0
        testChunk00 = new GameObject().AddComponent<TerrainChunk>();
        MeshRenderer r = testChunk00.gameObject.AddComponent<MeshRenderer>();
        testChunk00.gameObject.AddComponent<MeshFilter>();

        r.material = TerrainMaterial;

        HeightmapSettings hms = new HeightmapSettings();        
        Heightmap hm = Heightmap.CreateProceduralHeightmap(hms,0,0);
        testChunk00.Map = hm;
        testChunk00.Refresh();

        testChunk00.name = "Test chunk 0,0";

        //1,0
        testChunk10 = new GameObject().AddComponent<TerrainChunk>();
        r = testChunk10.gameObject.AddComponent<MeshRenderer>();
        testChunk10.gameObject.AddComponent<MeshFilter>();

        r.material = TerrainMaterial;
        
        hm = Heightmap.CreateProceduralHeightmap(hms,1,0);
        hm.offset_X = 1f;
        testChunk10.Map = hm;
        testChunk10.Refresh();

        testChunk10.name = "Test chunk 1,0";

        testChunk10.transform.position = new Vector3(hms.MapWidth/2,0,0);

        CreatedTestTerrain = true;
    }

	// Update is called once per frame
	void Update ()
    {
        chunktimer -= Time.deltaTime;
        if (chunktimer < 0)
        {
            UpdateTerrain();
            chunktimer = chunkmax;
        }

        //if(GameController.Instance.MainGameMode == GameController.MainGameModes.TEST && !CreatedTestTerrain)
        //{
            //MakeTestTerrain();
        //}
//        else
//        {
            
        //}
	}

    public void OnGUI()
    {
        if (GameController.Instance.MainGameMode == GameController.MainGameModes.LOADING)
        {            
            int halfdown = Screen.height / 2;            
            int half_accross = Screen.width /2;

            int boxheight = Screen.height / 5;
            int boxwidth = (Screen.width / 5) *2;

            int barheight = (boxheight / 10) * 8;
            int barwidth = (boxwidth / 10) * 9;

            GUI.Box(new Rect(half_accross - boxwidth/2, halfdown - boxheight/2, boxwidth, boxheight), "");

            float percent_done = (float)(totalToDo - xtodo.Count) / (float)totalToDo;            

            if (totalToDo >0 && percent_done >0 && progressTex != null)
            {
                if (percent_done != lastpercent)
                {
                    cycleLoadingTexts();
                }

                GUI.DrawTexture(new Rect(half_accross - barwidth / 2, halfdown - barheight / 2, barwidth*percent_done, barheight), progressTex);
            }

            if(totalToDo > 0 && xtodo.Count < 1)
            {
                GameController.Instance.MainGameMode = GameController.MainGameModes.TITLE;
            }

            lastpercent = percent_done;

            GUI.Label(new Rect(0, 0, Screen.width, halfdown - boxheight / 2), "Please Wait", loaderStyle);
            GUI.Label(new Rect(0, halfdown - (boxheight / 2) + boxheight, Screen.width, Screen.height - halfdown - (boxheight / 2) + boxheight), LoadingText, loaderStyle);
        }
    }
}
