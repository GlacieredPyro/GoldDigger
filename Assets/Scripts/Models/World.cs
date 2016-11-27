using UnityEngine;
using System.Collections.Generic;

public class World {

	private string ID;

	Tile[,] Tiles;
	public List<Fixture> Fixtures { get; protected set; }
	public List<Material> Items { get; protected set; }
	public List<Character> Characters { get; protected set; }

	public int Width { get; protected set; }
	public int Height  { get; protected set; }

	public Path_TileGraph TileGraph;

	public World(int width, int height) {
		ID = IDGenerator.CreateUniqueId (typeof(World));

		this.Width = width;
		this.Height = height;

		this.Fixtures = new List<Fixture> ();
		this.Items = new List<Material> ();
		this.Characters = new List<Character> ();


		GenerateTiles ();

		Debug.Log (ID + ":: World Created");
	}

	#region Public Functions
	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>. Null if out of bounds</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y) {
		if (x >= Width || x < 0 || y >= Height || y < 0) {
			return null;
		}
		return Tiles [x, y];
	}
	#endregion

	#region Private Helpers
	/// <summary>
	/// Generates the tiles.
	/// Should only run once.
	/// </summary>
	private void GenerateTiles() {
		if (Tiles == null) {
			Tiles = new Tile[Width, Height];
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					Tiles [x, y] = TileManager.Instance.CreateTile (this, x, y, TileType.Empty);
				}
			}
		}
		Debug.Log(ID +":: Tiles generated " + Width + " x " + Height);
	}
	#endregion
}
