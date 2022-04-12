using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using drnick;

namespace drnick
{
    public class MouseToGrid : MonoBehaviour
    {
        MiniGame game;

        private void Awake() => game = GetComponent<MiniGame>();

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0) && (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitData, 100)))
            {
                if (hitData.transform.TryGetComponent(out Tile tile))
                {
                    game.tileClicked(tile);
                }
            }
        }
    }
}
