using System;
using UnityEngine;
using System.Collections.Generic;

public class JobSpriteController : MonoBehaviour {

	string ID;
	Dictionary<Job, GameObject> jobGameObjectMap;
	//TODO support mutiple sprites
	Dictionary<string, Sprite> sprites;

	void Awake() {
		jobGameObjectMap = new Dictionary<Job, GameObject> ();
		ID = IDGenerator.CreateUniqueId (typeof(JobSpriteController));
		PreloadSprites ();
	}

	public void OnJobManagerInitialized() {
		JobManager.Instance.RegisterOnJobQueued (OnJobQueued);
	}

	public void OnJobQueued(Job job) {
		//get the game object for this job
		GameObject go = GetGameObjectFor (job);
		Debug.Log ("New job detected. need to make tmp go's");

		//if this job already has one a gameobject
		if (go != null) {
			//job was probaly requeued so we can bomb
			return;
		}

		Sprite sprite = GetSpriteFor (job);
		go = CreateGameObjectFor (job, sprite);
		jobGameObjectMap.Add (job, go);
		//job.RegisterJobComplete (OnJobDone);
		job.RegisterJobStopped (OnJobDone);

	}

	void OnJobDone(Job job) {
		GameObject go = GetGameObjectFor (job);
		if (go != null) {
			jobGameObjectMap.Remove (job);
			Destroy (go);
		}
	}

	Sprite GetSpriteFor(Job job) {
		if (job.JobType == JobType.BUILD) {
			//If you build it's going to be a fixture
			GameController gc = FindObjectOfType<GameController> ();
			return gc.FixtureSpriteController.GetFixtureSprite (job.TargetType);
		};
		if (job.JobType == JobType.MINE) {
			return sprites [JobType.MINE];
		}
		return null;
	}
		
	//TODO Improve this with a Pool
	//FIXME ASAP
	private GameObject CreateGameObjectFor(Job job, Sprite sprite) {
		GameObject job_go = new GameObject ();
		job_go.name = "Job_" + job.JobType + "_" + job.TargetType;
		job_go.transform.position = new Vector3 (job.Tile.X, job.Tile.Y, 0f);
		job_go.transform.SetParent (this.transform, true);
		
		SpriteRenderer sr = job_go.AddComponent<SpriteRenderer> ();
		sr.sprite = sprite;
		sr.sortingLayerName = "Jobs";
		sr.color = new Color (0.5f, 1f, 0.5f, 0.25f);

		return job_go;
	}

	private GameObject GetGameObjectFor(Job m) {
		if (jobGameObjectMap.ContainsKey (m)) {
			return jobGameObjectMap [m];
		}
		return null;
	}

	void PreloadSprites() {
		sprites = new Dictionary<string, Sprite> ();
		Sprite[] loadedSprites = Resources.LoadAll<Sprite> ("Sprites/Jobs");
		foreach (Sprite spr in loadedSprites) {
			sprites.Add (spr.texture.name, spr);
		}
		Debug.Log (ID + ":: finished pre-loading sprites");
	}
}
