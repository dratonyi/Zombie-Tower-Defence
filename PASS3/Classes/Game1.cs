/* Author: Dani Ratonyi
 * File name: Game1.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: driver class that handles switching gamestates
 */

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Helper;
using Animation2D;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //constant for the number of seconds in a minute
        public const int MINUTE = 60;
        
        //static variable for the gamestate
        private static int gameState = MENU;

        //streamreader that can read in files
        private static StreamReader inFile;

        //random instance
        private static Random rng = new Random();

        //variable for the graphics device
        private static GraphicsDevice gd;

        //constants for the width and height of the screen
        public const int WIDTH = 1200;
        public const int HEIGHT = 800;

        //constant for the back button
        public const int BACK_BT = 0;

        //variable for the back button
        Button backBt = new Button(true);
        
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //variables for the background img
        private Texture2D bgImg;

        //variables for the scrolling background rectangles
        private Rectangle bgRec1;

        //variables for the width and height of the screen
        private int screenWidth;
        private int screenHeight;

        //variable for the menu object
        private Menu mainMenu;

        //list of the levels in the game
        private Play gamePlay;

        //creates an instance of the end game
        private EndGame endGame;

        //creates an instance of the scoreboard
        private Scoreboard scoreboard;

        //variables for the state of the keyboard
        KeyboardState kb;
        KeyboardState prevKb;

        //variables for the location and status of the mouse
        MouseState mouse;
        bool prevMouse = false;
        private Vector2 mousePos = new Vector2();

        //constants for each game state and a variable for game state
        public const int MENU = 0;
        public const int INSTRUCTIONS = 1;
        public const int PLAY = 2;
        public const int PAUSE = 3;
        public const int END_GAME = 4;
        public const int SCOREBOARD = 5;
        public const int EXIT = 6;        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //sets the width and height of the game window
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

            //applies the graphical changes
            graphics.ApplyChanges();

            //makes the mouse visible to the user
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //sets the GraphicsDevice to the new static variable gd
            gd = GraphicsDevice;

            // TODO: use this.Content to load your game content here  

            //loads the background's image and rectangle
            bgImg = Content.Load<Texture2D>("Images/Backgrounds/menuBackground");
            bgRec1 = new Rectangle(0, 0, bgImg.Width, HEIGHT);

            //loads the image of the back button
            backBt.LoadButtonImg("Images/Sprites/backBt", "Images/Sprites/backBtSel", Content);
            backBt.LoadButtonRec(Play.TILE_WIDTH / 4, Play.TILE_WIDTH / 4, (int)(backBt.ImgWidth * 0.4f), (int)(backBt.ImgHeight * 0.4f));

            //sets variables to the width and height of the screen
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //calls a method of the menu to load the main menu
            mainMenu = new Menu(Content);

            //calls a method of the gamePlay to load the gamePlay
            gamePlay = new Play(Content);

            //initializes the scoreboard class
            scoreboard = new Scoreboard(Content);

            //loads the end game class
            endGame = new EndGame(Content, scoreboard);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Exit();

            // TODO: Add your update logic here

            //updates the position of the mouse
            mouse = Mouse.GetState();
            mousePos.X = mouse.X;
            mousePos.Y = mouse.Y;

            //updates the state of the keyboard
            kb = Keyboard.GetState();

            //decides which game state's logic should be executed
            switch (gameState)
            {
                case MENU:
                    //updates the menu
                    mainMenu.UpdateMenu(mouse, prevMouse, mousePos, scoreboard, gamePlay, Content);
                    break;

                case PLAY:
                    //updates the game                    
                    gamePlay.UpdatePlay(endGame, gameTime, mouse, prevMouse, mousePos, kb, prevKb, Content);
                    break;

                case PAUSE:
                    break;

                case END_GAME:
                    //updates logic after the game ended
                    endGame.UpdateEndGame(scoreboard, kb, prevKb, mouse, prevMouse, mousePos);
                    break;

                case SCOREBOARD:
                    //calls the scoreboard to update itself
                    scoreboard.UpdateScoreboard(kb, prevKb, mouse, prevMouse, mousePos);

                    //calls the method to update the back button
                    UpdateBackButton();
                    break;

                case EXIT:
                    Exit();
                    break;  
            }

            //if the left button is down and the prevMouse variable has not been updated yet it gets updated
            if (mouse.LeftButton == ButtonState.Pressed  && prevMouse == false)
            {
                //prevMouse is set to true indicating that the left button was down in this frame
                prevMouse = true;
            }
            else if(mouse.LeftButton != ButtonState.Pressed && prevMouse == true)
            {
                //prevMouse is set to false indicating that the left button has been released in this frame 
                prevMouse = false;
            }

            //sets the prevKb to kb to make sure no double clicks happen
            prevKb = kb;

            base.Update(gameTime);
        }

        /// <summary>
        /// updates the back button
        /// </summary>
        private void UpdateBackButton()
        {
            //if the back button is clicked the game returns to the menu
            if (backBt.UpdateButton(mouse, prevMouse, mousePos) == true)
            {
                //makes the gamestate the menu
                gameState = MENU;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //starts a spriteBatch
            spriteBatch.Begin();

            if(gameState != PLAY)
            {
                spriteBatch.Draw(bgImg, bgRec1, Color.White);
            }

            //decides which game state should be drawn
            switch (gameState)
            {
                case MENU:
                    //draws the menu
                    mainMenu.DrawMenu(spriteBatch);
                    break;

                case PLAY:
                    //draws the gameplay
                    gamePlay.DrawPlay(spriteBatch);
                    break;

                case PAUSE:
                    break;

                case END_GAME:
                    endGame.DrawEndGame(spriteBatch);
                    break;

                case SCOREBOARD:
                    //draws the scoreboard
                    scoreboard.DrawBoard(spriteBatch);

                    //calls a method to draw the back button
                    DrawBackButton();
                    break;
            }

            //ends the current spriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// draws the back button
        /// </summary>
        public void DrawBackButton()
        {
            //draws the back button
            backBt.DrawButton(spriteBatch);
        }

        /// <summary>
        /// property that is able to get and set the value of gameState
        /// </summary>
        public static int GameState
        {
            get { return gameState; }
            set { gameState = value; }
        }

        /// <summary>
        /// property that can return the value of rng
        /// </summary>
        public static Random Rng
        {
            get { return rng; }
        }

        /// <summary>
        /// property that can set and return the value of inFile
        /// </summary>
        public static StreamReader InFile
        {
            get { return inFile; }
            set { inFile = value; }
        }

        /// <summary>
        /// property that returns the static Graphics device variable gd
        /// </summary>
        public static GraphicsDevice Gd
        {
            get { return gd; }
        }
    }
}
