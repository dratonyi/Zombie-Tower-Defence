/* Author: Dani Ratonyi
 * File name: Tile.cs
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
    class Tile
    {
        //rectangle for the space the tile will take up
        protected Rectangle tileRec;

        //variable for the image of the tile
        protected Texture2D tileImg;

        //variable for whether the tile is selected
        protected bool selected = false;

        //variable for whath color the selected tile should be 
        protected Color selectColor;

        //variable for the type of tile
        protected int type;

        /// <summary>
        /// constructs a tower tile
        /// </summary>
        /// <param name="x"></param> x position
        /// <param name="y"></param> y position
        /// <param name="width"></param> width
        /// <param name="Content"></param> ContentManager instance to load images
        /// <param name="type"></param> integer for the type of tower
        public Tile(int x, int y, int width, ContentManager Content, int type)
        {
                   
        }

        /// <summary>
        /// constructs a tile
        /// </summary>
        /// <param name="x"></param> x position
        /// <param name="y"></param> y position
        /// <param name="width"></param> width
        /// <param name="Content"></param> ContentManager instance to load images
        public Tile(int x, int y, int width, ContentManager Content)
        {

        }

        /// <summary>
        /// updates a building tile 
        /// </summary>
        /// <param name="gameTime"></param> gameTime instance that allows for the timer to work
        /// <param name="resources"></param> a resource that will be generated
        public virtual void UpdateTile(GameTime gameTime, Resource resource)
        {

        }

        /// <summary>
        /// updates the logic of the tower tile
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance that allows for the timer to work
        /// <param name="enemies"></param> list of enemies the tower will be attacking
        /// <param name="mousePos"></param> the position of the mouse to see if its hovering over the tower
        public virtual void UpdateTile(GameTime gametime, List<Enemy> enemies, Vector2 mousePos)
        {

        }

        /// <summary>
        /// draws an individual tile
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch instanse that allows for drawing
        public virtual void DrawTile(SpriteBatch spriteBatch)
        {
            //if the tile isnt selected it gets drawn regularly otherwise it gets drawn with a color
            if (!selected)
            {
                //draws the image of the tile in the tile rectangle
                spriteBatch.Draw(tileImg, tileRec, Color.White);
            }
            else
            {
                //draws the image of the tile in the tile rectangle
                spriteBatch.Draw(tileImg, tileRec, selectColor);

                //unselects the tile
                selected = false;
            }
            
            //draws a rectangle outline of the tile
            spriteBatch.DrawRectangle(tileRec, Color.Black, 1, 0);
        }

        /// <summary>
        /// draws the projectile
        /// </summary>
        /// <param name="spriteBatch"></param> spriteBatch that allows for drawing
        public virtual void DrawExtra(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// //property that returns the rectangle around the tile
        /// </summary>
        public Rectangle Rec
        {
            get { return tileRec; }
        }

        /// <summary>
        /// property that returns whether a tile is being selected 
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// property that returns the type of the tile
        /// </summary>
        public int Type
        {
            get { return type; }
        }
    }
}
