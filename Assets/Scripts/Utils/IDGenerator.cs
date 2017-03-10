using UnityEngine;
using System.Collections;
using System;

public class IDGenerator {

	private static int id = 0;

	public static string CreateUniqueId(System.Type t) {
		return t.Name + (++id);
	}

	public static string CreateNew() {
		return Guid.NewGuid ().ToString ();
	}
}
