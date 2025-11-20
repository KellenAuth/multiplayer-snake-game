// <copyright file="WebServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified December 2024 </Date>


using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;
using CS3500.Networking;
using GUI.Client.Models;
using System.Text;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Security.Policy;

namespace GUI.Client.Controllers
{
    public static class WebServer
    {
        /// <summary>
        /// the header to send before each http code
        /// </summary>
        private const string httpOkHeader =
        "HTTP / 1.1 200 OK\r\n" +
        "Connection: close\r\n" +
        "Content-Type: text/html; charset=UTF-8\r\n" +
        "\r\n";

        /// <summary>
        /// The login for the SQL database
        /// </summary>
        public static string connectionString = 
        Environment.GetEnvironmentVariable("SNAKE_DB_CONNECTION") 
        ?? "server=localhost;database=snake_game;uid=root;password=";

        /// <summary>
        /// initializes the loop of the server
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Server.StartServer(HandleHttpConnection, 80);
            Console.ReadLine();
        }

        /// <summary>
        /// Handles each Http request
        /// </summary>
        /// <param name="connection"> The connection to the website </param>
        private static void HandleHttpConnection(NetworkConnection connection)
        {
            try
            {
                string request = connection.ReadLine();
                //home page
                Console.WriteLine(request);
                if (request.Contains("GET / "))
                {

                    connection.Send(httpOkHeader +
                    "<html>" +
                    "<h3> Welcome to the Snake Games Database! </h3>" +
                    "<a href = \"/games\"> View Games </a>" +
                    "</html>");


                }
                //this page will show a table of all the games in the database. Each row in the table
                //should have a link to the page for that specific game
                else if (request.Contains("GET /games "))
                {
                    StringBuilder html = new StringBuilder();
                    html.Append("<html><table border=\"1\"><thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>");
                    try
                    {
                        using (var conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            using var cmd = new MySqlCommand("SELECT ID, StartTime, EndTime FROM Games ORDER BY ID DESC", conn);
                            using var reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                html.Append($"<tr><td><a href=\"/games?gid={reader["ID"]}\">{reader["ID"]}</a></td>");
                                html.Append($"<td>{reader["StartTime"]}</td>");
                                html.Append($"<td>{reader["EndTime"]}</td></tr>");
                            }
                        }

                        html.Append("</tbody></table></html>");
                        connection.Send(httpOkHeader + html.ToString());
                    }
                    catch (Exception)
                    {

                    }

                }
                //this page shows the stats for the specific game with the given game ID.
                else if (request.Contains("GET /games?gid"))
                {
                    //get the Gid using a regex pattern on the request
                    string pattern = @"gid=([0-9]+)";

                    Match match = Regex.Match(request, pattern);
                    string gidString = match.Groups[1].Value;

                    int gameID = int.Parse(gidString);

                    //create the table for the game
                    StringBuilder html = new StringBuilder();
                    html.Append("<html>\r\n  <h3>Stats for Game " + gameID + "</h3>\r\n  <table border=\"1\">\r\n    <thead>\r\n      <tr>\r\n        <td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>\r\n      </tr>\r\n    </thead>\r\n    <tbody>");
                    try
                    {
                        using (var conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            using var cmd = new MySqlCommand("SELECT PlayerID, Name, MaxScore, EnterTime, LeaveTime FROM Players WHERE Players.GameID = " + gameID + " ", conn);
                            using var reader = cmd.ExecuteReader();

                            //assemble the html with the sql data
                            while (reader.Read())
                            {
                                html.Append($"<tr><td>{reader["playerID"]}</a></td>");
                                html.Append($"<td>{reader["Name"]}</td>");
                                html.Append($"<td>{reader["MaxScore"]}</td>");
                                html.Append($"<td>{reader["EnterTime"]}</td>");
                                html.Append($"<td>{reader["LeaveTime"]}</td></tr>");
                            }
                        }

                        html.Append("</tbody></table></html>");
                        connection.Send(httpOkHeader + html.ToString());
                    }
                    catch (Exception)
                    {

                    }
                }
                connection.Disconnect();
            }
            catch (Exception)
            {


            }

        }
    }
}
