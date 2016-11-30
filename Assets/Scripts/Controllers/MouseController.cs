using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

	private string ID;
	Vector3 currentFramePosition, lastFramePosition;
	Vector3 dragStartPos, dragEndPos;
	bool dragging = false;
	string buildMode;

	//FIXME DEBUG ONLY
	public Text tileDisplay;

	// Use this for initialization
	void Start () {
		ID = IDGenerator.CreateUniqueId (typeof(MouseController));
		currentFramePosition = lastFramePosition = Vector3.zero;
	}

	// Update is called once per frame
	void Update () {
		currentFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currentFramePosition.z = 0;
		tileDisplay.text = "["+Mathf.FloorToInt(currentFramePosition.x+0.5f)+","+Mathf.FloorToInt(currentFramePosition.y+0.5f)+"]";
		UpdateCamera ();
		UpdateDragging ();

		//Cant use a copy of currentFrame Position because it may have moved
		lastFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void UpdateDragging() {
		//Check that we are not over a UI element, if we are kick out
		if (EventSystem.current.IsPointerOverGameObject ())
			return;

		//no valid build mode
		if (buildMode == null)
			return;

		if (Input.GetMouseButtonDown (0)) {
			dragStartPos = currentFramePosition;
			dragging = true;
		} else if (dragging == false) {
			dragStartPos = currentFramePosition;
		}

		//if the rmb  or escape key kicks, we cancle dragging
		if (Input.GetMouseButtonUp (1) || Input.GetKeyDown (KeyCode.Escape)) {
			dragging = false;
		}

		int start_x = Mathf.FloorToInt (dragStartPos.x + 0.5f);
		int start_y = Mathf.FloorToInt (dragStartPos.y + 0.5f);
		int end_x = Mathf.FloorToInt (currentFramePosition.x + 0.5f);
		int end_y = Mathf.FloorToInt (currentFramePosition.y + 0.5f);

		//flip for inverse dragging
		if (end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}
		if (end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		//TODO:show drag indicators

		//End drag
		if (dragging && Input.GetMouseButtonUp (0)) {
			Debug.Log (ID + ":: DragEnd [" + start_x + "," + start_y + "] [" + end_x  + "," + end_y + "]");
			dragging = false;

			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					//FIXME Temp testing code
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if(buildMode == "Floor" && t != null)
						t.UpdateTileType(TileType.Floor);
					if (buildMode == "Wall" && t != null) {
						//FixtureManager.Instance.PlaceFixture ("Wall", t);
						Job j = new Job (t, 1f, (job) => {
							FixtureManager.Instance.PlaceFixture ("Wall", job.Tile);
						}, JobType.BUILD, "Wall");
						JobManager.EnqueueJob (j);
					}
					if (buildMode == "Mine" && t != null) {
						if (t.Material != null) {
							Job j = new Job (t, 1, (job) => {
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
		}

	}

	void UpdateCamera() {
		if(Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
			Vector3 diff = lastFramePosition - currentFramePosition;
			Camera.main.transform.Translate (diff);
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis ("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 3f, 25f);
	}

	public void SetBuildMode(string mode) {
		buildMode = mode;
		Debug.Log (ID + ":: BuildMode -> " + mode);
	}
}
