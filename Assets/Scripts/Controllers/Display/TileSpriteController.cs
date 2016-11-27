using UnityEngine;
using System.Collections.Generic;

public class TileSpriteController : MonoBehaviour {

	private string ID;
	private string basePath = "Tiles";
	Dictionary<string , Sprite> sprites;
	Dictionary<Tile, GameObject> tileGameObjects;

	void Awake() {
		ID = IDGenerator.CreateUniqueId (typeof(TileSpriteController));
		tileGameObjects = new Dictionary<Tile, GameObject> ();
		PreLoadSprites ();
	}

	public void OnWorldInitialized(World world) {
		TileManager.Instance.RegisterForTileChanged (ID, OnTileChanged);
		Debug.Log (ID + ":: Generating tile GO's");
		InitTileSprites (world);
	}
		
	public void OnTileChanged(TileEvent eventType, Tile oldTile, Tile newTile) {
		if (TileEvent.TYPE_CHANGED == eventType) {
			tileGameObjects [newTile].GetComponent<SpriteRenderer> ().sprite = sprites [newTile.TileType.ToString ()];
		}
	}

	private void InitTileSprites(World world) {
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				Tile t = world.GetTileAt (x, y);

				GameObject go = new GameObject ("Tile_" + x + "_" + y);
				go.transform.position = new Vector2 (x, y);
				go.transform.SetParent (this.transform, true);

				SpriteRenderer sr = go.AddComponent<SpriteRenderer> ();
				sr.sprite = sprites[t.TileType.ToString()];
				sr.sortingLayerName = "Tiles";

				tileGameObjects.Add (t, go);
			}
		}
	}

	private void PreLoadSprites() {
		sprites = new Dictionary<string, Sprite> ();
		Sprite[] loadedSprites = Resources.LoadAll<Sprite> ("Sprites/" + basePath);
		foreach (Sprite spr in loadedSprites) {
			sprites.Add (spr.texture.name, spr);
		}
		Debug.Log (ID + ":: finished pre-loading sprites");
	}
}
