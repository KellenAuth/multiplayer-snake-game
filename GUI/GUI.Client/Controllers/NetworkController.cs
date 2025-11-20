// <copyright file="NetworkController.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified December 2024 </Date>
using CS3500.Networking;
using GUI.Client.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace GUI.Client.Controllers
{
    /// <summary>
    /// Network ontrolcler class responsible for parsing information from the serer and updating the world model
    /// Also sends moe ommvands cto the server
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        /// the login info for the SQL database
        /// </summary>
        public static string connectionString = 
        Environment.GetEnvironmentVariable("SNAKE_DB_CONNECTION") 
        ?? "server=localhost;database=snake_game;uid=root;password=";
        /// <summary>
        /// Object representing the connection to the server
        /// </summary>
        public NetworkConnection? connection;
        /// <summary>
        /// world object having dictionaries and a set representing game objects
        /// </summary>
        public World world;
        /// <summary>
        /// integer representing worldsize
        /// </summary>
        public int worldSize;
        /// <summary>
        /// integer representing PlayerID
        /// </summary>
        public int playerID;

        /// <summary>
        /// NetworkController constructor taking in a world object
        /// </summary>
        /// <param name="world"></param>
        public NetworkController(World world)
        {
            this.world = world; 
            //create new connection
            connection = new NetworkConnection();
        }

        /// <summary>
        /// Disconnects client from server
        /// </summary>
        public void Disconnect()
        {
            DateTime datetime = DateTime.Now;
            DisconnectedGameUpdate(datetime);
            world.UpdateAllSnakeDisconnects(datetime);
            connection!.Disconnect();
        }
     
        /// <summary>
        /// Adds a new game to the SQL database
        /// </summary>
        private void SQLAddGame()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    DateTime datetime = DateTime.Now;
                    command.CommandText = command.CommandText = "INSERT INTO `Games` (`StartTime`) VALUES('" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";
                    command.ExecuteNonQuery();


                    // Get the games ID
                    using (MySqlCommand lastIdCommand = new MySqlCommand("Select LAST_INSERT_ID()", conn))
                    {
                        world.GameID = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Update the current games leave time to the disconnect time
        /// </summary>
        /// <param name="datetime"> the date the game was disconnected from </param>
        private void DisconnectedGameUpdate(DateTime datetime)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE Games SET EndTime = '" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' WHERE ID = " + world.GameID;
                    command.ExecuteNonQuery();



                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        /// <summary>
        /// Connect method to start the connection loop that starts constant communication with the server
        /// </summary>
        /// <param name="hostName"> string representing the hostName of the server</param>
        /// <param name="port"> intger representing the port number of the server</param>
        /// <param name="playerName"> string representing the playername of the player connecting to server</param>
        public void Connect(string hostName, int port, string playerName)
        {
            try
            {
               
                //connect to server
                connection.Connect(hostName, port);
                //send sever player name
                connection.Send(playerName);
                SQLAddGame();

                // Start background processing of server messages
                Task.Run(ProcessServerMessages);
            }
            catch (Exception)
            {
                connection?.Disconnect();
                throw;
            }


        }

        /// <summary>
        /// Private method to read the first information from the server then start the communication loop
        /// </summary>
        private void ProcessServerMessages()
        {
            try
            {
                // Read initial setup data
                playerID = int.Parse(connection!.ReadLine());
                worldSize = int.Parse(connection.ReadLine());


                world.PlayerId = playerID;
                // Process continuous server updates
                while (true)
                {
                    string? line = connection.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    ProcessJsonMessage(line);
                }
            }
            catch(Exception)
            {
                
            }
        }
        /// <summary>
        /// private method to deserialize messages from the server and update the world object 
        /// </summary>
        /// <param name="json"></param>
        private void ProcessJsonMessage(string json)
        {
            try
            {
                if (json.Contains("wall"))
                {
                    world.AddWall(JsonSerializer.Deserialize<Walls>(json));
                }
                if (json.Contains("snake"))
                {
                    world.UpdateSnake(JsonSerializer.Deserialize<Snake>(json));   
                }
                if (json.Contains("power"))
                {
                    world.UpdatePowerup(JsonSerializer.Deserialize<Powerups>(json));
                }
            }
            catch (JsonException)
            {
                
            }
        }

        /// <summary>
        /// method to send the up command to server
        /// </summary>
        public void MoveUp()
        {
            if (!(connection == null))
            {
                try
                {
                    connection.Send("{\"moving\":\"up\"}");
                }
                catch (Exception)
                {

                   
                }
               
            }
        }
        /// <summary>
        /// method to send the down command to the server
        /// </summary>
        public void MoveDown()
        {
            if (!(connection == null))
            {
                try
                {
                    connection.Send("{\"moving\":\"down\"}");
                }
                catch (Exception)
                {

                
                }
               
            }
        }
        /// <summary>
        /// method to send the left command to the server
        /// </summary>
        public void MoveLeft()
        {
            if (!(connection == null))
            {
                try
                {
                    connection.Send("{\"moving\":\"left\"}");
                }
                catch (Exception)
                {

                }
                
            }
        }
        /// <summary>
        /// method to send the right command to the server
        /// </summary>
        public void MoveRight()
        {
            if (!(connection == null))
            {
                try
                {
                    connection.Send("{\"moving\":\"right\"}");
                }
                catch (Exception)
                {

                 
                }
                
            }
        }
    }
}
