﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TileManager : MonoBehaviour {

	public class Tile
	{
		public int x { get; set; }
		public int y { get; set; }
		public bool occupied { get; set; }
		public int adjacentCount { get; set; }
		public bool isIntersection { get; set; }
		
		public Tile left, right, up, down;
		
		public Tile(int x_in, int y_in)
		{
			x = x_in; y = y_in;
			occupied = false;
			left = right = up = down = null;
		}


	};
	
	public List<Tile> tiles = new List<Tile>();
	
	void Start () 
	{
        ReadTiles();
	}
    
	void Update () 
	{

	}
	
    void ReadTiles()
    {
        string data = @"0000000000000000000000000000
0111111111111001111111111110
0100001000001001000001000010
0100001000001111000001000010
0100001000001001000001000010
0111111111111001111111111110
0100001001000000001001000010
0100001001000000001001000010
0111111001111001111001111110
0001001000001001000001001000
0001001000001001000001001000
0111001111111111111111001110
0100001001000000001001000010
0100001001000000001001000010
0111111001000000001001111110
0100001001000000001001000010
0100001001000000001001000010
0111001001111111111001001110
0001001001000000001001001000
0001001001000000001001001000
0111111111111111111111111110
0100001000001001000001000010
0100001000001001000001000010
0111001111111001111111001110
0001001001000000001001001000
0001001001000000001001001000
0111111001111001111001111110
0100001000001001000001000010
0100001000001001000001000010
0111111111111111111111111110
0000000000000000000000000000";

        int X = 1, Y = 31;
        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {

                X = 1; 
                for (int i = 0; i < line.Length; ++i)
                {
                    Tile newTile = new Tile(X, Y);
                    
                    if (line[i] == '1')
                    {
                        if (i != 0 && line[i - 1] == '1')
                        {
                            newTile.left = tiles[tiles.Count - 1];
                            tiles[tiles.Count - 1].right = newTile;
                            
                            newTile.adjacentCount++;
                            tiles[tiles.Count - 1].adjacentCount++;
                        }
                    }
                    
                    else newTile.occupied = true;
                    
                    int upNeighbor = tiles.Count - line.Length;
                    if (Y < 30 && !newTile.occupied && !tiles[upNeighbor].occupied)
                    {
                        tiles[upNeighbor].down = newTile;
                        newTile.up = tiles[upNeighbor];
                        
                        newTile.adjacentCount++;
                        tiles[upNeighbor].adjacentCount++;
                    }

                    tiles.Add(newTile);
                    X++;
                }

                Y--;
            }
        }
        
        foreach (Tile tile in tiles)
        {
            if (tile.adjacentCount > 2)
                tile.isIntersection = true;
        }

    }

	public int Index(int X, int Y)
	{
		if(X >= 1 && X <= 28 && Y <= 31 && Y >= 1)
			return (31 - Y) * 28 + X - 1;
        
	    if(X < 1)
            X = 1;
	    if(X > 28)
            X = 28;
	    if(Y < 1)
            Y = 1;
	    if(Y > 31)
            Y = 31;

	    return (31 - Y) * 28 + X - 1;
	}
	
	public int Index(Tile tile)
	{
		return (31 - tile.y) * 28 + tile.x - 1;
	}
    
	public float distance(Tile tile1, Tile tile2)
	{
		return Mathf.Sqrt(Mathf.Pow(tile1.x - tile2.x, 2) + Mathf.Pow(tile1.y - tile2.y, 2));
	}
}
