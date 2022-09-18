/* Author: Dani Ratonyi
 * File name: Resource.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for the resources
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Helper;
using MonoGame.Extended;

namespace PASS3
{
    class Resource
    {
        //constant for how often resources are generated
        public const int MAX_GEN_TIME = 5000;

        //variable for how fast resources are generated
        public const int GEN_RATE = 10;

        //variable for the amount of the resource remaining
        private int amount;

        //variable for the font used for the building resource display
        private SpriteFont font;

        //variables for the image and rectangle of the build icon
        private Texture2D resImg;
        private Rectangle resRec;
        
        //variable for the type of resource
        private int type;

        //the color of the resource's text
        private Color color;

        //timer variable for each time a resource is generated
        private Timer genTimer = new Timer(MAX_GEN_TIME, true);

        //vector 2 for the position of the text
        Vector2 msgPos;

        /// <summary>
        /// constructs a resource instance
        /// </summary>
        /// <param name="content"></param> contentManager instance to load content
        /// <param name="x"></param> x position
        /// <param name="y"></param> y position
        /// <param name="width"></param> width
        /// <param name="resImg"></param> the image of the resource
        /// <param name="type"></param> the type of reseource
        public Resource(ContentManager content, int x, int y, int width, Texture2D resImg, int type)
        {
            //sets the type to the given parameter
            this.type = type;

            //depending of the type of resource the color gets set
            if (type == Play.WOOD)
            {
                //sets the color to brown for wood
                color = Color.BurlyWood;
            }
            else
            {
                //sets the color to gray for stone
                color = Color.Gray;
            }
            
            //sets the position of the resRec
            resRec = new Rectangle(x, y, width, width);

            //loads the font
            font = content.Load<SpriteFont>("Fonts/UiFont");

            //sets the image of the build's icon
            this.resImg = resImg;
           
            //adds the x and y position of the resource to its position
            msgPos = new Vector2(x + width + width / 4, y);
        }

        /// <summary>
        /// generates more of a given resource
        /// </summary>
        /// <param name="numLevels"></param> the number of levels remaining
        ///<param name="gameTime"></param> gameTime instance that allows for use of the timer 
        public void GenerateReseource(GameTime gameTime, int numLevels)
        {
            //updates the resource timer
            genTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //if the timer is up add to the resource
            if(genTimer.GetTimePassed() >= MAX_GEN_TIME)
            {
                //reset the timer
                genTimer.ResetTimer(true);

                //add to the resource based on the number of levels the player has left
                amount += GEN_RATE * numLevels;
            }
        }

        /// <summary>
        /// property that is able to return and set the value of amount
        /// </summary>
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        /// <summary>
        /// property that returns the rectangle of the resource's icon
        /// </summary>
        public Rectangle Rec
        {
            get { return resRec; }
        }

        /// <summary>
        /// property that returns the type of resource this object is
        /// </summary>
        public int Type
        {
            get { return type; }
        }

        /// <summary>
        /// displays the number of the resource remaining with its icon
        /// </summary>
        /// <param name="spriteBatch"></param> spriteBatch that allows for drawing
        public void DisplayRes(SpriteBatch spriteBatch)
        {
            //draws the amount of resource remaining with its icon
            spriteBatch.Draw(resImg, resRec, Color.White);
            spriteBatch.DrawString(font, Convert.ToString(amount), msgPos, color);
        }
    }
}
