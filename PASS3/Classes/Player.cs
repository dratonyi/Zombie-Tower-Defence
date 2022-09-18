/* Author: Dani Ratonyi
 * File name: Player.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for the player
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    class Player
    {
        //variables for the attributes of the player
        private string name;
        private int score;

        //variable for the displayed info of the player
        private string display;

        //variable whether the current player is being searched up
        private bool searched = false;

        /// <summary>
        /// constructs an instance of the player
        /// </summary>
        /// <param name="name"></param> string for the name of the player
        /// <param name="score"></param> int for the score of the player
        public Player(string name, int score)
        {
            //sets the attributes of the player to the given attributes
            this.name = name;
            this.score = score;

            //sets the display of the player's score
            display = name + ": " + score;
        }

        /// <summary>
        /// property that returns the name of the player
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// property that returns the score of the player
        /// </summary>
        public int Score
        {
            get { return score; }
        }

        /// <summary>
        /// property that can return the displayed info 
        /// </summary>
        public string Display
        {
            get { return display; }
        }

        /// <summary>
        /// property that can return and set the value of searched
        /// </summary>
        public bool Searched
        {
            get { return searched; }
            set { searched = value; }
        }
    }
}
