using UnityEngine;
using System.Collections.Generic;
using System;

public enum TileEvent {
	TYPE_CHANGED
}

public class TileManager {

	private static TileManager _instance;

	Dictionary<string, Action<TileEvent, Tile,Tile>> listeners;

	public static TileManager Instance { 
		get { 
			if (_instance == null)
				_instance = new TileManager ();

			return _instance;
		} 
		private set {
			_instance = value;
		}}

	private TileManager() {
		listeners = new Dictionary<string, Action<TileEvent, Tile, Tile>> ();
	}

	public Tile CreateTile(World world, int x, int y, TileType t) {
		return new Tile (world, x, y, t, OnAnyTileChanged);
	}

	public Tile CreateTile(World world, S_Tile tileStruct) {
		return new Tile (world, tileStruct.X, tileStruct.Y, (TileType)tileStruct.TileType, OnAnyTileChanged, tileStruct.ID);
	}

	private void OnAnyTileChanged(Tile oldTile, Tile newTile) {
		if (oldTile.TileType != newTile.TileType) {
			foreach (string key in listeners.Keys) {
				listeners [key] (TileEvent.TYPE_CHANGED, oldTile, newTile);
			}
		}
	}

	//FIXME This registers for all events actually
	public void RegisterForTileChanged(string id, Action<TileEvent, Tile, Tile> handler) {
		if (listeners.ContainsKey (id)) {
			Debug.Log (id + " is already registered for tile events.");
			return;
		}
		listeners.Add (id, handler);
	}
		
}
