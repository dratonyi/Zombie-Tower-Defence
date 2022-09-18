/* Author: Dani Ratonyi
 * File name: PathTile.cs
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
using Animation2D;

namespace PASS3
{
    class PathTile: Tile
    {
        /// <summary>
        /// constructs a path tile
        /// </summary>
        /// <param name="x"></param> x position
        /// <param name="y"></param> y position
        /// <param name="width"></param> width
        /// <param name="Content"></param> ContentManager instance to load images
        public PathTile(int x, int y, int width, ContentManager Content) : base(x, y, width, Content)
        {
            //loads the image of the empty tile
            tileImg = Content.Load<Texture2D>("Images/Sprites/pathTile");

            //creates a rectangle at the given position
            tileRec = new Rectangle(x, y, width, width);

            //sets the select color to red
            selectColor = Color.Red;
        }
    }
}
