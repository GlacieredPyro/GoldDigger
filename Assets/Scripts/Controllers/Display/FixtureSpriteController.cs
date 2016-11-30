using UnityEngine;
using System.Collections.Generic;

public class FixtureSpriteController : MonoBehaviour {

	private string ID;
	private string basePath = "Fixtures";
	Dictionary<string , Sprite> sprites;
	Dictionary<Fixture, GameObject> fixtureGameObjects;

	void Awake() {
		ID = IDGenerator.CreateUniqueId (typeof(FixtureSpriteController));
		fixtureGameObjects = new Dictionary<Fixture, GameObject> ();
		PreLoadSprites ();
	}

	public void OnWorldInitialized(World world) {
		FixtureManager.Instance.RegisterForAllFixtureEvents (ID, OnFixtureEvent);
		Debug.Log (ID + ":: Generating Fixture GO's");
		InitFixtureSprites (world);
	}
		
	public void OnFixtureEvent(FixtureEvent eventType, Fixture fixture) {
		if (FixtureEvent.FIXTURE_CREATED == eventType) {
			//fixtureGameObjects [fixture].GetComponent<SpriteRenderer> ().sprite = sprites [newTile.TileType.ToString ()];
			if (fixtureGameObjects.ContainsKey (fixture) == false) {
				fixtureGameObjects.Add (fixture, CreateFixtureGameObject(fixture));
			}
		}
	}

	private GameObject CreateFixtureGameObject(Fixture fixture) {
		GameObject go = new GameObject ();

		go.name = fixture.FixtureType;

		go.transform.position = new Vector2 (fixture.Tile.X, fixture.Tile.Y);
		go.transform.SetParent (this.transform, true);

		SpriteRenderer sr = go.AddComponent<SpriteRenderer> ();
		sr.sprite = GetFixtureSprite(fixture.FixtureType);
		sr.sortingLayerName = "Fixtures";

		return go;
	}

	public Sprite GetFixtureSprite(string type) {
		return sprites [type];
	}

	//World load from file?
	private void InitFixtureSprites(World world) {
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				
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
