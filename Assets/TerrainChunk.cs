using System;
using UnityEngine;
using System.Collections.Generic;

public class TerrainChunk : MonoBehaviour
{
    private Heightmap map;

    public float offsetX = 0f;
    public float offsetY = 0f;

    //colours
    private List<KeyValuePair<float, Color>> ColourTemplates;
    private List<float> Badheights = new List<float>();    

    private bool loggedMissingMapError = false;
    private bool renderDataCreated = false;



    public Heightmap Map
    {
        get
        {
            return map;
        }

        set
        {
            map = value;
        }
    }

    /// <summary>
    /// Completely refresh the terrain component
    /// </summary>
    public void Refresh()
    {
        CreateRenderData();
    }

    /// <summary>
    /// Set up the bands of colour used to signify height ranges in this visualisation
    /// </summary>
    private void InitialiseColourTemplates()
    {
        ColourTemplates = new List<KeyValuePair<float, Color>>();

        ColourTemplates.Add(new KeyValuePair<float, Color>(0.128f,
            new Color(0.0f, 0.0f, 0.1f, 1.0f)));
        //Lowest level (underwater)
        ColourTemplates.Add(new KeyValuePair<float, Color>(0.25f,
            new Color(0.0f, 0.0f, 0.2f, 1.0f)));
        //Elevated level (beach)
        ColourTemplates.Add(new KeyValuePair<float, Color>(0.4f,
            new Color(0.0f, 0.0f, 0.3f, 1.0f)));
        //High level (grassland)
        ColourTemplates.Add(new KeyValuePair<float, Color>(0.7f,
            new Color(0.1f, 0.1f, 0.3f, 1.0f)));
        //Highest level (hills)
        ColourTemplates.Add(new KeyValuePair<float, Color>(0.8f,
            new Color(0.1f, 0.1f, 0.4f, 1.0f)));
        //Highest level (peaks)
        ColourTemplates.Add(new KeyValuePair<float, Color>(2f,
            new Color(0.1f, 0.1f, 0.5f, 1.0f)));
    }

    /// <summary>
    /// Return corresponding vertex colour for height
    /// </summary>    
    private Color getColorByHeight(float h)
    {
        if (ColourTemplates != null && ColourTemplates.Count > 0)
        {
            for (int i = 0; i < ColourTemplates.Count; i++)
            {
                if (ColourTemplates[i].Key >= h)
                    return ColourTemplates[i].Value;
            }
        }

        //No colour found, return horrible hot pink
        if (!Badheights.Contains(h))
        {
            Debug.LogWarning("Height value " + h.ToString() + " has no associated colour");
            Badheights.Add(h);
        }
        return new Color(1, 0, 1);
    }

    /// <summary>
    /// (re)create the terrain mesh
    /// </summary>
    private void CreateRenderData()
    {
        if (map != null && map.Settings != null && map.Settings.MapWidth > 0 && map.Settings.MapHeight > 0)
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            MeshRenderer renderer = GetComponent<MeshRenderer>();

            int width = map.Settings.MapWidth/2 +1;
            int height = map.Settings.MapHeight/2 +1;
            int x = 0;
            int y = 0;

            Vector3[] vertices = new Vector3[height * width];
            Color[] colors = new Color[height * width];
            Vector2[] uvs = new Vector2[vertices.Length];

            //to detatch the faces - there will be up to 6 faces at any given vertex, so the easy way is to make 6 vertices per map point
            Vector3[] detatchedVertices = new Vector3[height * width * 6];
            Color[] detatchedColors = new Color[height * width * 6];
            Vector2[] detatchedUVs = new Vector2[height * width * 6];

            //A queue of unused indices in the detatched buffer for the corresponding index in the vertex_per_point buffer.
            Queue<int>[] detachedIndices = new Queue<int>[height * width];

            //build heights and colours
            //and set uvs
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    int i = y * width + x;                    

                    float vertexHeight = map.GetSceneHeight(x, y, GameController.Instance.HeightMultiplier);

                    if (x == 10 && y == 10)
                    {
                        Debug.LogWarning("10,10 vheight is " + vertexHeight.ToString());
                    }

                    Vector3 vertex = new Vector3(x, vertexHeight, y);
                    vertices[i] = vertex;
                    colors[i] = getColorByHeight(vertexHeight / GameController.Instance.HeightMultiplier);

                    detachedIndices[i] = new Queue<int>();

                    for (int j = (i * 6); j < (i * 6) + 6; j++)
                    {
                        detatchedVertices[j] = vertices[i];
                        detatchedColors[j] = colors[i];
                        detatchedUVs[j] = uvs[i];
                        detachedIndices[i].Enqueue(j);
                    }
                }
            }

            //build triangle indices                
            int[] triangles = new int[(height -1) * (width -1) * 6];
            int index = 0;
            for (y = 0; y < height -1; y++)
            {
                for (x = 0; x < width -1; x++)
                {
                    int i1 = (y * width) + x;
                    int i2 = ((y + 1) * width) + x;
                    int i3 = (y * width) + x + 1;
                    int i4 = ((y + 1) * width) + x + 1;

                    float max1 = Mathf.Max(new float[] { vertices[i1].y, vertices[i2].y, vertices[i3].y });
                    float max2 = Mathf.Max(new float[] { vertices[i2].y, vertices[i3].y, vertices[i4].y });

                    //float av1 = (vertices[i1].y + vertices[i2].y + vertices[i3].y) / 3;
                    //float av2 = (vertices[i2].y + vertices[i3].y + vertices[i4].y) / 3;

                    Color c1 = getColorByHeight(max1 / GameController.Instance.HeightMultiplier);
                    Color c2 = getColorByHeight(max2 / GameController.Instance.HeightMultiplier);

                    //tri1 
                    triangles[index++] = detachedIndices[i1].Dequeue();
                    detatchedColors[triangles[index - 1]] = c1;
                    detatchedUVs[triangles[index - 1]] = new Vector2(0, 0);

                    triangles[index++] = detachedIndices[i2].Dequeue();
                    detatchedColors[triangles[index - 1]] = c1;
                    detatchedUVs[triangles[index - 1]] = new Vector2(0, 1);

                    triangles[index++] = detachedIndices[i3].Dequeue();
                    detatchedColors[triangles[index - 1]] = c1;
                    detatchedUVs[triangles[index - 1]] = new Vector2(1, 1);

                    //tri2
                    triangles[index++] = detachedIndices[i2].Dequeue();
                    detatchedColors[triangles[index - 1]] = c2;
                    detatchedUVs[triangles[index - 1]] = new Vector2(0, 0);

                    triangles[index++] = detachedIndices[i4].Dequeue();
                    detatchedColors[triangles[index - 1]] = c2;
                    detatchedUVs[triangles[index - 1]] = new Vector2(0, 1);

                    triangles[index++] = detachedIndices[i3].Dequeue();
                    detatchedColors[triangles[index - 1]] = c2;
                    detatchedUVs[triangles[index - 1]] = new Vector2(1, 1);
                }
            }

            //assign to mesh
            Mesh m = new Mesh();
            m.vertices = detatchedVertices;
            m.colors = detatchedColors;
            m.triangles = triangles;
            m.uv = detatchedUVs;
            m.RecalculateBounds();
            m.RecalculateNormals();
            filter.mesh = m;
        }
        else if (!loggedMissingMapError)
        {
            // map seems to be missing or broken
            Debug.LogError("Terrain Component '" + this.name + "' has no map data!");
            loggedMissingMapError = true;
        }
    }

    void Awake()
    {
        InitialiseColourTemplates();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }    
}
