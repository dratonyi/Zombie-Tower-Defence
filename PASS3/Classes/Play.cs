/* Author: Dani Ratonyi
 * File name: Play.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: instance of the main gameplay where all elements of the gameplay run
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    class Play
    {   
        //constants for the number of coloumns and rows on the map
        public const int NUM_COL = 9;
        public const int NUM_ROWS = 8;               
        public const int ROW = 0;
        public const int COL = 1;

        //variable for the change in y of the ui elements
        public const int uiHeightBuffer = 50;

        //constants for the number of points the player gets for doing certain things
        public const int KILL_SCORE = 10;
        public const int SURVIVE_SCORE = 100;

        //constants for the startung build amount
        public const int START_BUILD = 100;

        //constatns for the functionality of the buttons
        public const int UP_BT = 0;
        public const int DOWN_BT = 1;
        public const int GRD_TWR_ICON = 2;
        public const int AIR_TWR_ICON = 3;
        public const int MINE_TWR_ICON = 4;
        public const int MILL_TWR_ICON = 5;      

        //constants for the enemy timers
        public const int MAX_ENEMY_TIMER = 10000;
        public const int MIN_ENEMY_TIMER = 750;
        //public const int ENEMY_TIMER_CNG = 250;

        //variable for the width of a tile 
        public const int TILE_WIDTH = 100;

        //constant for the width of an icon
        public const int ICON_WIDTH = 100;

        //constants for the wood and stone generation
        public const int WOOD = 0;
        public const int STONE = 1;

        //constants for the air and ground targets
        public const int GROUND = 0;
        public const int AIR = 1;

        //variable for the button to go back to the menu
        private Button menuBt = new Button(true);
        private Button resumeBt = new Button(true);

        //rectangle and image for the background
        private Texture2D bgImg;
        private Rectangle bgRec;

        //font for the text in the Ui
        private SpriteFont uiFont;
        private SpriteFont costFont;
        private SpriteFont timerFont;
                      
        //list for the 3 levels 
        private List<Level> levels = new List<Level>();

        //variable for the number of levels
        private int numLvls = 3;

        //array for the buttons present in the UI
        private Button[] buttons = new Button[6];        

        //variable for the amount of time passed
        private Timer totalTime;
        private Vector2 timerPos;
        private int minutes;
        private int seconds;

        //variablse for the currently viewd and played level
        private int viewedLvl = 0;        

        //queue for all the enemies in the game that are off the screen
        private EnemyQueue offEnemies;

        //variable for the amount of score the player has
        private Score score;
        //private int[] score = new int[1];
        private Vector2 scorePos;

        //variable for which tower is selected to be placed
        private int selectedTower;

        //boolean for whether the game is paused
        private bool paused = false;

        //array of resources 
        private Resource[] resources = new Resource[2];

        //boolean for whether the game is over or not
        private bool gameOver;

        /// <summary>
        /// constructs an instance of the gameplay
        /// </summary>
        /// <param name="content"></param> ContentManager instance to load content
        public Play(ContentManager content)
        {                        
            //loads the background image and its rectangle
            bgImg = content.Load<Texture2D>("Images/Backgrounds/playBg");
            bgRec = new Rectangle(TILE_WIDTH * NUM_COL, 0, bgImg.Width, bgImg.Height);

            //loads the fonts for the user interface
            uiFont = content.Load<SpriteFont>("Fonts/UiFont");
            costFont = content.Load<SpriteFont>("Fonts/costFont");
            timerFont = content.Load<SpriteFont>("Fonts/timerFont");          

            //loads the images of the arrow buttons
            buttons[DOWN_BT] = new Button(false);
            buttons[UP_BT] = new Button(true);
            buttons[DOWN_BT].LoadButtonImg("Images/Sprites/rightBt", "Images/Sprites/rightBtSel", content);
            buttons[UP_BT].LoadButtonImg("Images/Sprites/leftBt", "Images/Sprites/leftBtSel", content);

            //loads the rectangles of the arrow buttons           
            buttons[UP_BT].LoadButtonRec(TILE_WIDTH * NUM_COL + 25, 25, TILE_WIDTH / 2, TILE_WIDTH / 2);
            buttons[DOWN_BT].LoadButtonRec(TILE_WIDTH * NUM_COL + 25, 25 + TILE_WIDTH / 2, TILE_WIDTH / 2, TILE_WIDTH / 2);

            //loads the ground tower's icon
            buttons[GRD_TWR_ICON] = new Icon(true, 25, 75, content);
            buttons[GRD_TWR_ICON].LoadButtonImg("Images/Sprites/towerIcon", "Images/Sprites/towerIconSel", content);
            buttons[GRD_TWR_ICON].LoadButtonRec(TILE_WIDTH * NUM_COL + 25, Game1.HEIGHT / 2 + uiHeightBuffer, ICON_WIDTH, ICON_WIDTH);

            //loads the image of the air tower's icon  
            buttons[AIR_TWR_ICON] = new Icon(true, 75, 25, content);
            buttons[AIR_TWR_ICON].LoadButtonImg("Images/Sprites/crossbowIcon", "Images/Sprites/crossbowIconSel", content);
            buttons[AIR_TWR_ICON].LoadButtonRec(Game1.WIDTH - 25 - ICON_WIDTH, Game1.HEIGHT / 2 + uiHeightBuffer, ICON_WIDTH, ICON_WIDTH);

            //loads the image of the mine icon
            buttons[MINE_TWR_ICON] = new Icon(true, 100, 0, content);
            buttons[MINE_TWR_ICON].LoadButtonImg("Images/Sprites/mineIcon", "Images/Sprites/mineIconSel", content);
            buttons[MINE_TWR_ICON].LoadButtonRec(Game1.WIDTH - 25 - ICON_WIDTH, Game1.HEIGHT * 3 / 4 + uiHeightBuffer, ICON_WIDTH, ICON_WIDTH);

            //loads the image and rectangle of the wood mill
            buttons[MILL_TWR_ICON] = new Icon(true, 0, 100, content);
            buttons[MILL_TWR_ICON].LoadButtonImg("Images/Sprites/lumberMillIcon", "Images/Sprites/lumberMillIconSel", content);
            buttons[MILL_TWR_ICON].LoadButtonRec(TILE_WIDTH * NUM_COL + 25, Game1.HEIGHT * 3 / 4 + uiHeightBuffer, ICON_WIDTH, ICON_WIDTH);

            //loads the image of the menu button
            menuBt.LoadButtonImg("Images/Sprites/menuBt", "Images/Sprites/menuBtSel", content);
            menuBt.LoadButtonRec(Game1.WIDTH / 2 - (int)(menuBt.ImgWidth * 0.7f / 2), Game1.HEIGHT / 3 * 2 - (int)(menuBt.ImgHeight * 0.7f / 2), (int)(menuBt.ImgWidth * 0.7f), (int)(menuBt.ImgHeight * 0.7f));

            //loads the image and rectangle of the resume button
            resumeBt.LoadButtonImg("Images/Sprites/resumeBt", "Images/Sprites/resumeBtSel", content);
            resumeBt.LoadButtonRec(Game1.WIDTH / 2 - (int)(resumeBt.ImgWidth * 0.7f / 2), Game1.HEIGHT / 3 - (int)(resumeBt.ImgHeight * 0.7f / 2), (int)(resumeBt.ImgWidth * 0.7f), (int)(resumeBt.ImgHeight * 0.7f));

            //creates the stone resource
            resources[WOOD] = new Resource(content, TILE_WIDTH * NUM_COL + 20, buttons[DOWN_BT].Rec.Y + buttons[DOWN_BT].Rec.Height + 25 + uiHeightBuffer, TILE_WIDTH, content.Load<Texture2D>("Images/Sprites/woodIcon"), WOOD);
            resources[STONE] = new Resource(content, TILE_WIDTH * NUM_COL + 20, resources[WOOD].Rec.Y + resources[WOOD].Rec.Height + uiHeightBuffer / 4, TILE_WIDTH, content.Load<Texture2D>("Images/Sprites/stoneIcon"), STONE);

            //loads the position of the timer
            timerPos = new Vector2(buttons[UP_BT].Rec.X + buttons[UP_BT].Rec.Width + ICON_WIDTH / 8, buttons[UP_BT].Rec.Y);

            //sets the position of hte score indicator
            score = new Score(content, new Vector2(TILE_WIDTH * NUM_COL + 20, buttons[DOWN_BT].Rec.Y + buttons[DOWN_BT].Rec.Height + 10));

            //calls a method to load the rest of the gameplay that will have to be loaded every reset of the game
            LoadLevels(content);
        }

        /// <summary>
        /// loads the parts of the game that have to be reloaded upon restart
        /// </summary>
        /// <param name="content"></param> Content Manager that allows for loading content into the project
        public void LoadLevels(ContentManager content)
        {
            //clears the levels
            levels.Clear();
            
            //loops throgu all the levels and draws them all
            for (int i = 0; i < numLvls; i++)
            {
                //loads a level
                levels.Add(new Level(content, offEnemies, timerFont));
            }            

            //starts the game timer
            totalTime = new Timer(Timer.INFINITE_TIMER, true);

            //sets the base resource amount ot the starting amount
            for (int i = 0; i < resources.Length; i++)
            {
                //sets the base resource amount
                resources[i].Amount = START_BUILD;
            }

            //creates all the off enemies
            offEnemies = new EnemyQueue();

            //sets the score to 0
            score.ResetScore();

            //resets the number of seconds and minutes
            minutes = 0;
            seconds = 0;

            //makes sure the game is not paused or over
            gameOver = false;
            paused = false;
        }

        /// <summary>
        /// updates logic while the game is running
        /// </summary>
        /// <param name="endGame"></param> EndGame instance so the score can be saved after the game ends
        /// <param name="gameTime"></param> GameTime instance to help use the timers
        /// <param name="mouse"></param> MouseState for the position and state of the mouse
        /// <param name="prevMouse"></param> boolean for whether the mouse was clicked last update
        /// <param name="mousePos"></param> Vector2 for a different type of position for the mouse
        /// <param name="kb"></param> KeyboardState for the current state of the keyboard
        /// <param name="prevKb"></param> KeyboardState for the state of the keyboard in the previous update
        /// <param name="content"></param> ContentManager to load content into the project
        public void UpdatePlay(EndGame endGame, GameTime gameTime, MouseState mouse, bool prevMouse, Vector2 mousePos, KeyboardState kb, KeyboardState prevKb, ContentManager content)
        {
            //if the game is paused executes pause logic otherwise it executes gameplay logic
            if (paused)
            {
                //calls a method to update the pause gameplay
                UpdatePause(kb, prevKb, mouse, prevMouse, mousePos);
            }
            else
            {
                //if nothing is currently being placed update the gameplay otherwise place a building and pause the game
                if (levels[viewedLvl].Placing == false)
                {
                    //updates the game timer
                    totalTime.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //calls a method to set the minutes and seconds
                    SetTime();

                    //sets arrow button visibility
                    SetArrowBtVisibility(buttons[UP_BT], levels.Count - 1);
                    SetArrowBtVisibility(buttons[DOWN_BT], 0);

                    //calls a method to check for button clicks
                    CheckButtons(mouse, prevMouse, mousePos);

                    //updates the active level and if an enemty reached the end of the path the level is switched
                    if (levels[0].UpdateLvl(content, mouse, mousePos, gameTime, offEnemies, score) == true)
                    {
                        //calls a method to switch the active level
                        NextLvl();       
                    }

                    //if the game ended than move to the end game
                    if(gameOver == true)
                    {
                        //changes the gamestate to end the game
                        Game1.GameState = Game1.END_GAME;

                        //activates the endGame class
                        endGame.ActivateEndGame(score);
                    }

                    //calls a method to generate resources
                    GenerateResources(gameTime);

                    //updates all the tiles
                    for (int i = 0; i < levels.Count; i++)
                    {
                        //updates all the tiles on a level
                        levels[i].UpdateTiles(gameTime, resources, content, mousePos);
                    }

                    //if the player presses escape pauses the game 
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb)
                    {
                        //pauses the game
                        paused = true;

                        //sets prevKb
                        prevKb = kb;
                    }
                }
                else
                {
                    //calls the method of the viewed level to place a tower
                    levels[viewedLvl].PlaceTower(selectedTower, resources, buttons[selectedTower], mouse, mousePos, content);

                    //if the escape key is down it stops placing the tower
                    if (kb.IsKeyDown(Keys.Escape) && kb != prevKb || levels[viewedLvl].Placing == false)
                    {
                        //sets placing to false
                        levels[viewedLvl].Placing = false;

                        //makes all buttons visible
                        MakeAllButtonsVisible();

                        //sets arrow button visibility
                        SetArrowBtVisibility(buttons[UP_BT], levels.Count - 1);
                        SetArrowBtVisibility(buttons[DOWN_BT], 0);
                    }
                }
            }                        
        }

        /// <summary>
        /// chekcs all the buttons one the screen to see if they are being pressed
        /// </summary>
        /// <param name="mouse"></param> MouseState for the position and state of the mouse
        /// <param name="prevMouse"></param> boolean for whether the mouse was clicked last update
        /// <param name="mousePos"></param> Vector2 for a different type of position for the mouse
        public void CheckButtons(MouseState mouse, bool prevMouse, Vector2 mousePos)
        {
            //checks if each button is being pressed and executes their function
            for (int i = 0; i < buttons.Length; i++)
            {
                //if a button gets pressed its logic is executed
                if (buttons[i].UpdateButton(mouse, prevMouse, mousePos) == true)
                {                                       
                    //checks which button was pressed
                    switch (i)
                    {
                        case DOWN_BT:
                            //decreases the viewed level
                            viewedLvl--;
                            break;

                        case UP_BT:
                            //increases the viewed level and updates the other button again
                            viewedLvl++;
                            break;

                        default:
                            //if there is enough of each resource to build the tower or building it gets built
                            if (buttons[i].WoodCost <= resources[WOOD].Amount && buttons[i].StoneCost <= resources[STONE].Amount)
                            {
                                //calls a method to make all buttons invisible
                                MakeAllButtonsInvisible();

                                //sets placing to true and the selected building or tower to the building the user clicked on
                                levels[viewedLvl].Placing = true;
                                selectedTower = i;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// switches to the next level of the player
        /// </summary>
        public void NextLvl()
        {
            //if the current level was not the last one it gets switched otherwise the game is over
            if (levels.Count > 1)
            {
                //tells the lost level to clear itself
                levels[0].ClearLvl(offEnemies);

                //removes the lost level
                levels.RemoveAt(0);
            }
            else
            {
                //ends the game
                gameOver = true;
            }
        }

        /// <summary>
        /// generates the 2 resources
        /// </summary>
        /// <param name="gameTime"></param> game time variable that allows for the timer to work
        public void GenerateResources(GameTime gameTime)
        {
            //generates both resources
            for (int i = 0; i < resources.Length; i++)
            {
                //generates resources
                resources[i].GenerateReseource(gameTime, levels.Count);
            }            
        }

        /// <summary>
        /// updates logic while the game is paused
        /// </summary>
        /// <param name="kb"></param> KeyboardState for the current state of the keyboard
        /// <param name="prevKb"></param> KeyboardState for the state of the keyboard in the previous update
        /// <param name="mouse"></param> MosueState for the state of the mouse
        /// <param name="prevMouse"></param> bool for the state of the mouse last update
        /// <param name="mousePos"></param> Vector2 for the position of the mouse
        public void UpdatePause(KeyboardState kb, KeyboardState prevKb, MouseState mouse, bool prevMouse, Vector2 mousePos)
        {
            //if the menu button is pressed the game goes back to the menu otherwise if the escape or the resume button is pressed then the game goes back to the gameplay
            if (menuBt.UpdateButton(mouse, prevMouse, mousePos) == true)
            {
                //changes the gamestate to the menu
                Game1.GameState = Game1.MENU;
            }
            else if (kb.IsKeyDown(Keys.Escape) && kb != prevKb || resumeBt.UpdateButton(mouse, prevMouse, mousePos))
            {
                //unpauses the game
                paused = false;
            }
        }

        /// <summary>
        /// makes all the active buttons invisible
        /// </summary>
        public void MakeAllButtonsInvisible()
        {
            //makes sure all the buttons are invisible while a building is being placed
            for (int i = 0; i < buttons.Length; i++)
            {
                //if the button is visible make it invisible
                if (buttons[i].Visible == true)
                {
                    //makes the button invisible and unselected
                    buttons[i].Visible = false;
                    buttons[i].Selected = false;
                }
            }
        }

        /// <summary>
        /// makes all the buttons visible
        /// </summary>
        public void MakeAllButtonsVisible()
        {
            //if the buttons are invisible makes them visible
            for (int i = 0; i < buttons.Length; i++)
            {
                //if the button is invisible makes it visible
                if (buttons[i].Visible == false)
                {
                    //makes the button visible
                    buttons[i].Visible = true;
                }
            }
        }

        /// <summary>
        /// sets the minutes and seconds on the clock
        /// </summary>
        public void SetTime()
        {
            //sets the number of seconds
            seconds = (int)(totalTime.GetTimePassed() / 1000) - minutes * Game1.MINUTE;

            //if a minute has passed it changes the displayed time
            if (seconds >= Game1.MINUTE)
            {
                //increases the number of minutes and resets the seconds
                minutes++;
                seconds -= Game1.MINUTE;
            }
        }

        /// <summary>
        /// sets the visibility of the arrow button
        /// </summary>
        /// <param name="button"></param> the arrow button
        /// <param name="levelNum"></param>
        public void SetArrowBtVisibility(Button button, int levelNum)
        {
            //if the viewed level is not the last one it updates the button that views upper levels
            if (viewedLvl == levelNum)
            {
                //if the arrow is currently visible makes it invisible
                if (button.Visible == true)
                {
                    //makes the arrow invisible
                    button.Visible = false;
                }
            }
            else
            {
                //if the button is invisible makes it visible
                if (button.Visible == false)
                {
                    //makes the arrow button visible
                    button.Visible = true;
                }
            }            
        }

        /// <summary>
        /// draws the game
        /// </summary>
        /// <param name="spriteBatch"></param> instance of SpriteBatch that allows for drawing
        public void DrawPlay(SpriteBatch spriteBatch)
        {
            //draws the currently selected level
            levels[viewedLvl].DrawLvl(spriteBatch);

            //draws the background of the UI
            spriteBatch.Draw(bgImg, bgRec, Color.White);
                        
            //loops throug the buttons and draws them all 
            for (int i = 0; i < buttons.Length; i++)
            {
                //draws a button
                buttons[i].DrawButton(spriteBatch);
            }

            //displays all the resource information
            for (int i = 0; i < resources.Length; i++)
            {
                //draws the resource 
                resources[i].DisplayRes(spriteBatch);
            }

            //draws the timer on the screen
            spriteBatch.DrawString(timerFont, "Time: " + minutes + ":" + seconds.ToString().PadLeft(2, '0'), timerPos, Color.White);

            //calls the font to draw itself
            score.DrawScore(spriteBatch);

            levels[viewedLvl].DrawTimer(spriteBatch);

            //if the game is paused it draws the paused buttons
            if (paused)
            {
                //draws the return to menu button and the resume button
                menuBt.DrawButton(spriteBatch);
                resumeBt.DrawButton(spriteBatch);
            }
        }
    }
}
  