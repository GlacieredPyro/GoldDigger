using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class GameController : MonoBehaviour {

	public TileSpriteController TileSpriteController { get; protected set; }
	public FixtureSpriteController FixtureSpriteController { get; protected set; }
	public CharacterSpriteController CharacterSpriteController { get; protected set; }
	public MaterialSpriteController MaterialSpriteController { get; protected set; }
	public JobSpriteController JobSpriteController { get; protected set; }

	// Use this for initialization
	void Start () {

		//Data loads
		SaveManager.OpenConnection();
		S_World worldStruct = World.LoadFromDB("a063906a-8cd7-4b74-ae24-d9c6b6d548f1");
		SaveManager.CloseConnection ();
		//End Data Loads

		//Managers get initialized first!
		WorldController.Instance.Initialize (worldStruct.Width, worldStruct.Height, worldStruct.ID);
		FixtureManager.Instance.InitializeFixtures ();
		CharacterManager.Instance.InitializeCharacterManager ();
		MaterialManager.Instance.InitializeMaterialManager ();

		SaveManager.OpenConnection ();
		Debug.Log ("Loading tiles");
		List<S_Tile> tiles = Tile.LoadAllFromDB (worldStruct.ID);
		Debug.Log ("Finished loading tiles");
		SaveManager.CloseConnection ();
		
		Debug.Log ("Placing Tiles");
		WorldController.Instance.World.InitializeTiles (tiles);
		Debug.Log ("Done placing Tiles");

		TileSpriteController = FindObjectOfType<TileSpriteController> ();
		TileSpriteController.OnWorldInitialized (WorldController.Instance.World);

		FixtureSpriteController = FindObjectOfType<FixtureSpriteController> ();
		FixtureSpriteController.OnWorldInitialized (WorldController.Instance.World);

		CharacterSpriteController = FindObjectOfType<CharacterSpriteController> ();
		CharacterSpriteController.OnCharacterManagerInitialized ();

		MaterialSpriteController = FindObjectOfType<MaterialSpriteController> ();
		MaterialSpriteController.OnMaterialManagerInitialized ();

		JobSpriteController = FindObjectOfType<JobSpriteController> ();
		JobSpriteController.OnJobManagerInitialized ();

		Character c = CharacterManager.Instance.CreateCharacter (WorldController.Instance.World.GetTileAt (0, 0));

		Material m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 3), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 4), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 3), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 4), m);

		//Save ();

	}

	private void Save() {
		SaveManager.OpenConnection ();
		//world first.
		WorldController.Instance.World.Save();
		
		SaveManager.CloseConnection ();

	}
	
	// Update is called once per frame
	void Update () {
		CharacterManager.Instance.Update (Time.deltaTime);
	}
}
