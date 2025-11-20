// <copyright file="Snake.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Kellen Auth and Other Student </authors>
//<Date> Last modified November 2024 </Date>
using System.Collections;

namespace GUI.Client.Models
{
    /// <summary>
    /// Snake object representing player in snake game
    /// </summary>
    public class Snake
    {
        /// <summary>
        /// an int representing the snake's unique ID.  
        /// </summary>
        public int snake { get; set; }

        /// <summary>
        /// a string representing the player's name.
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        ///a List[Point2D] representing the entire body of the snake. (See below for description of Point2D).
        /// Each point in this list represents one vertex of the snake's body, where two consecutive vertices
        ///make up one straight segment of the body. The first point of the list gives the location of the
        ///snake's tail, and the last gives the location of the snake's head.
        /// </summary>
        public List<Point2D> body { get; set; }

        /// <summary>
        /// an Point2D representing the snake's orientation. This will always be an axis-aligned vector
        ///(purely horizontal or vertical). This can be inferred from other information, but some clients may
        ///find it useful.
        /// </summary>
        public Point2D dir { get; set; }

        /// <summary>
        /// an int representing the player's score (the number of powerups it has eaten).
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// a bool indicating if the snake died on this frame. This will only be true on the exact frame in which
        ///the snake died. You can use this to determine when to start drawing an explosion or some other way of
        ///representing that the snake died.
        /// </summary>
        public bool died { get; set; }

        /// <summary>
        /// a bool indicating whether a snake is alive or dead. This is helpful for knowing when to draw the snake
        ///in a different way (or not draw it at all) between the time that it dies and the time that it respawns.
        /// </summary>
        public bool alive { get; set; }

        /// <summary>
        /// a bool indicating if the player controlling that snake disconnected on that frame. The server will send
        ///the snake with this flag set to true only once, then it will discontinue sending that snake for the rest
        ///of the game. You can use this to remove disconnected players from your model.
        /// </summary>
        public bool dc { get; set; }

        /// <summary>
        /// a bool indicating if the player joined on this frame. This will only be true for one frame. This field
        ///may not be needed, but may be useful for certain additional View related features
        ///(e.g. displaying a message that a new player has joined).
        /// </summary>
        public bool join { get; set; }

        /// <summary>
        /// an int representing the max score the snake has gotten.
        /// </summary>
        public int maxScore { get; set; } = 0;
    }
}
