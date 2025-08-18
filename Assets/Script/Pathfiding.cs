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
        Vector3Int mapSize = new(mapData.GetLength(0), mapData.GetLength(1), mapData.GetLength(2));

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
        public readonly Vector3Int coordinate;
        public readonly List<Node> neighbor = new List<Node>();

        public Node(Vector3Int coordinate)
        {
            this.coordinate = coordinate;
        }

        public void RefreshLink(Dictionary<Vector3Int, Node> pathMap)
        {

            Vector3Int[] offsets = new Vector3Int[] {
                new(1,0,0),
                new(-1,0,0),
                new(0,0,1),
                new(0,0,-1),
                new(1,1,0),
                new(-1,1,0),
                new(0,1,1),
                new(0,1,-1),
                new(1,-1,0),
                new(-1,-1,0),
                new(0,-1,1),
                new(0,-1,-1),
            };
            neighbor.Clear();

            foreach (Vector3Int offset in offsets)
            {
                Node node = null;
                bool succes = pathMap.TryGetValue(coordinate + offset, out node);
                if (succes) neighbor.Add(node);
            }
        }

        public override int GetHashCode()
        {
            return coordinate.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && coordinate.Equals(node.coordinate);
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


    public bool AStar(Vector3Int startingCoordinate, Vector3Int endingCoordinate, out List<Vector3Int> path)
    {
        path = new List<Vector3Int>();

        if (startingCoordinate.Equals(endingCoordinate))
        {
            path.Add(startingCoordinate);
            return true;
        }

        Dictionary<Node, float> nodeCost = new Dictionary<Node, float>();
        Dictionary<Node, Node> parent = new Dictionary<Node, Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        PriorityQueue<Node> openList = new PriorityQueue<Node>();

        Node startNode;
        bool isStartingNodeValid = pathMap.TryGetValue(startingCoordinate, out startNode);
        bool isEndingNodeValid = pathMap.ContainsKey(endingCoordinate);

        if (!isStartingNodeValid || !isEndingNodeValid)
        {
            Debug.LogWarning("Starting or ending node not found");
            return false;
        }

        openList.Enqueue(startNode, 0f);
        nodeCost[startNode] = 0f;

        while (openList.Count > 0)
        {
            Node current = openList.Dequeue();

            if (closedList.Contains(current))
                continue;

            closedList.Add(current);

            if (current.coordinate == endingCoordinate)
            {
                // Reconstruire le chemin
                path.Add(current.coordinate);
                while (parent.ContainsKey(current))
                {
                    current = parent[current];
                    path.Add(current.coordinate);
                }
                path.Reverse();
                return true;
            }

            float currentCost = nodeCost[current];

            foreach (Node neighbor in current.neighbor)
            {
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeCost = currentCost + 1f; // ou distance réelle si besoin

                bool visited = nodeCost.TryGetValue(neighbor, out float existingCost);
                if (!visited || tentativeCost < existingCost)
                {
                    nodeCost[neighbor] = tentativeCost;
                    parent[neighbor] = current;

                    float heuristic = Vector3Int.Distance(neighbor.coordinate, endingCoordinate);
                    float totalCost = tentativeCost + heuristic;
                    openList.Enqueue(neighbor, totalCost);
                }
            }
        }

        // Aucun chemin trouvé
        return false;
    }

}
