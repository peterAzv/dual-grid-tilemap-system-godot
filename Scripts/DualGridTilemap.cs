using Godot;
using System;
using System.Collections.Generic;
using static TileType;

public partial class DualGridTilemap : TileMap {
    [Export] TileMap displayTilemap;
    private Vector2 offset = new Vector2(0.5f, 0.5f);
    protected static Vector2[] NEIGHBOURS = new Vector2[] {
        new(-0.5f, -0.5f),
        new(0.5f, -0.5f),
        new(-0.5f, 0.5f),
        new(0.5f, 0.5f),
    };

    protected static readonly Dictionary<Tuple<TileType, TileType, TileType, TileType>, Vector2I> neighbourTupleToAtlasCoord = new() {
        {new (Grass, Grass, Grass, Grass), new Vector2I(2, 1)}, // DEFAULT
        {new (Dirt, Dirt, Dirt, Grass), new Vector2I(1, 3)}, // OUTER_BOTTOM_RIGHT
        {new (Dirt, Dirt, Grass, Dirt), new Vector2I(0, 0)}, // OUTER_BOTTOM_LEFT
        {new (Dirt, Grass, Dirt, Dirt), new Vector2I(0, 2)}, // OUTER_TOP_RIGHT
        {new (Grass, Dirt, Dirt, Dirt), new Vector2I(3, 3)}, // OUTER_TOP_LEFT
        {new (Dirt, Grass, Dirt, Grass), new Vector2I(1, 0)}, // EDGE_RIGHT
        {new (Grass, Dirt, Grass, Dirt), new Vector2I(3, 2)}, // EDGE_LEFT
        {new (Dirt, Dirt, Grass, Grass), new Vector2I(3, 0)}, // EDGE_BOTTOM
        {new (Grass, Grass, Dirt, Dirt), new Vector2I(1, 2)}, // EDGE_TOP
        {new (Dirt, Grass, Grass, Grass), new Vector2I(1, 1)}, // INNER_BOTTOM_RIGHT
        {new (Grass, Dirt, Grass, Grass), new Vector2I(2, 0)}, // INNER_BOTTOM_LEFT
        {new (Grass, Grass, Dirt, Grass), new Vector2I(2, 2)}, // INNER_TOP_RIGHT
        {new (Grass, Grass, Grass, Dirt), new Vector2I(3, 1)}, // INNER_TOP_LEFT
        {new (Dirt, Grass, Grass, Dirt), new Vector2I(2, 3)}, // DUAL_UP_RIGHT
        {new (Grass, Dirt, Dirt, Grass), new Vector2I(0, 1)}, // DUAL_DOWN_RIGHT
		{new (Dirt, Dirt, Dirt, Dirt), new Vector2I(0, 3)},
    };

    [Export] public Vector2I grassPlaceholderAtlasCoord;
    [Export] public Vector2I dirtPlaceholderAtlasCoord;

    public override void _Ready() {
        base._Ready();
        RefreshDisplayLayer();
    }

    public void SetTile(Vector2I coords, Vector2I atlasCoords) {
        SetCell(0, coords, 0, atlasCoords);
        setDisplayTile(coords);
    }

    protected void setDisplayTile(Vector2I pos) {
        // loop through 4 display neighbours
        for (int i = 0; i < NEIGHBOURS.Length; i++) {
            Vector2I newPos = (Vector2I)(pos + NEIGHBOURS[i] + offset);
            displayTilemap.SetCell(0, newPos, 1, calculateDisplayTile(newPos));
        }
    }

    protected Vector2I calculateDisplayTile(Vector2I coords) {
        // get 4 world tile neighbours
        TileType botRight = getWorldTile((Vector2I)(coords - NEIGHBOURS[0] - offset));
        TileType botLeft = getWorldTile((Vector2I)(coords - NEIGHBOURS[1] - offset));
        TileType topRight = getWorldTile((Vector2I)(coords - NEIGHBOURS[2] - offset));
        TileType topLeft = getWorldTile((Vector2I)(coords - NEIGHBOURS[3] - offset));

        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, botLeft, botRight);

        return neighbourTupleToAtlasCoord[neighbourTuple];
    }

    private TileType getWorldTile(Vector2I coords) {
        Vector2I atlasCoord = GetCellAtlasCoords(0, coords);
        if (atlasCoord == grassPlaceholderAtlasCoord)
            return Grass;
        else
            return Dirt;
    }

    public void RefreshDisplayLayer() {
        foreach (Vector2I coord in GetUsedCells(0)) {
            setDisplayTile(coord);
        }
    }
}

public enum TileType {
    None,
    Grass,
    Dirt
}