using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;

public static class DatabaseHandler
{
    private static string connectionString =
    "Server=127.0.0.1;Port=3306;Database=castillo_qwerty;Uid=unityuser;Pwd=12345;";



    public static bool TestConnection()
    {
        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                Debug.Log("Conexión a MySQL exitosa.");
                conn.Close();
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error de conexión a MySQL: " + ex.Message);
            return false;
        }
    }
}
