using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

//World DB representation
public struct S_World
{
	public string ID;
	public int Width;
	public int Height;
}

public class World {

	public string ID { get; private set; }

	Tile[,] Tiles;
	public List<Fixture> Fixtures { get; protected set; }
	public List<Material> Items { get; protected set; }
	public List<Character> Characters { get; protected set; }

	public int Width { get; protected set; }
	public int Height  { get; protected set; }

	public Path_TileGraph TileGraph;

	public World(int width, int height, string id = null) {
		ID = id == null ? IDGenerator.CreateNew () : id;

		this.Width = width;
		this.Height = height;

		this.Fixtures = new List<Fixture> ();
		this.Items = new List<Material> ();
		this.Characters = new List<Character> ();

		Tiles = new Tile[width, height];
		if(id == null) 
			GenerateTiles ();

		Debug.Log (ID + ":: World Created");
	}

	public void InitializeTiles(List<S_Tile> tiles) {
		foreach (var tile in tiles) {
			Tiles[tile.X, tile.Y] = TileManager.Instance.CreateTile (WorldController.Instance.World, tile);
		}
	}

	public void InvalidateTileGraph() {
		TileGraph = null;
	}
		
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
		

	// ////////////////// //
	//		Save/Load     //
	// ////////////////// //

	public void Save() {
		UpdateOrInsertWorld ();
		SaveTiles ();
	}

	private void SaveTiles() {
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				Tiles [x, y].Save ();
			}
		}
	}

	private void UpdateOrInsertWorld() {
		string sql = @"UPDATE World set Width = {0}, Height = {1} WHERE ID = '{2}';
		INSERT INTO World (ID, Width, Height) 
		SELECT '{2}', {0}, {1}
		WHERE (Select Changes() = 0); --This checks the update results
 		";
		sql = string.Format (sql, this.Width, this.Height, this.ID);
		
		SaveManager.ExecuteNonQuery (sql);
	}

	public static S_World LoadFromDB(string worldId) {
		//Load from DB
		string sqlQuery = "SELECT ID, Width, Height from World where ID = '" + worldId + "'";

		S_World result = new S_World();
		result.ID = null;
		SaveManager.ExecuteQuery (sqlQuery, (reader) => {
			while (reader.Read ()) {
				result.ID = reader.GetString (0);
				result.Width = reader.GetInt32 (1);
				result.Height = reader.GetInt32 (2);
			}
		});
		return result;
	}

}
