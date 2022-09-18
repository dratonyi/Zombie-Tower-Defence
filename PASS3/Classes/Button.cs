/* Author: Dani Ratonyi
 * File name: Button.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: button that can be clicked by the user
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

namespace PASS3
{
    class Button
    {            
        //variables for the image of the button
        protected Texture2D buttonImg;
        protected Texture2D selButtonImg;

        //rectangle for the button
        Rectangle buttonRec;        

        //boolean for whether the button is selected
        protected bool selected = false;

        //boolean for whether the button is visible or not
        protected bool visible;

        /// <summary>
        /// constructs an instance of a button
        /// </summary>
        /// <param name="visible"></param>
        public Button(bool visible)
        {
            //sets the visibility of the button
            this.visible = visible;
        }

        /// <summary>
        /// loads the image of the button 
        /// </summary>
        /// <param name="buttonPath"></param> the file path of the button img
        /// <param name="selButtonPath"></param> the file path of the selected button img
        /// <param name="Content"></param> ContentManager 
        public void LoadButtonImg(string buttonPath, string selButtonPath, ContentManager Content)
        {
            //loads the regular and the selected button image
            buttonImg = Content.Load<Texture2D>(buttonPath);
            selButtonImg = Content.Load<Texture2D>(selButtonPath);
        }
              
        /// <summary>
        /// loads the rectangle around the button
        /// </summary>
        /// <param name="x"></param> int for the x position of the rectangle
        /// <param name="y"></param> int for the y position of the rectangle
        /// <param name="width"></param> int for the width of the rectangle
        /// <param name="height"></param> int for the height og the rectangle
        public virtual void LoadButtonRec(int x, int y, int width, int height)
        {
            //creates a rectangle for the image
            buttonRec = new Rectangle(x, y, width, height);
        }

        //accessor for the width of the button
        public int ImgWidth
        {
            get { return buttonImg.Width; }
        }

        //accessor for the height of the button
        public int ImgHeight
        {
            get { return buttonImg.Height; }
        }    

        /// <summary>
        /// property for whether the button is visible
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// property that can return and set whether the button is selected
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// property that returns the rectangle around the button
        /// </summary>
        public Rectangle Rec
        {
            get { return buttonRec; }
        }

        /// <summary>
        /// property that returns the wood cost of an icon
        /// </summary>
        public virtual int WoodCost
        {
            get{ return 0; }
        }

        /// <summary>
        /// property that returns the stone cost of an icon
        /// </summary>
        public virtual int StoneCost
        {
            get { return 0; }
        }

        /// <summary>
        /// updates the state of the button by checking for collisions with the mouse
        /// </summary>
        /// <param name="mouse"></param> instance of a MouseState class that tracks the current position of the mouse
        /// <param name="prevMouse"></param> boolean for whether the left mouse button was pressed in the last frame
        /// <param name="mousePos"></param> vector2 for the current position of the mouse
        /// <returns></returns> whether the button was clicked on or not
        public bool UpdateButton(MouseState mouse, bool prevMouse, Vector2 mousePos)
        {
            if(visible)
            {
                //if the mouse is over the button it gets selected
                if (Helper.Util.Intersects(buttonRec, mousePos))
                {
                    //if the mouse is currently not selected it makes it selected
                    if (!selected)
                    {
                        //makes the button selected
                        selected = true;
                    }

                    //if the mouse is clicked return true
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse == false)
                    {
                        //unselects the button
                        selected = false;
                        
                        //return true
                        return true;
                    }
                }
                else
                {
                    //if the button is seelcted it unselects it
                    if (selected)
                    {
                        //makes the button unselected
                        selected = false;
                    }
                }
            }                                                                   
                       
            //return false;
            return false;
        }

        /// <summary>
        /// draws the button
        /// </summary>
        /// <param name="spriteBatch"></param> sprite batch to let me draw the button
        public virtual void DrawButton(SpriteBatch spriteBatch)
        {
            //if the button is visible it draws the active img otherwise it draws the inactive img
            if (visible)
            {
                //if the button is selected it draws a highlighted picture
                if (selected)
                {
                    //draws the selected button
                    spriteBatch.Draw(selButtonImg, buttonRec, Color.White);
                }
                else
                {
                    //draws the regular button
                    spriteBatch.Draw(buttonImg, buttonRec, Color.White);
                }
            }
            else
            {
                //draws the button with a different color
                spriteBatch.Draw(buttonImg, buttonRec, Color.Gray);
            }
                               
        }
    }
}
