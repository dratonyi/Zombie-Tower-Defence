/* Author: Dani Ratonyi
 * File name: Level.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: one level of the game 
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

namespace PASS3
{
    class Level
    {
        //2D array with each element being a tile on the map
        private Tile[,] map = new Tile[Play.NUM_ROWS, Play.NUM_COL];

        //boolean for whether a tower is being placed or not
        private bool placing = false;        

        //list for the enemies that are currently on screen
        private List<Enemy> onEnemies = new List<Enemy>();

        //creates a list that will hold the significant points where the enemies will have to turn
        private List<Vector2> sigPts = new List<Vector2>();

        //timer for the time passed in a level
        private Timer levelTimer;
        private double prevHpIncrease;
        private Vector2 timerPos;

        //variable for the initial health of each enemy
        private int enemyHealth;

        //variable for how much time passed since the last time an enemt spawned
        private double prevTime;

        //timer for when the next enemy is spawning
        private Timer enemyTimer;
        private double enemySpawnTime;        

        //font for the ui
        private SpriteFont uiFont;
        private int minutes;
        private int seconds;

        /// <summary>
        /// constructs an instance of a level
        /// </summary>
        /// <param name="content"></param> ContentManager to load content
        /// <param name="offEnemies"></param> Queue of enemies to spawn enemies
        /// <param name="uiFont"></param> Spritefont for the font of the timer
        public Level(ContentManager content, EnemyQueue offEnemies, SpriteFont uiFont)
        {
            //sets the uiFont
            this.uiFont = uiFont;

            //sets the passed minutes and seconds
            minutes = 0;
            seconds = 0;

            //calls a method to choose a random map from the files
            ChooseRandomMap(content);

            //initializes the enemy timer
            enemyTimer = new Timer(Timer.INFINITE_TIMER, true);

            //variable for the amount of time that needs to pass before an enemy is spawned
            enemySpawnTime = Play.MAX_ENEMY_TIMER;

            //creates a timer for the levels        
            levelTimer = new Timer(Timer.INFINITE_TIMER, true);

            //sets the enemy health to the minimum
            enemyHealth = 50;

            //resets the time since the previous enemy spawned
            prevTime = 0;

            //resets the time between enemy Hp increases
            prevHpIncrease = 0;

            //sets the position of the timer
            timerPos = new Vector2(Play.TILE_WIDTH * (Play.NUM_COL) + Play.TILE_WIDTH * 9 / 10, Play.TILE_WIDTH * 3 / 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mouse"></param>
        /// <param name="mousePos"></param>
        /// <param name="gameTime"></param>
        /// <param name="offEnemies"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool UpdateLvl(ContentManager content, MouseState mouse, Vector2 mousePos, GameTime gameTime, EnemyQueue offEnemies, Score score)
        {
            //updates the timers in the level
            enemyTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            levelTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //calls a method to increase the health of the enemies
            IncreaseEnemyHealth(levelTimer);

            //sets the time in minutes and seconds
            SetTime(score);

            //if its time to spawn an enemy an enemy gets spawned
            if(enemyTimer.GetTimePassed() >= enemySpawnTime)
            {
                //reseets the timer
                enemyTimer.ResetTimer(true);

                //if the enemy timer has not reached its minimum decreases it
                if (enemySpawnTime >= Play.MIN_ENEMY_TIMER)
                {
                    //reduces the spawn time of the enemies
                    enemySpawnTime -= (levelTimer.GetTimePassed() - prevTime) / 10;
                }

                //sets the amount of time that passed since the last enemy has spawned
                prevTime = levelTimer.GetTimePassed();

                //calls a method to spawn an enemy
                SpawnEnemy(offEnemies, content);
            }

            //loops through the on screen enemies
            for (int i = 0; i < onEnemies.Count; i++)
            {
                //updates an enemy
                onEnemies[i].UpdateEnemy(gameTime, sigPts);

                //if the enemy is dead it gets reused in the off enemies queue
                if(onEnemies[i].CheckIfDead() == true)
                {
                    //enques the dead enemy
                    offEnemies.Enqueue(onEnemies[i]);

                    //adds to the score
                    score.Amount+=Play.KILL_SCORE;

                    //removes  the dead enemy from the on screen enemies
                    onEnemies.RemoveAt(i);
                }
                else if(onEnemies[i].ReachEnd(sigPts) == true)
                {                                           
                    //returns true indicating that the level has ended
                    return true;
                }
            }

            //returns false
            return false;
        }

        /// <summary>
        /// spawns an enemy
        /// </summary>
        /// <param name="offEnemies"></param> a queue containing the enemies off the screen
        public void SpawnEnemy(EnemyQueue offEnemies, ContentManager content)
        {
            //dequeue's an enemy from the off screen enemies and adds it to the on screen enemies
            onEnemies.Add(offEnemies.Dequeue(content));
            
            //activates the enemy that was put on screen
            onEnemies[onEnemies.Count - 1].ActivateEnemy(sigPts, enemyHealth);
        }

        /// <summary>
        /// sets the minutes and seconds on the clock
        /// </summary>
        public void SetTime(Score score)
        {
            //sets the number of seconds
            seconds = (int)(levelTimer.GetTimePassed() / 1000) - minutes * Game1.MINUTE;

            //if a minute has passed it changes the displayed time
            if (seconds >= Game1.MINUTE)
            {
                //increases the number of minutes and resets the seconds
                minutes++;
                seconds -= Game1.MINUTE;

                //adds 50 to the score for surviving for a minute on the level
                score.Amount += Play.SURVIVE_SCORE;
            }
        }

        /// <summary>
        /// property that returns the time passed in the level
        /// </summary>
        public int Time
        {
            get { return (int)(levelTimer.GetTimePassed()); }
        }

        /// <summary>
        /// places a tile on the map
        /// </summary>
        /// <param name="type"></param> an integer for the type of tile that will be placed
        /// <param name="resources"></param> an array of resources holding the 2 types of resources wood and stone
        /// <param name="icon"></param> the icon of the type of tower that is placed
        /// <param name="mouse"></param> MouseState to check what the mouse is doing at the moment
        /// <param name="mousePos"></param> Vector2 for the position of the mouse
        /// <param name="content"></param> ContentManager instance that allows for content to be loaded into the project
        public void PlaceTower(int type, Resource[] resources, Button icon, MouseState mouse, Vector2 mousePos, ContentManager content)
        {            
            //loops through the rows on the map
            for (int row = 0; row < map.GetLength(Play.ROW); row++)
            {
                //loops through the coloumns on the map
                for (int col = 0; col < map.GetLength(Play.COL); col++)
                {
                    //if the mouse is hovering over the selected mouse it gets highlighted
                    if (Helper.Util.Intersects(map[row, col].Rec, mousePos) == true)
                    {
                        //makes the tile the mouse is hovering over selected
                        map[row, col].Selected = true;

                        // if the selected tile is clicked a tower or building gets placed on that tile
                        if (mouse.LeftButton == ButtonState.Pressed && map[row, col] is EmptyTile)
                        {
                            //places the selected building
                            switch (type)
                            {
                                case Play.GRD_TWR_ICON:
                                    //places a tower or building
                                    map[row, col] = new TowerTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content, Play.GROUND);                                    
                                    break;

                                case Play.AIR_TWR_ICON:
                                    //places a tower or building
                                    map[row, col] = new TowerTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content, Play.AIR);
                                    break;

                                case Play.MINE_TWR_ICON:
                                    //places a tower or building
                                    map[row, col] = new BuildingTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content, Play.STONE);
                                    break;

                                case Play.MILL_TWR_ICON:
                                    //places a tower or building
                                    map[row, col] = new BuildingTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content, Play.WOOD);
                                    break;
                            }

                            //takes the price of placing the building away from the total resources
                            resources[Play.WOOD].Amount -= icon.WoodCost;
                            resources[Play.STONE].Amount -= icon.StoneCost;

                            //stops placing
                            placing = false;
                        }
                    }                    
                }
            }
        }

        /// <summary>
        /// updates every single tile on this level
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance that allows for the timers to work
        /// <param name="resources"></param> an array of resources holding the 2 types of resources
        /// <param name="content"></param> ContentManager instance that allows for content to be loaded into the project
        /// <param name="mousePos"></param> a Vector2 holding the current position of the mosue
        public void UpdateTiles(GameTime gameTime, Resource[] resources, ContentManager content, Vector2 mousePos)
        {
            //loops through the map and updates all the tiles
            for (int row = 0; row < map.GetLength(Play.ROW); row++)
            {
                //loops through all the coloumns and updates the tiles
                for (int col = 0; col < map.GetLength(Play.COL); col++)
                {
                    //if the tile is a building it gets updated otherwise the tile only gets updated if its on the currently active level
                    if (map[row, col] is BuildingTile)
                    {
                        //updates the building tile
                        map[row, col].UpdateTile(gameTime, resources[map[row, col].Type]);
                    }
                    else if(map[row, col] is TowerTile)
                    {
                        //updates the towers
                        map[row, col].UpdateTile(gameTime, onEnemies, mousePos);
                    }
                }
            }
        }

        /// <summary>
        /// draws the level
        /// </summary>
        /// <param name="spriteBatch"></param> spriteBatch drawing variable
        public void DrawLvl(SpriteBatch spriteBatch)
        {            
            //loops through each row in the 2D array
            for (int row = 0; row < map.GetLength(Play.ROW); row++)
            {
                //loops through each coloumn in the 2D array
                for (int col = 0; col < map.GetLength(Play.COL); col++)
                {
                    //draws the selected tile
                    map[row, col].DrawTile(spriteBatch);
                }
            }

            //loops through each row in the 2D array
            for (int row = 0; row < map.GetLength(Play.ROW); row++)
            {
                //loops through each coloumn in the 2D array
                for (int col = 0; col < map.GetLength(Play.COL); col++)
                {
                    if (map[row, col] is TowerTile)
                    {
                        //draws the selected tile
                        map[row, col].DrawExtra(spriteBatch);
                    }                    
                }
            }

            //loops through the on enemies and draw sthem
            for (int i = 0; i < onEnemies.Count; i++)
            {
                //draws the currently selected enemy
                onEnemies[i].DrawEnemy(spriteBatch);
            }            
        }

        /// <summary>
        /// draws the level timer
        /// </summary>
        public void DrawTimer(SpriteBatch spriteBatch)
        {
            //displays the amount of time passed in this level
            spriteBatch.DrawString(uiFont, "Lvl Time: " + minutes + ":" + seconds.ToString().PadLeft(2, '0'), timerPos, Color.White);
        }

        /// <summary>
        /// emptries the level the player just lost
        /// </summary>
        /// <param name="offEnemies"></param> the queue of off enemies so the on enemies can be dumped back into the queue
        public void ClearLvl(EnemyQueue offEnemies)
        {
            
            //loops through the on enemies and enqueues them in the off enemies queue
            for (int i = 0; i < onEnemies.Count; i++)
            {
                //enqueues an on enemy
                offEnemies.Enqueue(onEnemies[i]);
            }
        }

        /// <summary>
        /// property that returns or sets the value of placing
        /// </summary>
        public bool Placing
        {
            get { return placing; }
            set { placing = value; }
        }

        /// <summary>
        /// increases the health of the enemies if a certain amount of time has passed
        /// </summary>
        /// <param name="lvlTime"></param> the Timer with the amount of time that has passed since the level began
        private void IncreaseEnemyHealth(Timer lvlTime)
        {
            //if a minute has passed increases the health of the enemies
            if (lvlTime.GetTimePassed() - prevHpIncrease >= 1000 * Game1.MINUTE * 2 / 3)
            {
                //increases the health of the enemies to one more projectile
                enemyHealth += Projectile.DAMAGE;

                //sets the previouse health increase time to now
                prevHpIncrease = lvlTime.GetTimePassed();
            }
        }

        /// <summary>
        /// chooses a random map from the file system
        /// </summary>
        /// <param name="content"></param> content manager that allows for loading the images of the tiles 
        private void ChooseRandomMap(ContentManager content)
        {
            //variable for one line in the path file
            string[] line = new string[Play.NUM_COL];

            //array for the coordinates of the significant points in the file
            string[] coor = new string[2];

            //tries to read the file with the enemy paths in them
            try
            {
                //opens a random path file in the game's files
                Game1.InFile = File.OpenText("paths/path" + Game1.Rng.Next(0, 6) + ".txt");
                //Game1.InFile = File.OpenText("paths/path5.txt");

                //loops through each row
                for (int row = 0; row < map.GetLength(Play.ROW); row++)
                {
                    //reads a line of the file
                    line = Game1.InFile.ReadLine().Split(',');
                    
                    //loops through the coloumns and adds either a path tile or an empty tile to the map
                    for (int col = 0; col < map.GetLength(Play.COL); col++)
                    {
                        //of the letter in the file is P adds a path tile if its not a P it adds an empty tile to the map
                        if (line[col] == "P")
                        {
                            //creates a tile with coordinates approporiate to its position and loads its image
                            map[row, col] = new PathTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content);
                        }
                        else
                        {
                            //creates an empty tile with the coordinates of the current position of the 2D array
                            map[row, col] = new EmptyTile(col * Play.TILE_WIDTH, row * Play.TILE_WIDTH, Play.TILE_WIDTH, content);
                        }
                    }
                }

                //reads the next line containin the significant points in the path
                line = Game1.InFile.ReadLine().Split(';');

                //loops through the line and adds each significant point to the significant points list
                for (int i = 0; i < line.Length; i++)
                {
                    //splits the coordinates to x and y
                    coor = line[i].Split(',');

                    //if the next significant point is the last one then make its Y value a little higher otherwise just add a regular significant point
                    if (i == line.Length - 1)
                    {
                        //adds a new significant point to the list of significant points with coordinates taken from the file
                        sigPts.Add(new Vector2(Convert.ToInt32(coor[1]) * Play.TILE_WIDTH + Play.TILE_WIDTH / 2, Convert.ToInt32(coor[0]) * Play.TILE_WIDTH + Play.TILE_WIDTH / 2 - Play.TILE_WIDTH));
                    }
                    else
                    {
                        //adds a new significant point to the list of significant points with coordinates taken from the file 
                        sigPts.Add(new Vector2(Convert.ToInt32(coor[1]) * Play.TILE_WIDTH + Play.TILE_WIDTH / 2, Convert.ToInt32(coor[0]) * Play.TILE_WIDTH + Play.TILE_WIDTH / 2));
                    }
                }

                //closes the file
                Game1.InFile.Close();
            }
            catch (FileNotFoundException)
            {
                //do nothign
            }
            catch(Exception)
            {
                //do nothing
            }            
        }
    }
}
