/* Author: Dani Ratonyi
 * File name: BuildingTile.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: a special tile that is able to generate resources
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
using Helper;
using Animation2D;

namespace PASS3
{
    class BuildingTile : Tile
    {
        //constant for the rate at which the building is generating resources
        public const int GEN_RATE = 2;

        //timer for how often the building is generating resources
        private Timer genTimer = new Timer(Resource.MAX_GEN_TIME, true);

        /// <summary>
        /// constructs an instance of the buildingTile
        /// </summary>
        /// <param name="x"></param> int for the x position of the building
        /// <param name="y"></param> int for the y position of the building
        /// <param name="width"></param> int for the width of the building
        /// <param name="content"></param> ContentManager instance to help loading images
        /// <param name="type"></param> int for the type of resource it generates
        public BuildingTile(int x, int y, int width, ContentManager content, int type) : base(x, y, width, content, type)
        {
            //sets the type to the given parameter
            this.type = type;

            //depending on the type of tower the proper image gets loaded
            switch (type)
            {
                case Play.WOOD:
                    //loads the lumbre mill image
                    tileImg = content.Load<Texture2D>("Images/Sprites/millImg");
                    break;

                case Play.STONE:
                    //loads the mine img
                    tileImg = content.Load<Texture2D>("Images/Sprites/mineImg");
                    break;                    
            }

            //creates a rectangle at the given position
            tileRec = new Rectangle(x, y, width, width);

            //sets the select color to red
            selectColor = Color.Red;
        }

        /// <summary>
        /// updates the building tile's logic
        /// </summary>
        /// <param name="gameTime"></param> game time variable that allows for drawing
        /// <param name="resource"></param> resource variable that will be generated
        public override void UpdateTile(GameTime gameTime, Resource resource)
        {
            //calls a method to generate resources
            GenerateResource(gameTime, resource);
        }

        /// <summary>
        /// generates resources from the building
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance that allows for the timer to work
        /// <param name="resources"></param> an array of resources to choose from
        private void GenerateResource(GameTime gameTime, Resource resource)
        {
            //updates the resource timer
            genTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //if the gentimer is up resources get generated
            if (genTimer.GetTimePassed() >= Resource.MAX_GEN_TIME)
            {
                //resets the timer
                genTimer.ResetTimer(true);
                
                //generates a resource
                resource.Amount += GEN_RATE;
            }
        }
    }
}
