// <copyright file="Walls.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified November 2024 </Date>
namespace GUI.Client.Models
{
    /// <summary>
    /// Class representing Wall objects in snake game 
    /// </summary>
    public class Walls
    {
        /// <summary>
        /// an int representing the wall's unique ID.
        /// </summary>
        public int wall { get; set; }
        /// <summary>
        /// a Point2D representing one endpoint of the wall.
        /// </summary>
        public Point2D p1 { get; set; } 
        /// <summary>
        /// a Point2D representing the other endpoint of the wall.
        /// </summary>
        public Point2D p2 { get; set; }    
    }
}
