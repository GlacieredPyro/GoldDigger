using System;
using System.Collections.Generic;
using UnityEngine;

public class JobManager
{
	private static JobManager _instance;
	private string ID;
	public static JobManager Instance {
		get{ 
			if (_instance == null) {
				JobManager i = new JobManager ();
				i.ID = IDGenerator.CreateUniqueId (typeof(JobManager));
				_instance = i;
			}
			return _instance;
		} 
		protected set { 
			_instance = value;
		}
	}
		
	Dictionary<string, Queue<Job>> queues;
	Dictionary<Tile, Job> pendingJobs;
	Action<Job> OnJobQueued;

	private JobManager() {
		queues = new Dictionary<string, Queue<Job>> ();
		//seems dodge having a second ref to each job.
		//seems better than have tiles needing to know about pending jobs and all the job rules.
		pendingJobs = new Dictionary<Tile, Job> ();
	}

	public static int AvailableJobCount(string jobType) {
		if(Instance.queues.ContainsKey(jobType))
			return Instance.queues[jobType].Count;
		return 0;
	}

	public void RegisterOnJobQueued(Action<Job> listener) {
		OnJobQueued += listener;
	}

	public static void EnqueueJob(Job job) {
		if (Instance.queues.ContainsKey (job.JobType) == false) {
			Instance.queues.Add(job.JobType, new Queue<Job>());
		}
		Instance.queues[job.JobType].Enqueue (job);
		if (Instance.OnJobQueued != null) {
			Instance.OnJobQueued (job);
			Instance.AddPendingJob (job);
		}
	}

	private void AddPendingJob(Job job) {
		if (pendingJobs.ContainsKey (job.Tile)) {
			Debug.LogError (ID + ":: Job already pending at specified tile");
		}
		pendingJobs.Add (job.Tile, job);
		job.RegisterJobStopped (PendingJobStopped);
	}

	private void PendingJobStopped(Job job) {
		if (pendingJobs.ContainsKey (job.Tile)) {
			pendingJobs.Remove (job.Tile);
		} else {
			Debug.LogError (ID + ":: Couldn't find pending job to remove");
		}
	}

	public static bool HasPendingJob(Tile tile) {
		return Instance.pendingJobs.ContainsKey (tile);
	}
		
	public static Job DequeueJob(string jobType) {
		if(Instance.queues.ContainsKey(jobType) && Instance.queues[jobType].Count > 0) 
			return Instance.queues[jobType].Dequeue ();
		return null;
	}
}


