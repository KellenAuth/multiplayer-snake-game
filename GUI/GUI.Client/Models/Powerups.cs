// <copyright file="Powerups.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified November 2024 </Date>
namespace GUI.Client.Models
{
    /// <summary>
    /// Game object representing powerups in snake game
    /// </summary>
    public class Powerups
    {
        /// <summary>
        /// an int representing the powerup's unique ID.
        /// </summary>
        public int power { get; set; }
        /// <summary>
        /// a Point2D representing the location of the powerup.
        /// </summary>
        public Point2D loc { get; set; }
        /// <summary>
        /// a bool indicating if the powerup "died" (was collected by a player) on this frame.The server will send the dead powerups only once.
        /// </summary>
        public bool died { get; set; }
    }
}
