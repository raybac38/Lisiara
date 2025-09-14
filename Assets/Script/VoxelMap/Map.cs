using UnityEngine;

[RequireComponent (typeof(MapRenderer))]
public class Map : MonoBehaviour
{
    [SerializeField]
    private MapRenderer mapRenderer;
    public readonly MapData mapData = new ();

    private void OnDestroy()
    {
        mapData.Dispose ();
    }

}
