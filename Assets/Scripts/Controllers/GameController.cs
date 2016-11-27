using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	TileSpriteController tileSpriteController;
	FixtureSpriteController fixtureSpriteController;
	CharacterSpriteController characterSpriteController;
	MaterialSpriteController materialSpriteController;

	// Use this for initialization
	void Start () {
		//Managers get initialized first!
		WorldController.Instance.Initialize ();
		FixtureManager.Instance.InitializeFixtures ();
		CharacterManager.Instance.InitializeCharacterManager ();
		MaterialManager.Instance.InitializeMaterialManager ();

		tileSpriteController = FindObjectOfType<TileSpriteController> ();
		tileSpriteController.OnWorldInitialized (WorldController.Instance.World);
		fixtureSpriteController = FindObjectOfType<FixtureSpriteController> ();
		fixtureSpriteController.OnWorldInitialized (WorldController.Instance.World);
		characterSpriteController = FindObjectOfType<CharacterSpriteController> ();
		characterSpriteController.OnCharacterManagerInitialized ();
		materialSpriteController = FindObjectOfType<MaterialSpriteController> ();
		materialSpriteController.OnMaterialManagerInitialized ();

		//Just for testing
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				WorldController.Instance.World.GetTileAt (x, y).UpdateTileType (TileType.Floor);
			}
		}

		Character c = CharacterManager.Instance.CreateCharacter (WorldController.Instance.World.GetTileAt (0, 0));

		Material m = new Material (100, 100, 0.1f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 3), m);
		m = new Material (100, 100, 0.1f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 4), m);
		m = new Material (100, 100, 0.1f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 3), m);
		m = new Material (100, 100, 0.1f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 4), m);


	}
	
	// Update is called once per frame
	void Update () {
		CharacterManager.Instance.Update (Time.deltaTime);
	}
}
