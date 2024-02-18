using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class TilemapAStar : MonoBehaviour
    {
        public Tile[] walkableTileTypes;

        private Tilemap tilemap;
        private int dealdLockLimit = 1000;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public class Node
        {
            public Vector2Int position;
            public int gCost;
            public int hCost;
            public Node parent;

            public int fCost { get { return gCost + hCost; } }

            public Node(Vector2Int position)
            {
                this.position = position;
            }
        }

        void Start()
        {
            tilemap = transform.GetComponent<Tilemap>();
        }

        public List<Direction> FindPath(Vector2Int start, Vector2Int goal)
        {
            List<Direction> path = new List<Direction>();

            Node startNode = new Node(start);
            Node goalNode = new Node(goal);

            SortedList<int, Node> openSet = new SortedList<int, Node>(new DuplicateKeyComparer<int>());
            HashSet<Node> closedSet = new HashSet<Node>(new NodePositionComparer());

            openSet.Add(startNode.fCost, startNode);

            while (openSet.Count > 0 && openSet.Count < dealdLockLimit)
            {
                Node currentNode = openSet.Values[0];
                openSet.RemoveAt(0);

                if (currentNode.position == goalNode.position)
                {
                    return RetracePath(startNode, currentNode);
                }

                closedSet.Add(currentNode);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                    if (!openSet.ContainsValue(neighbor) || newCostToNeighbor < neighbor.gCost)
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, goalNode);
                        neighbor.parent = currentNode;

                        if (!openSet.ContainsValue(neighbor))
                        {
                            openSet.Add(neighbor.fCost, neighbor);
                        }
                    }
                }
            }

            return path;
        }

        List<Direction> RetracePath(Node startNode, Node endNode)
        {
            List<Direction> path = new List<Direction>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                Vector2Int direction = currentNode.position - currentNode.parent.position;
                path.Insert(0, GetDirectionFromVector(direction));
                currentNode = currentNode.parent;
            }

            return path;
        }

        List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            Vector2Int[] directions =
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
            };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.position + dir;
                TileBase tile = tilemap.GetTile(new Vector3Int((int)neighborPos.x, (int)neighborPos.y, 0));

                if (IsWalkable(tile))
                {
                    neighbors.Add(new Node(neighborPos));
                }
            }

            return neighbors;
        }

        bool IsWalkable(TileBase tile)
        {
            foreach (var walkableTileType in walkableTileTypes)
            {
                if (tile == walkableTileType)
                {
                    return true;
                }
            }
            return false;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            return Mathf.Abs(nodeA.position.x - nodeB.position.x) + Mathf.Abs(nodeA.position.y - nodeB.position.y);
        }

        Direction GetDirectionFromVector(Vector2Int direction)
        {
            if (direction == Vector2Int.up)
                return Direction.Up;
            else if (direction == Vector2Int.down)
                return Direction.Down;
            else if (direction == Vector2Int.left)
                return Direction.Left;
            else if (direction == Vector2Int.right)
                return Direction.Right;
            else
                return Direction.Up;
        }

        public void PrintDebug(List<Direction> path, Vector2Int start)
        {
            Vector2 currentPosition = start;

            foreach (var dir in path)
            {
                Vector2 directionVector = Vector2.zero;

                switch (dir)
                {
                    case Direction.Up:
                        directionVector = Vector2.up;
                        break;
                    case Direction.Down:
                        directionVector = Vector2.down;
                        break;
                    case Direction.Left:
                        directionVector = Vector2.left;
                        break;
                    case Direction.Right:
                        directionVector = Vector2.right;
                        break;
                }

                currentPosition += directionVector;
            }
        }

        public class NodePositionComparer : IEqualityComparer<Node>
        {
            public bool Equals(Node x, Node y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null || y == null)
                    return false;

                return x.position.Equals(y.position);
            }

            public int GetHashCode(Node obj)
            {
                return obj.position.GetHashCode();
            }
        }
    }
}
