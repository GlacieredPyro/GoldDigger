using UnityEngine;
using System.Collections.Generic;
using System;

public class Fixture {

	public Tile Tile { get; protected set; }
	public string FixtureType  { get; protected set; }
	/// <summary>
	/// Gets or sets the movement cost.
	/// If Zero then impassable.
	/// </summary>
	/// <value>The movement cost.</value>
	public float MovementCost  { get; protected set; }
	public bool IsRoomEnclosure  { get; protected set; }
	public bool LinksToNeighbour  { get; protected set; }

	private Action<Fixture> OnFixtureCreated;
	private Action<Fixture> OnFixtureDestroyed;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	public Fixture(string type, float movementCost, bool isRoomEnclosure, bool linksToNeighbour, int width = 1, int height = 1) {
		FixtureType = type;
		MovementCost = movementCost;
		IsRoomEnclosure = isRoomEnclosure;
		LinksToNeighbour = linksToNeighbour;
		Width = width;
		Height = height;
	}

	public bool IsValidPlacementTile(Tile tile) {
		List<Tile> placementTiles = GetTiles (tile);
		bool valid = true;
		foreach (Tile t in placementTiles) {
			valid &= t.Fixture == null && t.TileType !=  TileType.Empty;
		}
		return valid;
	}

	/// <summary>
	/// Gets the tiles based on dimenstions.
	/// </summary>
	/// <returns>The tiles.</returns>
	List<Tile> GetTiles(Tile baseTile) {
		List<Tile> response = new List<Tile> ();
		for (int x = baseTile.X; x < baseTile.X + Width; x++) {
			for (int y = baseTile.Y; y < baseTile.Y + Height; y++) {
				response.Add(WorldController.Instance.World.GetTileAt(x, y));
			}
		}
		return response;
	}

	public bool PlaceAtTile(Tile tile) {
		List<Tile> placementTiles = GetTiles (tile);
		bool allPlaced = true;
		foreach (Tile t in placementTiles) {
			allPlaced &= t.PlaceFixture (this);
		}
		if (allPlaced) {
			this.Tile = tile;
			OnFixtureCreated (this);
			return true;
		}

		//if everything didnt place correctly roll back.
		foreach (Tile t in placementTiles) {
			tile.RemoveFixture (this);
		}
		return false;
	}

	public void RegisterFixtureCreated(Action<Fixture> listener) {
		OnFixtureCreated += listener;
	}

	public void RegisterFixtureDestroyed(Action<Fixture> listener) {
		OnFixtureDestroyed += listener;
	}

	public Fixture Clone() {
		return this.MemberwiseClone () as Fixture;
	}
}
