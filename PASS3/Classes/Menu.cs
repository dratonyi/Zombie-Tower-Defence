/* Author: Dani Ratonyi
 * File name: Menu.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: the main menu of the game
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
using MonoGame.Extended;

namespace PASS3
{
    class Menu
    {
        //array for the buttons in the menu
        private Button[] buttons = new Button[3];

        //constants for each button
        private const int PLAY_BT = 0;
        private const int SCORE_BT = 1;
        private const int EXIT_BT = 2;

        //variables for the display of the game's title
        private Texture2D titleImg;
        private Rectangle titleRec;

        //integer for the height change of the menu buttons with the title
        private int heightBuffer = 75;

        /// <summary>
        /// loads the menu
        /// </summary>
        /// <param name="content"></param> ContentManager that is able to load images into the project
        public Menu(ContentManager content)
        {
            //loops through the buttons and initializes all the variables
            for (int i = 0; i < buttons.Length; i++)
            {
                //creates a new button
                buttons[i] = new Button(true);
            }

            //loads the play button
            buttons[PLAY_BT].LoadButtonImg("Images/Sprites/playButton", "Images/Sprites/playBtSel", content);
            buttons[PLAY_BT].LoadButtonRec(Game1.WIDTH / 2 - (int)(buttons[PLAY_BT].ImgWidth * 0.35f), (int)(Game1.HEIGHT * 0.2f) + heightBuffer, (int)(buttons[PLAY_BT].ImgWidth * 0.7f), (int)(buttons[PLAY_BT].ImgHeight * 0.7f));

            //loads the score button 
            buttons[SCORE_BT].LoadButtonImg("Images/Sprites/scoreBt", "Images/Sprites/scoreBtSel", content);
            buttons[SCORE_BT].LoadButtonRec(Game1.WIDTH / 2 - (int)(buttons[SCORE_BT].ImgWidth / 2 * 0.7f), (int)(Game1.HEIGHT * 0.5f) + heightBuffer / 6, (int)(buttons[SCORE_BT].ImgWidth * 0.7f), (int)(buttons[SCORE_BT].ImgHeight * 0.7f));

            //loads the exit button's image
            buttons[EXIT_BT].LoadButtonImg("Images/Sprites/exitBt", "Images/Sprites/exitBtSel", content);
            buttons[EXIT_BT].LoadButtonRec(Game1.WIDTH / 2 - (int)(buttons[EXIT_BT].ImgWidth / 2 * 0.7f), (int)(Game1.HEIGHT * 0.65f) + heightBuffer, (int)(buttons[EXIT_BT].ImgWidth * 0.7f), (int)(buttons[EXIT_BT].ImgHeight * 0.7f));

            //loads the image of the game's title
            titleImg = content.Load<Texture2D>("Images/Sprites/titleImg");
            titleRec = new Rectangle(0, 0, Game1.WIDTH, Game1.HEIGHT);
        }

        /// <summary>
        /// updates the menu
        /// </summary>
        /// <param name="mouse"></param> MouseState for the position and state of the mosue
        /// <param name="prevMouse"></param> boolean for whether the mouse was clicked on in the last frame
        /// <param name="mousePos"></param> vector2 position of the mouse
        /// <param name="gamePlay"></param> GamePlay instance to be able to start the gameplay
        /// <param name="content"></param> ContentManager instance to help loading content
        public void UpdateMenu(MouseState mouse, bool prevMouse, Vector2 mousePos, Scoreboard scoreboard, Play gamePlay, ContentManager content)
        {
            //loops through the array of buttons and updates them all
            for (int i = 0; i < buttons.Length; i++)
            {
                //updates the current button
                if(buttons[i].UpdateButton(mouse, prevMouse, mousePos) == true)
                {
                    //checks which button was clicked and excecutes the approporiate command
                    switch (i)
                    {
                        case PLAY_BT:
                            //starts the game
                            gamePlay.LoadLevels(content);
                            Game1.GameState =  Game1.PLAY;
                            break;

                        case SCORE_BT:
                            //goes to the scoreboard and reads the scores from the files
                            scoreboard.ReadBoard();
                            Game1.GameState = Game1.SCOREBOARD;
                            break;

                        case EXIT_BT:
                            //exits the game
                            Game1.GameState = Game1.EXIT;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// draws the menu
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch
        public void DrawMenu(SpriteBatch spriteBatch)
        {
            //draws the backgrounds image
            //spriteBatch.Draw(bgImg, bgRec, Color.White);

            //draws the title
            spriteBatch.Draw(titleImg, titleRec, Color.White);
            
            //loops through the array of buttons and draws them all
            for (int i = 0; i < buttons.Length; i++)
            {
                //draws a button
                buttons[i].DrawButton(spriteBatch);
            }
        }
    }
}
