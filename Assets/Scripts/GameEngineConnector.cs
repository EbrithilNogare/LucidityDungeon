using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class GameEngineConnector : MonoBehaviour
    {
        public Tilemap tilemap;
        public Tile[] floorTiles;
        public Tile[] wallTiles;
        public Tile[] edgeUpTiles;
        public Tile[] edgeLeftTiles;
        public Tile[] edgeRightTiles;
        public Tile[] edgeUpLeftTiles;
        public Tile[] edgeUpRightTiles;
        public Tile[] nothingTiles;

        private GameEngine gameEngine;
        
        void Start()
        {
            Config config = new Config();
            gameEngine = new GameEngine(config);
        }

        private bool first = true;
        private void Update()
        {
            if (!first)
            {
                return;
            }
            first = false;

            Coordinate pos = new Coordinate(0, 0);
            RenderRoom(gameEngine.map[pos], pos);
        }

        RoomTileType[,] defaultRoom = {
            { RoomTileType.R, RoomTileType.W, RoomTileType.W, RoomTileType.W, RoomTileType.W, RoomTileType.W, RoomTileType.W, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.R, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.F, RoomTileType.L },
            { RoomTileType.N, RoomTileType.U, RoomTileType.U, RoomTileType.U, RoomTileType.U, RoomTileType.U, RoomTileType.U, RoomTileType.N },
        };

        void RenderRoom(MapTile mapTile, Coordinate coordinate)
        {
            RoomTileType[,] room = (RoomTileType[,])defaultRoom.Clone();

            if (mapTile.entries.up)
            {
                room[0, 3] = RoomTileType.F;
                room[0, 4] = RoomTileType.F;
            }

            if (mapTile.entries.left)
            {
                room[2, 0] = RoomTileType.W;
                room[3, 0] = RoomTileType.F;
                room[4, 0] = RoomTileType.F;
                room[5, 0] = RoomTileType.UR;
            }

            if (mapTile.entries.down)
            {
                room[7, 2] = RoomTileType.UR;
                room[7, 3] = RoomTileType.F;
                room[7, 4] = RoomTileType.F;
                room[7, 5] = RoomTileType.UL;
            }

            if (mapTile.entries.right)
            {
                room[2, 7] = RoomTileType.W;
                room[3, 7] = RoomTileType.F;
                room[4, 7] = RoomTileType.F;
                room[5, 7] = RoomTileType.UL;
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    switch (room[7-y, x])
                    {
                        case RoomTileType.F:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), floorTiles[(x*13+y*29) % floorTiles.Length]);
                            break;
                        case RoomTileType.W:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), wallTiles[0]);
                            break;
                        case RoomTileType.U:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), edgeUpTiles[0]);
                            break;
                        case RoomTileType.L:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), edgeLeftTiles[0]);
                            break;
                        case RoomTileType.R:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), edgeRightTiles[0]);
                            break;
                        case RoomTileType.UL:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), edgeUpLeftTiles[0]);
                            break;
                        case RoomTileType.UR:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), edgeUpRightTiles[0]);
                            break;
                        case RoomTileType.N:
                            tilemap.SetTile(new Vector3Int(coordinate.x+x, coordinate.y+y, 0), nothingTiles[0]);
                            break;
                    }
                }
            }

        }
    }
}
