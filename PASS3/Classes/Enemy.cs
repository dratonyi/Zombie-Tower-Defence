/* Author: Dani Ratonyi
 * File name: Enemy.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: class for the enemies including zombies bats and big zombies
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
using MonoGame.Extended;
using Animation2D;

namespace PASS3
{
    class Enemy
    {
        //constants for the width and height of an enemy
        public const int HEIGHT = 50;
        public const int WIDTH = 50;

        //constants for the directions of the enemy
        public const int UP = -1;
        public const int DOWN = 1;
        public const int RIGHT = 1;
        public const int LEFT = -1;
        
        //constant for the max health of the enemy
        public const int MAX_HEALTH = 100;

        //constant for the speed of the enemy
        public const int MAX_SPEED = 50;

        //constant for the special type of enemy
        public const int BIG = 3;

        //variables for the maximum x and y speed
        private float maxXSpeed;
        private float maxYSpeed;

        //the speed in one update
        private float xSpeed;
        private float ySpeed;

        //the direction the enemy is moving in which is innitially up
        private int xDir = 1;
        private int yDir = 1;

        //variable for the position of the enemy
        protected Vector2 pos;       

        //variable for the rectangle around the enemy
        private Rectangle enemyRec;

        //variable for the health of the enemy
        private int health = MAX_HEALTH;

        //variable for the type of enemy air or ground
        private int type;

        //variables for the texture of the enemy
        private Texture2D enemyImgY;
        private Texture2D enemyImgX;
        private Animation enemyAnimY;
        private Animation enemyAnimX;

        private GameRectangle healthBar;        

        //variable for which element in the array of siginificant points this enemy is going towards
        private int targetPtNum = 0;

        /// <summary>
        /// constructs an instance of an enemy
        /// </summary>
        /// <param name="type"></param> integer that indicates the type of enemy
        /// <param name="content"></param> ContentManager instance that allows for loading images and text
        public Enemy(int type, ContentManager content)
        {
            //sets the type of the enemy based on a random number
            this.type = type;

            //creates the destination rectangle of the enemy
            enemyRec = new Rectangle(0, 0, 50, 50);

            //creates the vector2 position of the enemy
            pos = new Vector2();

            healthBar = new GameRectangle(Game1.Gd, new Rectangle());

            //if the type is ground then loads an image of a ground enemy else it loads an image of an air enemy
            if (type < 5)
            {
                //sets the type of the enemy
                this.type = Play.GROUND;
                
                //loads the x and y spriteSheet of the zombie
                enemyImgY = content.Load<Texture2D>("Images/Animations/zombieMovingY");
                enemyImgX = content.Load<Texture2D>("Images/Animations/zombieMovingX");
                
                //instatiates the x and y animation of the enemy
                enemyAnimY = new Animation(enemyImgY, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.35f, true);
                enemyAnimX = new Animation(enemyImgX, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.35f, false);
            }
            else if (type < 10)
            {
                //sets the type of the enemy
                this.type = Play.AIR;
                
                //loads the x and y spriteSheet of the bird
                enemyImgY = content.Load<Texture2D>("Images/Animations/birdAnimY");
                enemyImgX = content.Load<Texture2D>("Images/Animations/birdAnimX");

                //instatiates the x and y animation of the enemy
                enemyAnimY = new Animation(enemyImgY, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.61f, true);
                enemyAnimX = new Animation(enemyImgX, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.61f, false);
            }
            else if (type == 10)
            {
                //sets the type of the enemy
                this.type = BIG;
                
                //loads the x and y spriteSheet of the bird
                enemyImgY = content.Load<Texture2D>("Images/Animations/bigZombieAnimy");
                enemyImgX = content.Load<Texture2D>("Images/Animations/bigZombieAnimX");

                //instatiates the x and y animation of the enemy
                enemyAnimY = new Animation(enemyImgY, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.35f, true);
                enemyAnimX = new Animation(enemyImgX, 4, 1, 4, 1, 1, Animation.ANIMATE_FOREVER, 10, pos, 1.35f, false);
            }
        }

        /// <summary>
        /// activates an enemy by moving it to the proper position on the map
        /// </summary>
        /// <param name="sigPts"></param>
        public void ActivateEnemy(List<Vector2> sigPts, int health)
        {
            //sets the position of the newly activated enemy's real time position and its destination rectangle
            pos.X = sigPts[0].X - WIDTH / 2;
            pos.Y = sigPts[0].Y + Play.TILE_WIDTH;
            enemyRec.X = (int)pos.X;
            enemyRec.Y = (int)pos.Y;

            healthBar.Rec = new Rectangle((int)pos.X, (int)pos.Y - 10, 50, 10);

            //if the enemy is big then sets its health to twice the health of a normal enemy
            if (type == BIG)
            {
                //sets the health to twice the health of a normal enemy
                this.health = health * 2;
            }
            else
            {
                //sets the health of the enemy
                this.health = health;
            }
           
            //resets the enemy's target position
            targetPtNum = 0;

            //sets the initial position of the animated enemy
            enemyAnimY.destRec.X = (int)pos.X;
            enemyAnimY.destRec.Y = (int)pos.Y;

            //sets the speed of the enemy
            SetSpeed(sigPts[targetPtNum]);
        }

        /// <summary>
        /// Updates the logic of the enemy
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance to time the movement
        /// <param name="sigPts"></param> a list of significant points in the path the enemy is moving towards
        public void UpdateEnemy(GameTime gameTime, List<Vector2> sigPts)
        {
            //updates the animations of the enemy
            enemyAnimY.Update(gameTime);
            enemyAnimX.Update(gameTime);

            //if the enemy has reached a significant point start moving it towards the next one
            if (CheckForPtReached(sigPts[targetPtNum]) == true)
            {                
                //increases the number of the targeted signicicant point
                targetPtNum++;

                //if the target point is not the last one its gets switched
                if (targetPtNum != sigPts.Count)
                {
                    //sets the speed towards the next significant point
                    SetSpeed(sigPts[targetPtNum]);
                }                
            }

            //calls a method to move the enemy
            MoveEnemy(gameTime);
        }

        /// <summary>
        /// checks if the enemy reached the end of the path
        /// </summary>
        /// <param name="sigPts"></param> the array of significant points
        /// <returns></returns> true if the enemy reached the end false if it didnt
        public bool ReachEnd(List<Vector2> sigPts)
        {
            //if the enemy reached the end of the path returns true
            if (targetPtNum == sigPts.Count)
            {
                //returns true
                return true;
            }

            //returns false
            return false;
        }

        /// <summary>
        /// checks whether the enemy is dead
        /// </summary>
        /// <returns></returns> returns true if the enemy is dead 
        public bool CheckIfDead()
        {
            //if the enemy has health return false
            if (health <= 0)
            {
                //returns false
                return true;
            }

            //returns false
            return false;
        }

        /// <summary>
        /// property that returns the type of the enemy
        /// </summary>
        public int Type
        {
            get { return type; }
        }

        /// <summary>
        /// property that returns the rectangle around the enemy
        /// </summary>
        public Rectangle Rec
        {
            get { return enemyRec; }
        }

        /// <summary>
        /// sets the x and y speed of the enemy given a point it is supposed to move towards
        /// </summary>
        /// <param name="targetPos"></param> a given position the enemy will move towards
        private void SetSpeed(Vector2 targetPos)
        {
            //sets the y direction of the enemy
            if (targetPos.Y > pos.Y)
            {
                //sets the y direction to positive
                yDir = DOWN;                
            }
            else
            {
                //sets the y direction to negative
                yDir = UP;
            }

            //sets the x direction of the enemy
            if(targetPos.X > pos.X)
            {
                //sets the x direction to positive
                xDir = RIGHT;
            }
            else
            {
                //sets the x direction to negative
                xDir = LEFT;
            }

            //finds the angle between the current position and the target position
            double angle = Math.Atan2(Math.Abs((targetPos.Y - HEIGHT / 2) - pos.Y), Math.Abs((targetPos.X - WIDTH / 2) - pos.X));

            //finds the x and y speed using trigonometry
            maxYSpeed = (float)(MAX_SPEED * Math.Sin(angle));
            maxXSpeed = (float)(MAX_SPEED * Math.Cos(angle));

            //if the distance in the y direction is greater the enemy will look like its moving in the y direction otherwise it will move in the x direction
            if (maxYSpeed > maxXSpeed)
            {
                // makes sure the y animation is on
                if (enemyAnimY.isAnimating == false)
                {
                    //turns the y animation on
                    enemyAnimY.isAnimating = true;
                }   
                
                //makes sure the x animation is off
                if(enemyAnimX.isAnimating == true)
                {
                    //turns the x animation off
                    enemyAnimX.isAnimating = false;
                }
            }
            else
            {
                //makes sure the y animation if off
                if (enemyAnimY.isAnimating == true)
                {
                    //turns the y animation off
                    enemyAnimY.isAnimating = false;
                }

                //makes sure the x animation is on
                if(enemyAnimX.isAnimating == false)
                {
                    //turns the x animation on
                    enemyAnimX.isAnimating = true;
                }
            }
        }

        /// <summary>
        /// checks whether the enemy has reached a significant point in the path
        /// </summary>
        /// <param name="targetPos"></param> the target position the enemy is currently moving towards
        /// <returns></returns> returns whether the enemy has reached a significant point in the path
        private bool CheckForPtReached(Vector2 targetPos)
        {                                    
            //if the enemy has reached the target position returns true
            if (Helper.Util.Intersects(enemyRec, new Vector2(targetPos.X, targetPos.Y)) == true)
            {               
                //returns true
                return true;
            }

            //returns false
            return false;
        }

        /// <summary>
        /// moves the enemy
        /// </summary>
        /// <param name="gameTime"></param> GameTime instance for timing the change in speed
        private void MoveEnemy(GameTime gameTime)
        {
            //moves the position of the enemy
            xSpeed = xDir * (maxXSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            ySpeed = yDir * (maxYSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            pos.X = pos.X + xSpeed;
            pos.Y = pos.Y + ySpeed;
            enemyRec.X = (int)pos.X;
            enemyRec.Y = (int)pos.Y;

            healthBar.TranslateTo((int)pos.X, (int)pos.Y);

            //moves the animation that is currently on
            if (enemyAnimY.isAnimating == true)
            {
                //moves the y animation
                enemyAnimY.destRec.X = (int)pos.X;
                enemyAnimY.destRec.Y = (int)pos.Y;
            }
            else
            {
                //moves the x animation
                enemyAnimX.destRec.X = (int)pos.X;
                enemyAnimX.destRec.Y = (int)pos.Y;
            }
        }     

        /// <summary>
        /// property that returns the position of the enemy
        /// </summary>
        public Vector2 Pos
        {
            get { return pos; }
        }

        /// <summary>
        /// damages the enemy by the given amount of damage
        /// </summary>
        /// <param name="damage"></param> the amount of damage that will be dealt to the enemy
        public void DamageEnemy(int damage)
        {
            //reduces the health of the enemy
            health -= damage;
        }

        /// <summary>
        /// draws the enemy
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch instance that allows for drawing
        public virtual void DrawEnemy(SpriteBatch spriteBatch)
        {
            
            healthBar.Draw(spriteBatch, Color.Red, true);

            //if the y animation is on it draws the proper animation in the y direction
            if (enemyAnimY.isAnimating == true)
            {
                //draws the animation up or down depending on the direction
                switch (yDir)
                {
                    case UP:
                        //draws the animation upwards
                        enemyAnimY.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                        break;

                    case DOWN:
                        //draws the animation downwards
                        enemyAnimY.Draw(spriteBatch, Color.White, Animation.FLIP_VERTICAL);
                        break;
                }
            }
            else
            {
                //draws the animation left or right depending on the direction
                switch (xDir)
                {
                    case RIGHT:
                        //draws the animation to the right
                        enemyAnimX.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                        break;

                    case LEFT:
                        //draws the animation to the left
                        enemyAnimX.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                        break;
                }
            }            
        }
    }
}
