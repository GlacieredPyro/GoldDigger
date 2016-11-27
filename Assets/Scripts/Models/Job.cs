using System;
using UnityEngine;
using System.Collections.Generic;

public class Job {

	public Tile Tile { get; protected set; }

	public float JobTimeRequired { get; protected set; }
	public float JobTimePassed { get; protected set; }

	public bool JobRepeats { get; protected set; }

	public bool Adjacant { get; protected set; }

	List<string> jobWorkedLua;
	List<string> jobCompletedLua;

	public Character Character { get; protected set; }

	Action<Job> OnJobCompleted;
	Action<Job> OnJobStopped;
	Action<Job> OnJobWorked;

	public Job(Tile t, float jobTimeRequired, Action<Job> jobCompleteListener) {
		JobRepeats = false;
		Adjacant = false;
		JobTimeRequired = jobTimeRequired;
		OnJobCompleted = jobCompleteListener;
		Tile = t;
	}

	public void RegisterJobComplete(Action<Job> listener) {
		OnJobCompleted += listener;
	}

	public void RegisterJobStopped(Action<Job> listener) {
		OnJobStopped += listener;
	}

	public void Update(float deltaTime) {
		if (JobTimePassed < JobTimeRequired) {
			JobTimePassed += deltaTime;
			if (OnJobWorked != null) {
				OnJobWorked (this);
			}
		} else {
			if (OnJobCompleted != null) {
				OnJobCompleted (this);
				Debug.Log ("Job complete");
			}
			if (JobRepeats) {
				JobTimePassed = 0f;
			} else {
				OnJobStopped (this);
				Debug.Log ("Job Stopped");
				TryDestroyMe ();
			}
		}
	}

	void TryDestroyMe() {
		Debug.Log ("Destroying Job");
		OnJobCompleted = null;
		OnJobStopped = null;
		OnJobWorked = null;
		Character = null;
		Tile = null;
	}

}
