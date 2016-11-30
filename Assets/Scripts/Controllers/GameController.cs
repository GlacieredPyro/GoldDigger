using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public TileSpriteController TileSpriteController { get; protected set; }
	public FixtureSpriteController FixtureSpriteController { get; protected set; }
	public CharacterSpriteController CharacterSpriteController { get; protected set; }
	public MaterialSpriteController MaterialSpriteController { get; protected set; }
	public JobSpriteController JobSpriteController { get; protected set; }

	// Use this for initialization
	void Start () {
		//Managers get initialized first!
		WorldController.Instance.Initialize ();
		FixtureManager.Instance.InitializeFixtures ();
		CharacterManager.Instance.InitializeCharacterManager ();
		MaterialManager.Instance.InitializeMaterialManager ();

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

		//Just for testing
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				WorldController.Instance.World.GetTileAt (x, y).UpdateTileType (TileType.Floor);
			}
		}

		Character c = CharacterManager.Instance.CreateCharacter (WorldController.Instance.World.GetTileAt (0, 0));

		Material m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 3), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (3, 4), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 3), m);
		m = new Material (100, 100, 0.5f);
		MaterialManager.Instance.PlaceMaterial (WorldController.Instance.World.GetTileAt (4, 4), m);

		Job j = new Job (WorldController.Instance.World.GetTileAt (3, 3), 1, (job) => {
			//if the character has no material on hand, create one for him.
			if(job.Character.Material == null) {
				//TODO:Max invetory size from somewhere?
				Material mat = new Material(0, 50, 1f);
				job.Character.SetMaterial(mat);
				Debug.Log("Adding new material object to player");

			} else if (job.Character.Material.IsFull()) {//if the character has no more room.
				//end the job.
				job.RequestJobStop();
				Debug.Log("Char inv full");
				return;
			} else if(job.Tile.Material.IsEmpty()) {//theres nothing left to collect
				//end the job
				//TODO start another?
				job.RequestJobStop();
				Debug.Log("Material Empty");
				return;
			}

			//Set the take amount based on something later
			//So diggers can take more at a time.
			//Maybe a character modifier.
			int takenMaterial = job.Tile.Material.TakeMaterial(20, true);
			Debug.LogFormat("Adding {0} material to char", takenMaterial);
			//FIXME some material might go missing here
			int addedMaterial = job.Character.Material.addMaterial(takenMaterial);

		}, "Collect", "Dirt", true);
		//JobManager.EnqueueJob (JobType.MINE, j);
	}
	
	// Update is called once per frame
	void Update () {
		CharacterManager.Instance.Update (Time.deltaTime);
	}
}
