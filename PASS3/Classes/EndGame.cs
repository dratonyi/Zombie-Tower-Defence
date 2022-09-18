/* Author: Dani Ratonyi
 * File name: EmptyTile.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for what happens when the game is over
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

namespace PASS3
{
    class EndGame
    {
        //variable for the final score of the player
        private Score finalScore;
               
        //variable for the menu button
        private Button menuBt = new Button(true);

        //variables for the image and rectangle of the name bar
        private Texture2D nameBarImg;
        private Rectangle nameBarRec;

        //variables for the end game message
        private Texture2D gameOverImg;
        private Rectangle gameOverRec;

        //variables for the name of the player and its display on the screen
        private string playerName = "";
        private Vector2 namePos;
        private SpriteFont nameFont;

        //boolean for whether the player is writing their name down or not
        private bool naming = true;

        //position for the id of the name
        private Vector2 nameIDPos;
        private SpriteFont nameIDFont;

        /// <summary>
        /// constructs an instance of the end game
        /// </summary>
        /// <param name="content"></param> ContentManager instance to load images
        /// <param name="scoreboard"></param> Scoreboard to add a new player to the scoreboard
        public EndGame(ContentManager content, Scoreboard scoreboard)
        {
            //loads the font for displaying "name: "
            nameIDFont = content.Load<SpriteFont>("Fonts/nameFt");

            //loads the image of the menu button
            menuBt.LoadButtonImg("Images/Sprites/menuBt", "Images/Sprites/menuBtSel", content);
            menuBt.LoadButtonRec(Game1.WIDTH / 2 - (int)(menuBt.ImgWidth * 0.7f / 2), Game1.HEIGHT / 6 * 5 - (int)(menuBt.ImgHeight * 0.7f / 2), (int)(menuBt.ImgWidth * 0.7f), (int)(menuBt.ImgHeight * 0.7f));

            //loads the image and rectangle of the nameBar
            nameBarImg = content.Load<Texture2D>("Images/Sprites/nameBar");
            nameBarRec = new Rectangle(Game1.WIDTH / 2 - (int)(nameBarImg.Width / 2.5f / 2), Game1.HEIGHT / 3 * 2 - (int)(nameBarImg.Height / 2.5f / 2), (int)(nameBarImg.Width / 2.5f), (int)(nameBarImg.Height / 2.5f));

            //loads the image of the game over message and its destination rectangle
            gameOverImg = content.Load<Texture2D>("Images/Sprites/gameOverImg");
            gameOverRec = new Rectangle(Game1.WIDTH / 2 - gameOverImg.Width / 2, Game1.HEIGHT / 15, gameOverImg.Width, gameOverImg.Height);

            //sets the position of the name input
            namePos = new Vector2(nameBarRec.X + 10, nameBarRec.Y + 11);

            //sets the font for the user input
            nameFont = scoreboard.SearchFont;

            //sets the position of the name indicator
            nameIDPos = new Vector2(nameBarRec.X, nameBarRec.Y - 40);

            //sets the final score as a backup
            finalScore = new Score(content, new Vector2());
        }

        /// <summary>
        /// when the game goes to endgame it gets activated
        /// </summary>
        /// <param name="score"></param> the final score of the player in the game
        public void ActivateEndGame(Score score)
        {
            //sets naming to true
            naming = true;

            //sets the players score to the given parameter
            finalScore = score;
        }

        /// <summary>
        /// updates the end game
        /// </summary>
        /// <param name="scoreboard"></param> Scoreboard instance to update the scoreboard
        /// <param name="kb"></param> Keyboardstate to get keyboard input
        /// <param name="prevKb"></param> Keyboard state to verify input
        /// <param name="mouse"></param> MouseState to get mouse input
        /// <param name="prevMouse"></param> bool to verify mouse input
        /// <param name="mousePos"></param> Vector2 to get mouse position
        public void UpdateEndGame(Scoreboard scoreboard, KeyboardState kb, KeyboardState prevKb, MouseState mouse, bool prevMouse, Vector2 mousePos)
        {
            //gets the users input to get their name
            playerName = scoreboard.GetKeyboardInput(playerName, kb, prevKb);

            //if the user presses enter their name and score gets saved in the scoreboard
            if (kb.IsKeyDown(Keys.Enter) && kb != prevKb && naming && scoreboard.CheckForDuplicates(playerName) == false && playerName != "")
            {
                //adds the player to the scoreboard
                scoreboard.AddPlayer(new Player(playerName, finalScore.Amount));

                //stops getting the name from the user
                naming = false;
            }

            //if the player presses the return to menu button the game goes back to the menu
            if (menuBt.UpdateButton(mouse, prevMouse, mousePos))
            {
                //changes the gamestate to the menu
                Game1.GameState = Game1.MENU;
            }
        }

        /// <summary>
        /// draws the end game
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch instance that allows for drawing
        public void DrawEndGame(SpriteBatch spriteBatch)
        {
            //draws the menu button
            menuBt.DrawButton(spriteBatch);

            //draws the game over image
            spriteBatch.Draw(gameOverImg, gameOverRec, Color.White);

            //if the player is naming themselves it shows the namebar and the name he is typing in
            if(naming)
            {
                //draws the namebar and the name the user types into it
                spriteBatch.Draw(nameBarImg, nameBarRec, Color.White);
                spriteBatch.DrawString(nameFont, playerName, namePos, Color.Black);

                //draws the name indicator
                spriteBatch.DrawString(nameIDFont, "Name:", nameIDPos, Color.Black);

                //calls the score to draw itself
                finalScore.DrawScore(spriteBatch);
            }            
        }
    }
}
