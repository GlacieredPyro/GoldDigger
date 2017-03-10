using UnityEngine;
using System.Collections;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class SaveManager {

	private static string ConnectionLocation = "URI=file:" + Application.dataPath + "/SaveData/SaveState.s3db";
	public static IDbConnection connection { get; private set; }

	public static void OpenConnection() {
		connection = (IDbConnection) new SqliteConnection (ConnectionLocation);
		connection.Open ();
	}

	public static void CloseConnection() {
		connection.Close ();
	}
		
	/// <summary>
	/// Executes the non query. UPDATE, DELETE ETC</summary>
	/// </summary>
	/// <returns>The non query.</returns>
	/// <param name="sql">Sql.</param>
	public static int ExecuteNonQuery(String sql) {
		IDbCommand dbcmd = connection.CreateCommand ();
		dbcmd.CommandText = sql;
		int result = dbcmd.ExecuteNonQuery ();
		dbcmd.Dispose ();
		return result;
	}

	/// <summary>
	/// Executes the SELECT query.
	/// </summary>
	/// <param name="sql">Sql.</param>
	/// <param name="OnExecuted">On executed. Action that receives the raw db reader to use.</param>
	public static void ExecuteQuery(string sql, Action<IDataReader> OnExecuted) {
		IDbCommand dbcmd = connection.CreateCommand ();
		string sqlQuery = sql;
		dbcmd.CommandText = sqlQuery;

		IDataReader reader = dbcmd.ExecuteReader ();

		OnExecuted (reader);

		reader.Close ();
		dbcmd.Dispose ();
	}

	private SaveManager() {
	}
}
