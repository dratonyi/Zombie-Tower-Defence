/* Author: Dani Ratonyi
 * File name: Score.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for the score of the player
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
    class Score
    {
        //variable for the amount of score
        private int amount = 0;

        //variable for the position of the score
        private Vector2 pos;
        private Vector2 endPos;

        //variable for the font of the score
        private SpriteFont font;

        /// <summary>
        /// constructs an instance of the player's score
        /// </summary>
        /// <param name="position"></param> Vecotr2 for the position of the score's display
        /// <param name="font"></param> SpriteFont for the font it appears with
        public Score(ContentManager content, Vector2 position)
        {
            //sets the position of the score
            pos = position;

            //sets the end game position of the score
            endPos = new Vector2(Game1.WIDTH / 2 - Game1.WIDTH / 12, Game1.HEIGHT / 2);

            //sets the font
            font = content.Load<SpriteFont>("Fonts/scoreFt");
        }

        /// <summary>
        /// resets the amount of score the player has
        /// </summary>
        public void ResetScore()
        {
            //sets the amount of the score to 0
            amount = 0;
        }

        /// <summary>
        /// property that can return and set the amount of score the player has
        /// </summary>
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        /// <summary>
        /// displays the score
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch instance that allows for drawing 
        public void DrawScore(SpriteBatch spriteBatch)
        {
            switch (Game1.GameState)
            {
                case Game1.PLAY:
                    //draws the score
                    spriteBatch.DrawString(font, "Score: " + Amount, pos, Color.Yellow);
                    break;

                case Game1.END_GAME:
                    //draws the score in the end game
                    spriteBatch.DrawString(font, "Score: " + Amount, endPos, Color.Yellow);
                    break;
            }                     
        }

        /// <summary>
        /// displays the score in the end game state
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch that allows for drawing
        /*public void DrawEndGameScore(SpriteBatch spriteBatch)
        {
            //draws the 
            spriteBatch.DrawString(font, "Score: " + Amount, endPos, Color.Yellow);
        }*/
    }
}
