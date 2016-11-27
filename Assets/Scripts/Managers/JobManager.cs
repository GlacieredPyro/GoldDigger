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

	Queue<Job> jobQueue;
	Action<Job> OnJobQueued;

	private JobManager() {
		jobQueue = new Queue<Job> ();
	}

	public void RegisterOnJobQueued(Action<Job> listener) {
		OnJobQueued += listener;
	}

	public static void EnqueueJob(Job job) {
		Instance.jobQueue.Enqueue (job);
		if (Instance.OnJobQueued != null) {
			Instance.OnJobQueued (job);
		}
	}
		
	public static Job DequeueJob() {
		if(Instance.jobQueue.Count > 0) 
			return Instance.jobQueue.Dequeue ();
		return null;
	}
}


