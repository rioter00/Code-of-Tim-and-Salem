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
        float tileSize;
        //
        public Camera mainCamera;
        public TileGrid activeGrid;
        public int activeCheckLimiter;
        public int activeCheckCount;
        public LayerMask tileGridMask;
        //
        public float tickTime;
        float currentTime;
        public bool gameStarted = false;


        // Start is called before the first frame update
        void Start()
        {
            generateGrids();
            currentTime = tickTime;
            mainCamera = (mainCamera == null) ? Camera.main : null;
            Invoke("startGame", 4f);
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
                        if (activeGrid == face.grid) continue;
                        face.grid?.spread();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            activeCheckCount++;
            if (activeCheckCount % activeCheckLimiter != 0)
            {
                return;
            }
            
            activeCheckCount = 0;
            if (Physics.Raycast(mainCamera.transform.position, Vector3.forward, out RaycastHit hitInfo, 1000, tileGridMask))
            {
                //print("hit! " + hitInfo.collider.gameObject.name);
                if (hitInfo.collider.transform.parent.TryGetComponent(out TileGrid tileGrid))
                {
                    activeGrid = tileGrid;
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
                // math to resize the tiles -- demo size is 10 x 10 with a size of 1
                // 100 x 100 would be .1

                tileSize = (float) 10 / gridSize.x;
            
                face.grid?.setSizes(gridSize, tileSize);
                face.grid?.generateGrid();
            }
        }
    }
}
