using UnityEngine;
using System.Collections;
using System;

//FIXME:: User string types instead
public enum TileType {Empty, Floor, Bedrock};

public enum Enterability{ YES, NEVER, SOON }

public class Tile {

	public int X { get; protected set; }
	public int Y { get; protected set; }
	//TODO FIXME This should be loosely linked to tile type.
	public float MovementCost { get { 
			if (TileType.Empty == this.TileType)
				return 0;
			if (Fixture != null) {
				return Fixture.MovementCost;
			}
			return 1;
		}
	}

	public TileType TileType { get; protected set; }
	public Material Material { get; protected set; }
	public Fixture Fixture { get; protected set; }

	public World World { get; protected set; }

	Action<Tile, Tile> cbTileChanged;

	public Tile(World world, int x, int y, TileType t,  Action<Tile, Tile> cbTileChanged) {
		this.World = world;
		this.X = x;
		this.Y = y;
		this.TileType = t;
		this.cbTileChanged += cbTileChanged;
	}

	public void UpdateTileType(TileType type) {
		Tile t = this.MemberwiseClone () as Tile;
		this.TileType = type;
		cbTileChanged (t, this);
	}

	public bool PlaceFixture(Fixture f) {
		//Tile must be unused
		if (this.Fixture != null || this.Material != null)
			return false;

		this.Fixture = f;
		return true;
	}

	public void RemoveFixture(Fixture f) {
		if(this.Fixture == f)
			this.Fixture = null;
	}

	public bool PlaceMaterial(Material material) {
		if (this.Material != null) {
			Debug.LogError ("Material already set at [" + X + "," + Y + "]");
			return false;
		}

		this.Material = material;
		//FIXME We only support dirt currently
		UpdateTileType(TileType.Bedrock);
		return true;
	}

	public Enterability IsEnterable() {
		if (MovementCost == 0) {
			return Enterability.NEVER;
		}

		if(Fixture != null) {
			//check if the fixture can be entered
		}

		return Enterability.YES;
	}

	#region Helpers

	/// <summary>
	/// Determines whether this instance is neighbour of the specified tile.
	/// </summary>
	/// <returns><c>true</c> if this instance is neighbour of the specified tile; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile the neighbour</param>
	/// <param name="diagOkay">If set to <c>true</c> diagonals will be allowed okay.</param>
	public bool IsNeighbour(Tile tile, bool diagOkay = false) {
		return 
			Mathf.Abs( this.X - tile.X ) + Mathf.Abs( this.Y - tile.Y ) == 1 ||  // Check hori/vert adjacency
			( diagOkay && ( Mathf.Abs( this.X - tile.X ) == 1 && Mathf.Abs( this.Y - tile.Y ) == 1 ) ) // Check diag adjacency
			;
	}

	/// <summary>
	/// Gets the neighbours.
	/// </summary>
	/// <returns>The neighbours.</returns>
	/// <param name="diagOkay">Is diagonal movement okay?.</param>
	public Tile[] GetNeighbours(bool diagOkay = false) {
		Tile[] ns;

		if(diagOkay == false) {
			ns = new Tile[4];	// Tile order: N E S W
		}
		else {
			ns = new Tile[8];	// Tile order : N E S W NE SE SW NW
		}

		Tile n;

		n = World.GetTileAt(X, Y+1);
		ns[0] = n;	// Could be null, but that's okay.
		n = World.GetTileAt(X+1, Y);
		ns[1] = n;	// Could be null, but that's okay.
		n = World.GetTileAt(X, Y-1);
		ns[2] = n;	// Could be null, but that's okay.
		n = World.GetTileAt(X-1, Y);
		ns[3] = n;	// Could be null, but that's okay.

		if(diagOkay == true) {
			n = World.GetTileAt(X+1, Y+1);
			ns[4] = n;	// Could be null, but that's okay.
			n = World.GetTileAt(X+1, Y-1);
			ns[5] = n;	// Could be null, but that's okay.
			n = World.GetTileAt(X-1, Y-1);
			ns[6] = n;	// Could be null, but that's okay.
			n = World.GetTileAt(X-1, Y+1);
			ns[7] = n;	// Could be null, but that's okay.
		}

		return ns;
	}
	#endregion
}
