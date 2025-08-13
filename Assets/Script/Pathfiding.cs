using System.Collections.Generic;
using UnityEngine;

public class Pathfiding
{
    private Dictionary<Vector3Int, Node> pathMap = new Dictionary<Vector3Int, Node>();

    /// <summary>
    /// Generate the initial pathmap from the map data
    /// </summary>
    /// <param name="map"></param>
    public void GeneratePathMap(bool[,,] mapData)
    {
        Vector3Int mapSize = new Vector3Int(mapData.GetLength(0), mapData.GetLength(1), mapData.GetLength(2));

        /// Generate all Node
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    if (!IsWalkable(mapData, new Vector3Int(x, y, z)))
                    {
                        continue;
                    }
                    Vector3Int coordinate = new Vector3Int(x, y, z);
                    pathMap.Add(coordinate, new Node(coordinate));
                }
            }
        }
        RefreshAllNodeLink(pathMap);

    }
    class Node
    {
        private Vector3Int coordinate;
        readonly List<Node> neighbor = new List<Node>();

        public Node(Vector3Int coordinate)
        {
            this.coordinate = coordinate;
        }

        public void RefreshLink(Dictionary<Vector3Int, Node> pathMap)
        {

            Vector3Int[] offsets = new Vector3Int[] {
                new Vector3Int(1,0,0),
                new Vector3Int(-1,0,0),
                new Vector3Int(0,0,1),
                new Vector3Int(0,0,-1),
                new Vector3Int(1,1,0),
                new Vector3Int(-1,1,0),
                new Vector3Int(0,1,1),
                new Vector3Int(0,1,-1),
                new Vector3Int(1,-1,0),
                new Vector3Int(-1,-1,0),
                new Vector3Int(0,-1,1),
                new Vector3Int(0,-1,-1),
            };
            neighbor.Clear();

            foreach (Vector3Int offset in offsets)
            {
                Node node = null;
                bool succes = pathMap.TryGetValue(coordinate + offset, out node);
                if (succes) neighbor.Add(node);
            }
        }
    }

    private void RefreshAllNodeLink(Dictionary<Vector3Int, Node> pathMap)
    {
        foreach (Node node in pathMap.Values)
        {
            node.RefreshLink(pathMap);
        }
    }

    private bool IsWalkable(bool[,,] mapData, Vector3Int coordinate)
    {
        int maxY = mapData.GetLength(1);
        int y = coordinate.y;
        if (y <= 0 || y >= maxY - 1) return false;

        int x = coordinate.x;
        int z = coordinate.z;
        bool isGroundBelow = mapData[x, y - 1, z];
        bool isAirAtFeet = !mapData[x, y, z];
        bool isAirAtHead = !mapData[x, y + 1, z];
        return isGroundBelow && isAirAtFeet && isAirAtHead;
    }
}
