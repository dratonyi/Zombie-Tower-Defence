/* Author: Dani Ratonyi
 * File name: TowerTile.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: A tile on the 2 dimensional map of the game
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
    class TowerTile : Tile
    {
        //constant for how long a tower is alive for
        //public const int TIME_ALIVE = 30000;
        public const int FIRE_RATE = 2000;
        
        //variabgles for the attributes of the towers
        private int range = 200;       

        //timer for when the tower will despawn
        private Timer despawnTimer;

        //timer for the fire rate of the tower
        private Timer attackTimer;

        private bool hovering = false;

        //private Texture2D
        private GameCircle rangeCir;

        //the projectile of the tower 
        protected Projectile proj;

        /// <summary>
        /// constructs a tower tile
        /// </summary>
        /// <param name="x"></param> x position of the tower
        /// <param name="y"></param> y position
        /// <param name="width"></param> width of the tower
        /// <param name="Content"></param> ContentManager instance to load images
        /// <param name="type"></param> integer for the type of tower
        public TowerTile(int x, int y, int width, ContentManager Content, int type) : base(x, y, width, Content, type)
        {
            //sets the type of the tower to the given parameter
            this.type = type;

            //if the tower is ground the approporiate image gets loaded
            switch (type)
            {
                case Play.GROUND:
                    //loads the image of the crossbow tower and makes an instance of the projectile for the tower
                    tileImg = Content.Load<Texture2D>("Images/Sprites/towerImg");
                    proj = new Projectile(Content, Color.Black);
                    break;

                case Play.AIR:
                    //loads the image of the crossbow tower and makes an instance of the projectile for the tower
                    tileImg = Content.Load<Texture2D>("Images/Sprites/airTowerImg");
                    proj = new Projectile(Content, Color.Red);
                    break;
            }

            //sets the attack timer
            attackTimer = new Timer(FIRE_RATE, true);

            //creates a rectangle at the given position
            tileRec = new Rectangle(x, y, width, width);            

            rangeCir = new GameCircle(Game1.Gd, x + width / 2, y + width / 2, range, 3);

            //sets the select color to red
            selectColor = Color.Red;

            //initializes the despawn timer
            //despawnTimer = new Timer(TIME_ALIVE, true);
        }

        /// <summary>
        /// updates the logic of the tower tile
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance that allows for the timer to work
        /// <param name="enemies"></param> list of enemies the tower will be attacking
        /// <param name="mousePos"></param> the position of the mouse to see if its hovering over the tower
        public override void UpdateTile(GameTime gameTime, List<Enemy> enemies, Vector2 mousePos)
        {
            //updates the despawn timer
            //despawnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //if the fire rate timer is not at 0 it gets updated otherwise if the projectile is currently inactive the tower attacks an enemy
            if (attackTimer.IsFinished() == false)
            {
                //updates the fire rate timer
                attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            else if(proj.IsActive == false)
            {
                //attack the enemies
                AttackEnemy(enemies);
            }

            //calls a method to show the range of the tower
            ShowRange(mousePos);

            //calls a method of the projectile to update the projectile's logic
            proj.UpdateProj(gameTime);
        }         
         
        /// <summary>
        /// attacks enemies that are in range and that are the correct type
        /// </summary>
        /// <param name="enemies"></param> list of the enemies on the screen
        private void AttackEnemy(List<Enemy> enemies)
        {
            //loops through the enemies and if they are in range the tower shoots at them
            for (int i = 0; i < enemies.Count; i++)
            {
                //if an enemy is in range and if the type of tower matches the type of enemy the tower shoots at the enemy
                if (InRange(enemies[i]) == true && (enemies[i].Type == type || enemies[i].Type == Enemy.BIG) && enemies[i].Rec.Y <= Game1.HEIGHT)
                {
                    //activates the projectile towards the enemy that will be attacked
                    proj.ActivateProj(tileRec, enemies[i]);
                    
                    //resets the attack timer and exits the loop
                    attackTimer.ResetTimer(true);
                    break;
                }
            }
        }

        /// <summary>
        /// displays the range of the tower when the mouse is hovering over it
        /// </summary>
        /// <param name="mousePos"></param>
        private void ShowRange(Vector2 mousePos)
        {
            //if the mouse is over the tower the range gets displayed
            if (Helper.Util.Intersects(tileRec, mousePos))
            {
                //if the mouse isnt hovering turns of hovering
                if (!hovering)
                {
                    //turns hovering on
                    hovering = true;
                }
            }
            else
            {
                //turns hovering off if its on
                if (hovering)
                {
                    //turns off hovering
                    hovering = false;
                }
            }
        }

        /// <summary>
        /// checks whether an enemy is in range
        /// </summary>
        /// <param name="enemy"></param> enemy that is being looked at
        /// <returns></returns> returns true if the enemy is in range
        public bool InRange(Enemy enemy)
        {
            //if the given enemy is in range returns true
            if (Helper.Util.Intersects(enemy.Rec, rangeCir))
            {
                //returns true
                return true;
            }

            //returns false
            return false;
        }

        /// <summary>
        /// draws the tile
        /// </summary>
        /// <param name="spriteBatch"></param> spriteBatch that allows for drawing
        public override void DrawTile(SpriteBatch spriteBatch)
        {
            base.DrawTile(spriteBatch);
        }

        /// <summary>
        /// draws the projectile
        /// </summary> 
        /// <param name="spriteBatch"></param> spriteBatch that allows for drawing
        public override void DrawExtra(SpriteBatch spriteBatch)
        {
            //draws the projectile
            proj.DrawProj(spriteBatch);

            //if the mouse is over the tower the range gets drawn
            if (hovering)
            {
                //draws the range
                rangeCir.Draw(spriteBatch, Color.Transparent, Color.Red);
            }
        }
    }
}
