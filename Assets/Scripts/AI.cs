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

	public void AILogic()
	{
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];
		
		targetTile = GetTargetTilePerGhost();
		
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];
		
		if(nextTile.occupied || currentTile.isIntersection)
		{
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;
					
				}
				
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;
					
				}
				
			}

			if(currentTile.isIntersection)
			{
				
				float dist1, dist2, dist3, dist4;
				dist1 = dist2 = dist3 = dist4 = 999999f;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 		dist1 = manager.distance(currentTile.up, targetTile);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	dist2 = manager.distance(currentTile.down, targetTile);
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 	dist3 = manager.distance(currentTile.left, targetTile);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	dist4 = manager.distance(currentTile.right, targetTile);
				
				float min = Mathf.Min(dist1, dist2, dist3, dist4);
				if(min == dist1) ghost.direction = Vector3.up;
				if(min == dist2) ghost.direction = Vector3.down;
				if(min == dist3) ghost.direction = Vector3.left;
				if(min == dist4) ghost.direction = Vector3.right;
				
			}
			
		}
		
		else
		{
			ghost.direction = ghost.direction;
		}
	}

	public void RunLogic()
	{
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];
        
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];

		if(nextTile.occupied || currentTile.isIntersection)
		{
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;					
				}
				
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;					
				}
				
			}
			
			if(currentTile.isIntersection)
			{
				List<TileManager.Tile> availableTiles = new List<TileManager.Tile>();
				TileManager.Tile chosenTile;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 			availableTiles.Add (currentTile.up);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	availableTiles.Add (currentTile.down);	
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 		availableTiles.Add (currentTile.left);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	availableTiles.Add (currentTile.right);

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


	TileManager.Tile GetTargetTilePerGhost()
	{
		Vector3 targetPos;
		TileManager.Tile targetTile;
		Vector3 dir;
        
		switch(name)
		{
		case "blinky":
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "pinky":
			dir = target.GetComponent<PlayerController>().getDir();
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f) + 4*dir;
                
			if(dir == Vector3.up)	targetPos -= new Vector3(4, 0, 0);

			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "inky":
			dir = target.GetComponent<PlayerController>().getDir();
			Vector3 blinkyPos = GameObject.Find ("blinky").transform.position;
			Vector3 ambushVector = target.position + 2*dir - blinkyPos ;
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f) + 2*dir + ambushVector;
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "clyde":
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			if(manager.distance(targetTile, currentTile) < 9)
				targetTile = tiles[manager.Index (0, 2)];
			break;
		default:
			targetTile = null;
			break;
		
		}
		return targetTile;
	}
}