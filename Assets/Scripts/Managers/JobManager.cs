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
	Action<Job> OnJobQueued;

	private JobManager() {
		queues = new Dictionary<string, Queue<Job>> ();
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
		}
	}
		
	public static Job DequeueJob(string jobType) {
		if(Instance.queues.ContainsKey(jobType) && Instance.queues[jobType].Count > 0) 
			return Instance.queues[jobType].Dequeue ();
		return null;
	}
}


