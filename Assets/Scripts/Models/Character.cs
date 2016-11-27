using UnityEngine;
using System.Collections;
using System;

public class Character {

	public float X {
		get {
			if (nextTile == null) 
				return CurrentTile.X;
			
			return Mathf.Lerp (CurrentTile.X, nextTile.X, movementPercentage);
		}
	}

	public float Y {
		get {
			if (nextTile == null) 
				return CurrentTile.Y;

			return Mathf.Lerp (CurrentTile.Y, nextTile.Y, movementPercentage);
		}
	}

	/// <summary>
	/// The tile I am in.
	/// </summary>
	/// <value>The current tile.</value>
	public Tile CurrentTile { get; protected set; }

	/// <summary>
	/// The final tile to arrive at after pathfinding
	/// </summary>
	public Tile DestinationTile {get; protected set;}
	/// <summary>
	/// The tile i am entering
	/// </summary>
	Tile nextTile;
	Path_AStar path;
	float movementPercentage; //Between 0 and 1
	float speed = 5f;

	Action<Character> OnCharacterChanged;

	Job currentJob;

	public Character(Tile tile) {
		CurrentTile = DestinationTile = nextTile = tile;
	}

	public void Update(float deltaTime) {
		UpdateJob (deltaTime);
		UpdateMovement (deltaTime);

		if (OnCharacterChanged != null) {
			OnCharacterChanged (this);
		}
	}

	public void RegisterOnChanged(Action<Character> listener) {
		OnCharacterChanged += listener;
	}

	public void UnRegisterOnChanged(Action<Character> listener) {
		OnCharacterChanged -= listener;
	}

	public bool IsMoving() {
		return path != null && CurrentTile != DestinationTile;
	}

	private void UpdateMovement(float deltaTime) {
		
		if (CurrentTile == DestinationTile) {
			path = null;
			return;
		}

		//we arrived at the next node or dont have one yet
		if (nextTile == null || nextTile == CurrentTile) {
			//if the pathing is not set yet
			if (path == null || path.Length () == 0) {
				//generate a path
				path = new Path_AStar(CurrentTile.World, CurrentTile, DestinationTile);
				if (path.Length () == 0) {
					Debug.Log ("Character could not path to destination");
					//TODO AbandonJob? throw event that job can listen to?
					path = null;
					return;
				}

				//skip the first node
				nextTile = path.Dequeue();
			}

			//get a new node off the list
			nextTile = path.Dequeue();

			if (nextTile == CurrentTile) {
				Debug.LogError ("Character :: Update movement, something went wrong.");
			}
		}

		//Make sure the shit hasnt hit the fan
		//This can happen if something was built while we were already moving.
		if (nextTile.IsEnterable () == Enterability.NEVER) {
			Debug.LogError ("TRYING TO ENTER UNWALKABLE TILE:: BOMBING");
			nextTile = null;
			path = null;
			return;

		} else if (nextTile.IsEnterable() == Enterability.SOON) {
			//we wait a frame
			return;
		}


		//NOW WE CAN MOVE

		//get the distance between our two nodes
		float distanceToTravel = Mathf.Sqrt (
			Mathf.Pow(CurrentTile.X - nextTile.X, 2) +
			Mathf.Pow(CurrentTile.Y - nextTile.Y, 2)
		);

		//how far can we go this frame?
		float distanceThisFrame = speed / nextTile.MovementCost * deltaTime;
		//convert to percentage between nodes
		float percentageThisFrame = distanceThisFrame / distanceToTravel;

		//Increment our movement progress.
		movementPercentage += percentageThisFrame;

		//Have we arrived at the next node?
		if (movementPercentage >= 1) {
			CurrentTile = nextTile;
			movementPercentage = 0;
		}
	}

	void UpdateJob(float deltaTime) {
		if (currentJob == null) {
			currentJob = JobManager.DequeueJob ();

			if (currentJob != null) {
				DestinationTile = currentJob.Tile;
				currentJob.RegisterJobComplete (OnJobComplete);
				currentJob.RegisterJobStopped (OnJobStopped);
			}
		}

		if (currentJob != null && CurrentTile == currentJob.Tile) {
			currentJob.Update (deltaTime);
		}
	}

	void OnJobComplete(Job job) {
		currentJob = null;
	}

	void OnJobStopped(Job job) {			
		currentJob = null;
	}

	public void SetDestination(Tile tile) {
		if (CurrentTile.IsNeighbour (tile, true) == false) {
			Debug.Log ("Character Movement destination tile is not neighbour. This must not show once A* is in");
		}

		DestinationTile = tile;
	}


}
