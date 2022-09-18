/* Author: Dani Ratonyi
 * File name: Scoreboard.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for the scoreboard
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
using System.IO;

namespace PASS3
{
    class Scoreboard
    {
        //constant for the maximum length of a search
        public const int SEARCH_LIMIT = 10;
        
        //list for the players on the scoreboard
        private List<Player> players = new List<Player>();

        //variables for the background of the scoreboard
        private Texture2D scoreBgImg;
        private Rectangle scoreBgRec;

        //variable that allows the scoreboard to write to the scoreboard file
        private StreamWriter outFile;

        //button for when the user wants to search up a name
        private Button searchBt = new Button(true);

        //button for clearing the scoreboard
        private Button clearBt = new Button(true);

        //variables for the score title
        private Texture2D scoreTitleImg;
        private Rectangle scoreTitleRec;

        //variables for the search bar
        private Texture2D searchBarImg;
        private Rectangle searchBarRec;

        //variable for the font of the scoreboard
        private SpriteFont scoreFont;
        private Vector2 displayPos;

        //variables for the search bar
        private SpriteFont searchFont;
        private Vector2 searchPos;
        private string search = "";
        private bool isSearching = false;

        //array for the keys pressed by the user each update
        private Keys[] keys;

        //string for the letters that can be entered to get a name
        private string allowedLetters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// constructs the scoreboard
        /// </summary>
        /// <param name="content"></param> contentManager instance to load content
        public Scoreboard(ContentManager content)
        {
            //loads the image of the scoreboard background and its rectangle
            scoreBgImg = content.Load<Texture2D>("Images/Backgrounds/scoreboardBg");
            scoreBgRec = new Rectangle(Game1.WIDTH / 2 - (int)(scoreBgImg.Width * 0.8f / 2), Game1.HEIGHT / 2 - (int)(scoreBgImg.Height * 0.8f / 2), (int)(scoreBgImg.Width * 0.8f), (int)(scoreBgImg.Height * 0.8f));

            //loads the scorefont
            scoreFont = content.Load<SpriteFont>("Fonts/scoreboardFt");

            //loads the image of the score title and its rectangle
            scoreTitleImg = content.Load<Texture2D>("Images/Sprites/scoreBt");
            scoreTitleRec = new Rectangle(Game1.WIDTH / 2 - (int)(scoreTitleImg.Width * 0.7f / 2), Game1.HEIGHT / 2 - scoreBgRec.Height / 2 + scoreBgRec.Height / 40, (int)(scoreTitleImg.Width * 0.7f), (int)(scoreTitleImg.Height * 0.7f));

            //loads the font of the search bar
            searchFont = content.Load<SpriteFont>("Fonts/searchFt");

            //loads the image of the search bar and its rectangle
            searchBarImg = content.Load<Texture2D>("Images/Sprites/searchBar");
            searchBarRec = new Rectangle(Game1.WIDTH / 2 - (int)(searchBarImg.Width / 2.5 / 2), Game1.HEIGHT / 2 - (int)(scoreBgRec.Height / 3.75f), (int)(searchBarImg.Width / 2.5), (int)(searchBarImg.Height / 2.5));
            searchPos = new Vector2(searchBarRec.X + 10, searchBarRec.Y + 10);
            
            //sets the position of the scores
            displayPos = new Vector2(Game1.WIDTH / 2 - scoreBgRec.Width / 2.5f, Game1.HEIGHT / 2 - scoreBgRec.Height / 5.5f);

            //loads the image and rectangle of the search button
            searchBt.LoadButtonImg("Images/Sprites/searchBt", "Images/Sprites/searchBtSel", content);
            searchBt.LoadButtonRec(searchBarRec.X + searchBarRec.Width - (int)(searchBt.ImgWidth / 2.6), Game1.HEIGHT / 2 - (int)(scoreBgRec.Height / 3.75f), (int)(searchBt.ImgWidth / 2.6), (int)(searchBt.ImgWidth / 2.6));

            clearBt.LoadButtonImg("Images/Sprites/clearBt", "Images/Sprites/clearBtSel", content);
            clearBt.LoadButtonRec(Game1.WIDTH - (int)(clearBt.ImgWidth * 0.55f) - Play.TILE_WIDTH / 4, Play.TILE_WIDTH / 4, (int)(clearBt.ImgWidth * 0.55f), (int)(clearBt.ImgHeight * 0.55f));
        }

        /// <summary>
        /// updates the logic in the scoreboard
        /// </summary>
        /// <param name="kb"></param> Keyboard state for the current state of the kyboard
        /// <param name="prevKb"></param> Keyboard state for the state of the keyboard in the last update
        /// <param name="mouse"></param> mouse state for the current state of the mouse
        /// <param name="prevMouse"></param> boolean for whether the mouse has been clicked in the last update
        /// <param name="mousePos"></param> vector2 for the position of the mouse
        public void UpdateScoreboard(KeyboardState kb, KeyboardState prevKb, MouseState mouse, bool prevMouse, Vector2 mousePos)
        {
            //if the search bar is empty and the player is currently searching than ends the search
            if (search == "" && isSearching)
            {
                //loops through the players and unsearches the searched up ones
                for (int i = 0; i < players.Count; i++)
                {
                    //if the player is searched up then it stops searching it
                    if (players[i].Searched)
                    {
                        //stops searching the player
                        players[i].Searched = false;

                        //stops looping
                        break;
                    }
                }
                
                //stops searching
                isSearching = false;
            }
            
            //gets keyboard input
            search = GetKeyboardInput(search, kb, prevKb);

            if (searchBt.UpdateButton(mouse, prevMouse, mousePos) == true || kb.IsKeyDown(Keys.Enter) == true)
            {
                //calls a method to search up the typed in name
                SearchName();
            }
            else if(clearBt.UpdateButton(mouse, prevMouse, mousePos))
            {
                //clears the scoreboard
                ClearBoard();
            }
        }

        /// <summary>
        /// gets keyboard input from the user
        /// </summary>
        /// <param name="input"></param> string input for what the input will be saved to
        /// <param name="kb"></param> Keyboard state for the current state of the kyboard
        /// <param name="prevKb"></param> Keyboard state for the state of the keyboard in the last update
        /// <returns></returns>
        public string GetKeyboardInput(string input, KeyboardState kb, KeyboardState prevKb)
        {
            //if there is space in the search bar checks for keyboard input
            if (kb != prevKb)
            {
                //gets all the keys that are pressed in the update
                keys = kb.GetPressedKeys();

                //if at least one button is down checks if its valid
                if(keys.Length > 0 && input.Length <= SEARCH_LIMIT)
                {
                    //if its a valid letter it gets entered into the search if the backspace was pressed a letter gets deleted and if space gets pressed a space is added
                    if (allowedLetters.Contains(keys[0].ToString().ToLower()))
                    {
                        //adds the letter to the search
                        return input + keys[0].ToString();
                    }
                    else if(keys[0] == Keys.Space)
                    {
                        //adds a space to the search
                        return input.PadRight(input.Length + 1);
                    }                  
                }

                //if backspace is pressed a letter gets deleted
                if (keys.Length > 0 && keys[0] == Keys.Back && input.Length > 0)
                {
                    //removes a letter from the search
                    return input.Remove(input.Length - 1);
                }
            }

            //returs the input without any changes
            return input;
        }

        /// <summary>
        /// searches for a player on the leaderboard based on a name
        /// </summary>
        private void SearchName()
        {
            //loops through the leaderboard and looks for the searched player
            for (int i = 0; i < players.Count; i++)
            {
                //if the searched name is the same as the name of a player on the leaderboard that player gets selected
                if (players[i].Name.Equals(search))
                {
                    //sets the selected player to searched
                    players[i].Searched = true;

                    //sets is searching to true
                    isSearching = true;

                    //stops looking for players
                    break;
                }
            }
        }

        /// <summary>
        /// adds a player to the scoreboard
        /// </summary>
        /// <param name="player"></param> variable for the player thats gonna be added
        public void AddPlayer(Player player)
        {
            //reads the scoreboard
            ReadBoard();
            
            //adds the player to the scoreboard list
            players.Add(player);

            //sorts the board after a new player gets entered and updates the file
            SortBoard();

            //writes the changes to the scoreboard
            WriteScoreboard();
        }

        /// <summary>
        /// reads the players on the scoreboard file
        /// </summary>
        public void ReadBoard()
        {
            //variable for the size of the scoreboard
            int size;

            //variable for the array of info in each line of the file
            string[] info;

            //tries to open the file and if it cant it does nothing
            try
            {
                //opens the scoreboard file
                Game1.InFile = File.OpenText("scoreboard.txt");

                players.Clear();

                //reads the size of the scoreboard from the file
                size = Convert.ToInt32(Game1.InFile.ReadLine());

                for (int i = 0; i < size; i++)
                {
                    //reads a line from the file
                    info = Game1.InFile.ReadLine().Split(',');

                     //adds a player to the list of players
                    players.Add(new Player(info[1], Convert.ToInt32(info[2])));
                }

                //closes the file
                Game1.InFile.Close();
            }
            catch (Exception)
            {
                //do nothing
            }            
        }

        /// <summary>
        /// writes the new scoreboard to the file
        /// </summary>
        private void WriteScoreboard()
        {
            //tries to open the file
            try
            {
                //opens the scoreboard file
                outFile = File.CreateText("scoreboard.txt");

                //writes the number of players on the first line
                outFile.WriteLine(players.Count);

                //loops through all the players on the list and puts them on the scoreboard
                for (int i = 0; i < players.Count; i++)
                {
                    //writes the player's info onto one line of the scoreboard file
                    outFile.WriteLine((i + 1) + "," + players[i].Name + "," + players[i].Score);
                }

                //closes the file
                outFile.Close();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// checks if the given name exists in the leaderboard
        /// </summary>
        /// <param name="name"></param> the name that is being compared to the leaderboard
        /// <returns></returns> returns true if it exists
        public bool CheckForDuplicates(string name)
        {
            //reads the scoreboard
            ReadBoard();
            
            //loops through the leaderboard to check for duplicate names
            for (int i = 0; i < players.Count; i++)
            {
                //if the given name nmathes a name in the scoreboard returns true
                if (players[i].Name.Equals(name))
                {
                    //returns true
                    return true;
                }
            }

            //returns false
            return false;
        }

        /// <summary>
        /// sorts the scoreboard using insert sort algorithm
        /// </summary>
        private void SortBoard()
        {
            //if there are more than 1 players in the scoreboard it sorts it
            if (players.Count > 1)
            {
                //creates a temporary swap variable
                Player temp;

                //loops through each player on the board and puts them in the right place
                for (int i = 1; i < players.Count; i++)
                {
                    //sets the swap variable to the currently selected player
                    temp = players[i];

                    //loops through all the players 
                    for (int j = i - 1; j >= 0; j--)
                    {
                        //if the temporary player has a smaller score than the next player that player gets placed into that spot otherwise it moves on to the next player
                        if (temp.Score < players[j].Score)
                        {
                            //places the temporary player variable in the right stop and stops looping
                            players[j + 1] = temp;
                            break;
                        }
                        else
                        {
                            //moves on to the next player on the leaderboard
                            players[j + 1] = players[j];

                            //if there are no more players than the temporary variable gets placed at the front of the board
                            if (j == 0)
                            {
                                //plces the temporary player to the front of the board
                                players[0] = temp;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// clears the scoreboard
        /// </summary>
        private void ClearBoard()
        {
            //clears the scoreboard
            players.Clear();

            //updates the scoreboard file
            WriteScoreboard();
        }

        /// <summary>
        /// property that returns searchFont
        /// </summary>
        public SpriteFont SearchFont
        {
            get { return searchFont; }
        }

        /// <summary>
        /// draws the scoreboard
        /// </summary>
        /// <param name="spriteBatch"></param> spritebatch that allows for drawing
        public void DrawBoard(SpriteBatch spriteBatch)
        {
            //draws the background of the score menu
            spriteBatch.Draw(scoreBgImg, scoreBgRec, Color.White);

            //draws the title of the score menu
            spriteBatch.Draw(scoreTitleImg, scoreTitleRec, Color.White);

            //draws the search bar and the search button
            spriteBatch.Draw(searchBarImg, searchBarRec, Color.White);
            searchBt.DrawButton(spriteBatch);

            clearBt.DrawButton(spriteBatch);
           
            if(search != null)
            {
                spriteBatch.DrawString(searchFont, search, searchPos, Color.Black);
            }
            
            //if a player is searched up then it displays that name otherwise it displays the top 5 positions
            if(isSearching)
            {
                //loops through the players to see if they are searched up and displays their score
                for (int i = 0; i < players.Count; i++)
                {
                    //if the player is searched up then it displays their score
                    if (players[i].Searched)
                    {
                        //diplays the info of the player
                        spriteBatch.DrawString(scoreFont, (i + 1) + ". " + players[i].Display, new Vector2(displayPos.X, displayPos.Y), Color.Cyan);

                        //stops looking for searched up players
                        break;
                    }
                }
            }
            else
            {
                //loops through the top 5 positions and displays them on the screen
                for (int i = 0; i < Math.Min(players.Count, 5); i++)
                {
                    //displays a position on the leaderboard
                    spriteBatch.DrawString(scoreFont, (i + 1) + ". " + players[i].Display, new Vector2(displayPos.X, displayPos.Y + i * 65), Color.Cyan);
                }
            }                       
        }
    }
}
