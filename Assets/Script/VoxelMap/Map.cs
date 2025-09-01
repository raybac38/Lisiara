using UnityEngine;

[RequireComponent (typeof(MapRenderer))]
public class Map : MonoBehaviour
{
    [SerializeField]
    private MapRenderer mapRenderer;
    public readonly MapData MapData = new MapData ();

    
}
