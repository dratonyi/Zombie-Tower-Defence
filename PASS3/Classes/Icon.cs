/* Author: Dani Ratonyi
 * File name: Icon.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: subclass of the button but the icon also has a cost
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
    class Icon: Button
    {
        //variables for the amouint of wood and stone it costs to place down the buildings
        private int woodCost;
        private int stoneCost;

        //spritefont for the font of the 
        SpriteFont font;

        //variables for the position of the cost of the building
        private Vector2 woodCostPos;
        private Vector2 stoneCostPos;

        /// <summary>
        /// constructs an instance of an icon
        /// </summary>
        /// <param name="visible"></param> bool for whether its visible
        /// <param name="woodCost"></param> int for its wood cost
        /// <param name="stoneCost"></param> int for its stone cost
        /// <param name="Content"></param> ContentManager instance to load content
        public Icon(bool visible, int woodCost, int stoneCost, ContentManager Content): base(visible)
        {
            //sets the attributes of the icon to the given parameters
            this.visible = visible;
            this.woodCost = woodCost;
            this.stoneCost = stoneCost;

            //loads the font
            font = Content.Load<SpriteFont>("Fonts/costFont");
        }

        /// <summary>
        /// loads the rectangle around the button and the positions of the tower costs
        /// </summary>
        /// <param name="x"></param> the x position of the icon
        /// <param name="y"></param> the y position of the icon
        /// <param name="width"></param> the width of the icon
        /// <param name="height"></param> the height of the icon
        public override void LoadButtonRec(int x, int y, int width, int height)
        {
            //calls the parent function to load the rectangle
            base.LoadButtonRec(x, y, width, height);

            //sets the position of the cost
            woodCostPos = new Vector2(x, y - 30);
            stoneCostPos = new Vector2(x + width - (Convert.ToString(stoneCost).Length * 12), y - 30);
        }

        /// <summary>
        /// property that returns the amount of wood the building costs
        /// </summary>
        public override int WoodCost
        {
            get { return woodCost; }
        }

        /// <summary>
        /// property that returns the amount of stone the building costs
        /// </summary>
        public override int StoneCost
        {
            get { return stoneCost; }
        }

        /// <summary>
        /// draws the icon
        /// </summary>
        /// <param name="spriteBatch"></param> instance of the SpriteBatch that allows for drawing
        public override void DrawButton(SpriteBatch spriteBatch)
        {
            //calls the parent class's draw method 
            base.DrawButton(spriteBatch);

            //draws the cost of the tower or building 
            spriteBatch.DrawString(font, Convert.ToString(woodCost), woodCostPos, Color.BurlyWood);
            spriteBatch.DrawString(font, Convert.ToString(stoneCost), stoneCostPos, Color.Gray);
        }
    }
}
