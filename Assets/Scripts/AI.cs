using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AI : MonoBehaviour {

	public Transform target;

	private List<TileManager.Tile> tiles = new List<TileManager.Tile>();
	private TileManager manager;
	public GhostMove ghost;

	public TileManager.Tile nextTile = null;
	public TileManager.Tile targetTile;
	TileManager.Tile currentTile;

	void Awake()
	{
		manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
		tiles = manager.tiles;
	}

	public void RunLogic()
	{
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];
        
		if(ghost.direction.x > 0)
            nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)
            nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)
            nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)
            nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];

		if(nextTile.occupied || currentTile.isIntersection)
		{
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)
                        ghost.direction = Vector3.up;
					else
                        ghost.direction = Vector3.down;					
				}				
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)
                        ghost.direction = Vector3.right; 
					else
                        ghost.direction = Vector3.left;					
				}				
			}
			
			if(currentTile.isIntersection)
			{
				List<TileManager.Tile> availableTiles = new List<TileManager.Tile>();
				TileManager.Tile chosenTile;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0))
                    availableTiles.Add(currentTile.up);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0))
                    availableTiles.Add(currentTile.down);	
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0))
                    availableTiles.Add(currentTile.left);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))
                    availableTiles.Add(currentTile.right);

				int rand = Random.Range(0, availableTiles.Count);
				chosenTile = availableTiles[rand];
				ghost.direction = Vector3.Normalize(new Vector3(chosenTile.x - currentTile.x, chosenTile.y - currentTile.y, 0));
			}			
		}		
		else
		{
			ghost.direction = ghost.direction;
		}
	}
}