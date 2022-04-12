using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public TileState state;
    public bool originTile;

    SpriteRenderer sr;

    [Header("State Sprites")]
    public Sprite open, filled, one_quart, half, three_quart;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void setOriginTile()
    {
        originTile = true;
        setState(TileState.filled);
    }

    public void setState(TileState state)
    {
        this.state = state;
        if(state == TileState.filled)
        {
            sr.sprite = filled;
        }
        if(state == TileState.open)
        {
            clearVirus();
        }
    }

    public void setPosition(Vector2Int position)
    {
        this.position = position;
    }

    public Vector2Int getPosition()
    {
        return this.position;
    }

    public bool clearVirus()
    {
        if (!originTile)
        {
            state = TileState.open;
            sr.sprite = open;
            return true;
        }
        else
        {
            return false;
        }
    }

    public TileState getState()
    {
        return state;
    }
}
