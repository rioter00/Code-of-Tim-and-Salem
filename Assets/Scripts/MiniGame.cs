using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using drnick;

namespace drnick
{
    public class MiniGame : MonoBehaviour
    {
        public CubeFace[] faces = new CubeFace[6];

        [SerializeField]
        private Vector2Int gridSize;
        [Tooltip("Multiple of base tile 1"), SerializeField]
        public float tileSize;
        public TileGrid activeGrid;

        //
        public float tickTime;
        float currentTime;
        public bool gameStarted = false;

        // Start is called before the first frame update
        void Start()
        {
            generateGrids();
            currentTime = tickTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameStarted = true;
            }

            if (gameStarted)
            {
                if(currentTime > 0)
                {
                    currentTime -= Time.deltaTime;
                } else
                {
                    currentTime = tickTime;
                    foreach (CubeFace face in faces)
                    {
                        face.grid?.spread();
                    }
                }
            }
        }

        public void startGame()
        {
            gameStarted = true;
        }

        public void stopGame()
        {
            gameStarted = false;
        }

        public void tileClicked(Tile tile)
        {
            Debug.Log("Tile Clicked - " + tile);
            activeGrid.tileClicked(tile);
        }

        public void generateGrids()
        {
            if (gridSize.x < 3 || gridSize.y < 3)
            {
                Debug.LogError("Grid Size is too small, check the vales in MiniGame script");
                return;
            }

            foreach (CubeFace face in faces)
            {
                face.grid?.setSizes(gridSize, tileSize);
                face.grid?.generateGrid();
            }
        }
    }
}
