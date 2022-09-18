/* Author: Dani Ratonyi
 * File name: EmptyTile.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: A tile on the 2 dimensional map of the game
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using Animation2D;

namespace PASS3
{
    class EmptyTile : Tile
    {
        /// <summary>
        /// constructs an empty tile
        /// </summary>
        /// <param name="x"></param> x position
        /// <param name="y"></param> y position
        /// <param name="width"></param> width
        /// <param name="Content"></param> ContentManager instance to load images
        public EmptyTile(int x, int y, int width, ContentManager Content) : base(x, y, width, Content)
        {
            //loads the image of the empty tile
            tileImg = Content.Load<Texture2D>("Images/Sprites/emptyTile");

            //creates a rectangle at the given position
            tileRec = new Rectangle(x, y, width, width);

            //sets the select color to green
            selectColor = Color.Green;
        }
    }
}
