using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject[] tilePrefabs;
    private Point bridgeSpawn;
    [SerializeField]
    private GameObject waterBridgePrefab;
    private Point[] startPoints;
    [SerializeField]
    private GameObject startPointPrefab;
    [SerializeField]
    private GameObject startPointPrefab1;

    public SpawnPlace[] TreeScript { get; set; }
    [SerializeField]
    private Transform map;
    private Point mapSize;

    private Stack<Node> finalPath;
    private Stack<Node>[] pathArray;
    public Stack<Node>[] PathArray
    {
        get
        {
            if (pathArray == null)
            {
                GeneratePath();
            }
            Stack<Node>[] tmp =  new Stack<Node>[startPoints.Length-1];
            Array.Copy(pathArray, tmp, startPoints.Length-1);
            return tmp;
            //return new Stack<Node>(new Stack<Node>(pathArray))[startPoints.Length];
        }
    }

    public Dictionary<Point, TileScript> Tiles {get; set;}

    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;}

    }

    // Start is called before the first frame update
    void Start()
    {
        //Camera.main.transform = new Vector3(-43.89, 27.71, -10);

        CreateLevel();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        string[] mapData = ReadLevelText();
        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);
        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;
        Vector3 maxTile = Vector3.zero;
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        
        for (int y = 0; y < mapY; y ++) 
        {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);    
            }
        }
        maxTile = Tiles[new Point(mapX-1, mapY-1)].transform.position;
        SpawnBridgeAndTrees();

    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
        
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map);

    }

    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;
        string data = bindData.text.Replace(Environment.NewLine, string.Empty);
        return data.Split('-');
    }

    private void SpawnBridgeAndTrees()
    {
        bridgeSpawn = new Point(6, 4);
        Instantiate(waterBridgePrefab, Tiles[bridgeSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        int numX = mapSize.X;
        int numY = mapSize.Y;
        TreeScript = new SpawnPlace[numY];
        startPoints = new Point[mapSize.Y];
        GameObject tmp;
        for (int i = 0; i < numY; i++)
        {
            
            startPoints[i] = new Point(numX-1, i);
            int treeIndex = UnityEngine.Random.Range(0,2);
            string type = string.Empty; //set to prefab
            switch (treeIndex)
            {
                case 0:
                    tmp = (GameObject)Instantiate(startPointPrefab, Tiles[startPoints[i]].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
                    TreeScript[i] = tmp.GetComponent<SpawnPlace>();
                    TreeScript[i].name = "Tree";
                    break;
                case 1:
                    tmp = (GameObject)Instantiate(startPointPrefab1, Tiles[startPoints[i]].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
                    TreeScript[i] = tmp.GetComponent<SpawnPlace>();
                    TreeScript[i].name = "Tree";
                    break;
            }
            
        }
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y <mapSize.Y;
    }

    public void GeneratePath()
    {
        pathArray = new Stack<Node>[startPoints.Length-1];
        Point goalPoint = new Point(bridgeSpawn.X, bridgeSpawn.Y);
        for (int i = 0; i < pathArray.Length; i++)
        {
            
            pathArray[i] = AStar.GetPath(startPoints[i], goalPoint);
        }
        
    }


}
