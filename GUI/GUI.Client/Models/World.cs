// <copyright file="World.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified December 2024 </Date>
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace GUI.Client.Models
{
    /// <summary>
    /// Class representing the world with dictionaries and a set to represent groups of game objects
    /// </summary>
    public class World
    {

        /// <summary>
        /// String representing the login to the SQL database
        /// </summary>
        public static string connectionString = 
        Environment.GetEnvironmentVariable("SNAKE_DB_CONNECTION") 
        ?? "server=localhost;database=snake_game;uid=root;password=";

        /// <summary>
        /// int representing the games ID
        /// </summary>
        public int GameID = 0;

        /// <summary>
        /// Dictionary object of snakes where key is their id number 
        /// </summary>
        public Dictionary<int, Snake> snakes = new();

        /// <summary>
        /// Dictionary object of dead snakes where key is their id number
        /// </summary>
        public Dictionary<int, Snake> deadSnakes = new();

        /// <summary>
        /// Hashset object representing all the walls
        /// </summary>
        public HashSet<Walls> walls = new();

        /// <summary>
        /// Dictionary object of powerups where their key is powerup id number
        /// </summary>
        public Dictionary<int, Powerups> powerups = new();

        /// <summary>
        /// public get-setter for the width of the world
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// public get-setter for the height of the world 
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// public get-setter for an int representing Player Id
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// public snake object representing the snake of the player
        /// </summary>
        public Snake player;

        /// <summary>
        /// intializer class for setting the width and height of the world
        /// also sets the Player id
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="worldSize"></param>
        public void Initialize(int playerId, int worldSize)
        {
            PlayerId = playerId;
            Width = worldSize;
            Height = worldSize;
        }

        /// <summary>
        /// Updates all of the snake leave times when client disconnects from server to be that time
        /// </summary>
        /// <param name="datetime"> the time the disconnect occured</param>
        public void UpdateAllSnakeDisconnects(DateTime datetime)
        {
            lock (snakes)
            {
                foreach (Snake snake in snakes.Values)
                {
                    DisconnectedSnakeUpdate(snake, datetime);
                }
            }
            lock (deadSnakes) {
                foreach (Snake snake in deadSnakes.Values)
                {
                    DisconnectedSnakeUpdate(snake, datetime);
                }
            }

        }

        /// <summary>
        /// Updates the sql database with the snakes leavetime when they disconnect from server
        /// </summary>
        /// <param name="snake"> the snake that left </param>
        /// <param name="datetime"> The time they left</param>
        private void DisconnectedSnakeUpdate(Snake snake, DateTime datetime)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE Players SET LeaveTime = '" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' WHERE playerID = " + snake.snake + " AND gameID = " + GameID;
                    command.ExecuteNonQuery();

                    

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Adds a new player to the SQL database on connection
        /// </summary>
        /// <param name="snake"> The players snake </param>
        private void SQLAddSnake(Snake snake)
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
                    command.CommandText = "INSERT INTO `Players` (`GameID`, `PlayerID`, `Name`, `MaxScore`, `EnterTime`) VALUES('" + GameID + "', '" + snake.snake + "', '" + snake.name + "', '" + snake.maxScore + "', '" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";
                    command.ExecuteNonQuery();



                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Updates the snakes max score in the SQL database
        /// </summary>
        /// <param name="snake"> the snake to update the max score of </param>
        private void MaxScoreSnakeUpdate(Snake snake)
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
                    command.CommandText = "UPDATE Players SET MaxScore = " + snake.maxScore + " WHERE playerID = " + snake.snake;
                    command.ExecuteNonQuery();



                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// public method for changing snake objects in the snake dictionary 
        /// </summary>
        /// <param name="snake"> the snake object being updated</param>
        public void UpdateSnake(Snake snake)
        {
            lock (snakes)
            {   //if snake isnt in either dict add it to the SQL
                if((!(snakes.ContainsKey(snake.snake))) && (!(deadSnakes.ContainsKey(snake.snake))))
                {
                    SQLAddSnake(snake);
                }
                //if snake disconnected
                if (snake.dc == true) {
                    //make sure it is removed from both dictionaries
                    lock (deadSnakes)
                    {
                        if (deadSnakes.ContainsKey(snake.snake))
                        {
                            //remove snake from dictionary
                            deadSnakes.Remove(snake.snake);
                        }
                        else if (snakes.ContainsKey(snake.snake))
                        {
                            snakes.Remove(snake.snake);
                        }
                    }
                    DisconnectedSnakeUpdate(snake, DateTime.Now);

                }
                //if snake is not alive
                else if (snake.alive == false)
                {
                    //remove snake from ddictionary
                    snakes.Remove(snake.snake);
                    lock (deadSnakes)
                    {
                        //if snake id is already in deadsnakes update it 
                        if (deadSnakes.ContainsKey(snake.snake))
                        {
                            deadSnakes[snake.snake] = snake;
                        }
                        //else add it to the dictionary 
                        else
                        {
                            deadSnakes.Add(snake.snake, snake);
                        }
                    }
                }
                //if snake is already in dictionary update it 
                else if (snakes.ContainsKey(snake.snake))
                {
                    snakes[snake.snake] = snake;
                    if (snakes[snake.snake].maxScore < snake.score) { 
                        snakes[snake.snake].maxScore = snake.score;
                        MaxScoreSnakeUpdate(snake);
                    }
                }
                //if snake is not already in the dictionary add it and remove it from deadsnakes
                else if (!(snakes.ContainsKey(snake.snake)))
                {
                    deadSnakes.Remove(snake.snake);
                    snakes.Add(snake.snake, snake);
                }
                //if snake is = to player id set the snake object to the player object
                if (snake.snake == PlayerId)
                {
                    player = snake;
                }
                
            }
        }

        /// <summary>
        /// method to add wall objects to the hashset
        /// </summary>
        /// <param name="wall"></param>
        public void AddWall(Walls wall)
        {
            lock (walls)
            {
                walls.Add(wall);
            }
        }

        /// <summary>
        /// public method to update powerup objects in the powerups dictionary
        /// </summary>
        /// <param name="powerup"></param>
        public void UpdatePowerup(Powerups powerup)
        {
            lock (powerups)
            {
                //if powerup is dead remove it from list
                if (powerup.died)
                {
                    powerups.Remove(powerup.power);
                    return;
                }
                //if powerup is in the dictioanry, update it
                else if(powerups.ContainsKey(powerup.power))
                {
                    powerups[powerup.power] = powerup;
                }
                //else add the new powerup to the dictionary 
                else
                {
                    powerups.Add(powerup.power, powerup);
                }
            }
        }
    }
}
