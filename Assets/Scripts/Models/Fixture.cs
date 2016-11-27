using UnityEngine;
using System.Collections;
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

	public Fixture(string type, float movementCost, bool isRoomEnclosure, bool linksToNeighbour) {
		FixtureType = type;
		MovementCost = movementCost;
		IsRoomEnclosure = isRoomEnclosure;
		LinksToNeighbour = linksToNeighbour;
	}

	public bool IsValidPlacementTile(Tile tile) {
		return tile.Fixture == null && tile.TileType !=  TileType.Empty;
	}

	public bool PlaceAtTile(Tile tile) {
		if (tile.PlaceFixture (this)) {
			this.Tile = tile;
			OnFixtureCreated (this);
			return true;
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
