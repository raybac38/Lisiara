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
        chunkRenderers = new ChunkRenderer[MapData.MAP_SIZE_X, MapData.MAP_SIZE_Y, MapData.MAP_SIZE_Z];
        for (int x = 0; x < MapData.MAP_SIZE_X; x++)
        {
            for (int y = 0; y < MapData.MAP_SIZE_Y; y++)
            {
                for (int z = 0; z < MapData.MAP_SIZE_Z; z++)
                {
                    GameObject chunk = new (string.Format("Chunk ", x, ":", y, ":", z));
                    ChunkRenderer chunkRenderer = chunk.AddComponent<ChunkRenderer>();
                    chunkRenderers[x, y, z] = chunkRenderer; 
                    chunkRenderer.mapData = mapData;
                    chunkRenderer.chunkPosition = new Vector3Int(x, y, z);
                    chunk.transform.position = new Vector3(x * MapData.CHUNK_SIZE_X, y * MapData.CHUNK_SIZE_Y, z * MapData.CHUNK_SIZE_Z);
                    
                }
            }
        }
    }
}
