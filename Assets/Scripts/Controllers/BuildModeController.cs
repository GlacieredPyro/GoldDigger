using UnityEngine;
using System.Collections;

class BuildModeActionType {
	public const string BUILD = "Build";
	public const string PLACE = "Place";
	public const string MINE = "Mine";
}

public class BuildModeController : MonoBehaviour {

	MouseController mouseController;

	bool buildModeIsFixture = false;
	string buildModeObjectType;
	TileType buildModeTileType = TileType.Floor;
	string buildModeActionType;

	// Use this for initialization
	void Start () {
		mouseController = FindObjectOfType<MouseController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsObjectDraggable() {
		return buildModeIsFixture == false;
	}

	public void SetMode_Floor() {
		buildModeIsFixture = false;
		buildModeTileType = TileType.Floor;
		buildModeActionType = BuildModeActionType.PLACE;
	}

	public void SetMode_Bulldoze() {
		buildModeIsFixture = false;
		buildModeTileType = TileType.Empty;
		buildModeActionType = BuildModeActionType.BUILD;
	}

	public void SetMode_BuildFixture(string fixtureType) {
		buildModeIsFixture = true;
		buildModeObjectType = fixtureType;
		buildModeActionType = BuildModeActionType.BUILD;
	}

	public void SetMode_Action(string actionType) {
		buildModeIsFixture = false;
		buildModeTileType = TileType.Empty;
		buildModeActionType = actionType;
	}

	public void BuildAt(Tile tile) {
		if (buildModeIsFixture && buildModeActionType == BuildModeActionType.BUILD) {
			string fixtureType = buildModeObjectType.Clone ().ToString();
			//build a fixture
			if (JobManager.HasPendingJob (tile) == false && FixtureManager.IsValidPlacementFor (tile, buildModeObjectType)) {
				Job j = new Job (tile, 1f, (job) => {
					FixtureManager.Instance.PlaceFixture (fixtureType, job.Tile);
				}, JobType.BUILD, fixtureType);
				JobManager.EnqueueJob (j);
			}
		} else if (buildModeTileType == null && buildModeActionType == BuildModeActionType.BUILD) {
			//bulldoze a fixture
		} else if (buildModeActionType == BuildModeActionType.PLACE) {
			tile.UpdateTileType (buildModeTileType);
		} else if (buildModeActionType == BuildModeActionType.MINE) {
			if (tile.Material != null) {
				Job j = new Job (tile, 1, (job) => {
					//if the character has no material on hand, create one for him.
					if(job.Character.Material == null) {
						//TODO:Max invetory size from somewhere?
						Material mat = new Material(0, 1000, 1f);
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

				}, JobType.MINE, "Dirt", true);
				JobManager.EnqueueJob (j);
			}
		}
	}
}
