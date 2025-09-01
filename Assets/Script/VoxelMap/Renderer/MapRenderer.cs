using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public Material defaultMaterial;
    public static Material defaultStaticMaterial;
    [SerializeField]
    private MapData mapData;

    [SerializeField]
    private Map map;

    private ChunkRenderer[,,] chunkRenderers;

    private void Awake()
    {
        defaultStaticMaterial = defaultMaterial;
    }

    private void Start()
    {
        mapData = map.MapData;
        chunkRenderers = new ChunkRenderer[MapData.mapSize.x, MapData.mapSize.y, MapData.chunkSize.z];
        for (int x = 0; x < MapData.mapSize.x; x++)
        {
            for (int y = 0; y < MapData.mapSize.y; y++)
            {
                for (int z = 0; z < MapData.mapSize.z; z++)
                {
                    GameObject chunk = new (string.Format("Chunk ", x, ":", y, ":", z));
                    ChunkRenderer chunkRenderer = chunk.AddComponent<ChunkRenderer>();
                    chunkRenderers[x, y, z] = chunkRenderer; 
                    chunkRenderer.mapData = mapData;
                    chunkRenderer.chunkPosition = new Vector3Int(x, y, z);
                    chunk.transform.position = Vector3.zero;
                    chunkRenderer.GenerateMesh();
                }
            }
        }
    }
}
