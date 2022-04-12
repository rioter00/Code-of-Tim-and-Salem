using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace drnick
{
    public class TileGrid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public Transform startPoint;
        [SerializeField]
        private Vector2Int gridSize;
        [Tooltip("size by size"), SerializeField]
        public float tileSize;

        private Tile[,] tiles;
        public List<Tile> openTiles = new List<Tile>();
        public List<Tile> filledTiles = new List<Tile>();

        public Dictionary<TileDirection, Vector2Int> fourDirectionsDictionary = new Dictionary<TileDirection, Vector2Int>
        {
            {TileDirection.up, new Vector2Int (0, 1) },
            {TileDirection.right, new Vector2Int (1, 0) },
            {TileDirection.down, new Vector2Int (0, -1) },
            {TileDirection.left, new Vector2Int (-1, 0) }
        };

        public List<Vector2Int> eightDirectionsDictionary = new List<Vector2Int>
        {
            new Vector2Int (0, 1),
            new Vector2Int (1, 0),
            new Vector2Int (0, -1),
            new Vector2Int (-1, 0),

            new Vector2Int (1, 1),
            new Vector2Int (1, -1),
            new Vector2Int (-1, -1),
            new Vector2Int (-1, 1),
        };


        // Start is called before the first frame update
        void Start()
        {
  
        }

        /// <summary>
        /// Set GridSize and TileSizes
        /// </summary>
        /// <param name="gridSize">Vector2</param>
        /// <param name="tileSize">Float</param>
        public void setSizes(Vector2Int gridSize, float tileSize)
        {
            this.gridSize = gridSize;
            this.tileSize = tileSize;

            float offset = (gridSize.x % 2 == 0) ? tileSize * .5f : 0;

            float startOffset = (gridSize.x / 2) * tileSize - offset;
            startPoint.position -= new Vector3(startOffset, startOffset, 0);
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    spread();
            //}
        }

        void setGridSize(Vector2Int gridSize)
        {
            this.gridSize = gridSize;
        }

        /// <summary>
        /// Generate the Grid
        /// </summary>
        /// <returns></returns>
        public bool generateGrid()
        {
            if (gridSize.x < 3 || gridSize.y < 3)
            {
                Debug.LogError("Grid Size is too small, check the vales in MiniGame script");
                return false;
            }

            tiles = new Tile[gridSize.x, gridSize.y];

            for (int i = 0; i < gridSize.y; i++)
            {
                for (int j = 0; j < gridSize.x; j++)
                {
                    Vector3 newPos = new Vector3(startPoint.position.x + i * tileSize, startPoint.position.y + j * tileSize, Mathf.Epsilon);
                    tiles[i, j] = Instantiate(tilePrefab, newPos, Quaternion.identity, null).GetComponent<Tile>();
                    tiles[i, j].clearVirus();
                    openTiles.Add(tiles[i, j]);
                    tiles[i, j].setPosition(new Vector2Int(i,j));
                    tiles[i, j].name = "Tile: " + i + ", " + j;
                }
            }

            // update origin tiles
            if (gridSize.x % 2 == 1)
            {
                int min = (int)Mathf.Floor(gridSize.x / 2) + 1;
                Debug.Log("Setting origin tiles at " + (min - 1));
                tiles[min - 1, min - 1].setOriginTile();
                fillTile(tiles[min - 1, min - 1]);
            }
            else
            {
                int min = (int)Mathf.Floor(gridSize.x / 2);
                tiles[min - 1, min - 1].setOriginTile();
                tiles[min, min - 1].setOriginTile();
                tiles[min - 1, min].setOriginTile();
                tiles[min, min].setOriginTile();
                fillTile(tiles[min - 1, min - 1]);
                fillTile(tiles[min, min - 1]);
                fillTile(tiles[min - 1, min]);
                fillTile(tiles[min, min]);
            }
            return true;
        }

        public void openTile(Tile tile)
        {
            openTiles.Add(tile);
            filledTiles.Remove(tile);
            tile.setState(TileState.open);
        }

        public void fillTile(Tile tile)
        {
            openTiles.Remove(tile);
            filledTiles.Add(tile);
            tile.setState(TileState.filled);
        }

        public void spread()
        {
            Debug.Log("Filled: " + filledTiles.Count + " " + (gridSize.x * gridSize.y));
            if(filledTiles.Count >= (gridSize.x * gridSize.y))
            {
                Debug.LogError("Grid Filled");
                return;
            }
            // random select filled cell
            //Debug.Log(getOpenNeighbor(getRandomFilledTile()).getPosition());
            Tile newTile;
            do
            {
                newTile = getOpenNeighbor(getRandomFilledTile());
            } while (newTile == null);
             
            fillTile(newTile);
        }

        public Tile getRandomFilledTile()
        {
            return filledTiles[Random.Range(0, filledTiles.Count)];
        }

        public Tile getOpenNeighbor(Tile tile)
        {
            List<TileDirection> dirsChecked = new List<TileDirection>();

            do
            {
                Debug.Log("Starting cycle..");
                TileDirection dir = (TileDirection)Random.Range(0, 4);
                if (dirsChecked.Contains(dir))
                {
                    Debug.Log("Dir already checked; Repeating random search");
                    continue;
                }

                Vector2Int checkVector = fourDirectionsDictionary[dir];

                Vector2Int tilePos = new Vector2Int(tile.position.x + checkVector.x, tile.position.y + checkVector.y);

                Debug.Log("tile pos: " + tilePos);
                if (tilePos.x < 0 || tilePos.x > (gridSize.x - 1) || tilePos.y < 0 || tilePos.y > (gridSize.y - 1))
                {
                    Debug.Log("Tile pos out of bounds, repeating: " + tilePos);
                    dirsChecked.Add(dir);
                    Debug.Log("Dirs Checked: " + dirsChecked.Count);
                    continue;
                }

                if (tiles[tilePos.x, tilePos.y].state == TileState.open)
                {
                    return tiles[tilePos.x, tilePos.y];
                }
                else
                {
                    dirsChecked.Add(dir);
                    Debug.Log("Dirs Checked: " + dirsChecked.Count);
                }
            } while (dirsChecked.Count < 4);

            return null;
            // 
        }

        public void tileClicked(Tile tile)
        {
            if(tile.getState() == TileState.filled)
            {
                openTile(tile);

                // run orphan check
                orphanCheck(tile);
            }            
            //tile.clearVirus();
        }

        private bool checkForOpenTile(Tile tile)
        {

            return true;
        }

        private void orphanCheck(Tile tile)
        {
            List<Tile> orphans = new List<Tile>();
            Debug.Log("running orphan check");
            //watch out nest foreach!
            foreach (Vector2Int checkVector in fourDirectionsDictionary.Values)
            {
                Vector2Int tilePos = new Vector2Int(tile.position.x + checkVector.x, tile.position.y + checkVector.y);

                Debug.Log("tile pos: " + tilePos);
                if (tilePos.x < 0 || tilePos.x > (gridSize.x - 1) || tilePos.y < 0 || tilePos.y > (gridSize.y - 1))
                {
                    continue;
                }

                if (tiles[tilePos.x, tilePos.y].state == TileState.filled)
                {
                    Tile tile2 = tiles[tilePos.x, tilePos.y];

                    int checkIterator = 0;

                    foreach (Vector2Int checkVector2 in fourDirectionsDictionary.Values)
                    {
                        Vector2Int secTile = new Vector2Int(tile2.position.x + checkVector2.x, tile2.position.y + checkVector2.y);

                        Debug.LogWarning("Checking parents " + tile2.position + " at " + secTile);
                        if (secTile.x < 0 || secTile.x > (gridSize.x - 1) || secTile.y < 0 || secTile.y > (gridSize.y - 1))
                        {
                            checkIterator++;
                            //Debug.Log("Sec " + secTile + " tile out of bounds: " + secTile);
                            continue;
                        }

                        if (tiles[secTile.x, secTile.y].state == TileState.filled)
                        {
                            //Debug.Log("Sec "+ secTile +  " tile state: " + tiles[secTile.x, secTile.y].state);
                            break;
                        }
                        checkIterator++;
                    }
                    Debug.Log("Check Iterator: " + checkIterator);
                    if (checkIterator == 4)
                    {
                        Debug.LogError("Found orphan at " + tile2.position + " : " + tile2.state.ToString());
                        openTile(tile2);
                    }

                }
            }
        }
    }
}
