using UnityEngine;
using System.Collections;

public class IDGenerator {

	private static int id = 0;

	public static string CreateUniqueId(System.Type t) {
		return t.Name + (++id);
	}
}
