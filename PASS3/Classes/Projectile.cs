/* Author: Dani Ratonyi
 * File name: Projectile.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: the projectile that is used to kill the enemies
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
    class Projectile
    {
        //constant for the minimum distance from an enemy
        public const int MIN_DISTANCE = 20;
        
        //constant for the damage the projectile deals
        public const int DAMAGE = 25;        
        
        //constant for the max speed of the projectile
        public const int MAX_SPEED = 1000;

        //variable for the iamge of the projectile
        private Texture2D projImg;

        //vector 2 for the real time position of the projectile
        private Vector2 pos;

        //variables for the directino in the x and y dimension the projectile is moving in
        private int yDir;
        private int xDir;

        //variables for the maximum x and y speed
        private float maxXSpeed;
        private float maxYSpeed;

        //the speed in one update
        private float xSpeed;
        private float ySpeed;

        //variable for the rectangle around the projectile
        private Rectangle projRec;

        //variable for whether the projectile is active
        bool isActive = false;

        private Enemy target;

        //variable for the color of the projectile
        Color color;

        /// <summary>
        /// constructs an instance of the projectile
        /// </summary>
        /// <param name="Content"></param> ContentManager instance to load content
        /// <param name="color"></param> color of the projectile
        public Projectile(ContentManager Content, Color color)
        {
            //loads the image of the projectile
            projImg = Content.Load<Texture2D>("Images/Sprites/cannonBall");

            projRec = new Rectangle(0, 0, 10, 10);

            //sets the color of the projectile based on the type of enemy it attacks
            this.color = color;
        }

        /// <summary>
        /// updates the logic of the projectile
        /// </summary>
        /// <param name="gameTime"></param> gameTime variable that allows for the timing of the projectile's movement
        public void UpdateProj(GameTime gameTime)
        {
            //if the projectile is active it gets updated
            if (isActive)
            {
                //calls a method to move the projectile
                MoveProj(gameTime);

                //calls a method to collide the projectile
                CollideProj();

                //calls a method to check if the projectile is off the map
                CheckIfOffMap();
            }                        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startRec"></param>
        /// <param name="target"></param>
        public void ActivateProj(Rectangle startRec, Enemy target)
        {
            //sets the target enemy of this projectile
            this.target = target;
            
            //activates the projectile
            isActive = true;

            //sets the starting position of the projectile
            pos.X = startRec.X + Play.TILE_WIDTH / 2 - projRec.Width / 2;
            pos.Y = startRec.Y + Play.TILE_WIDTH / 2 - projRec.Height / 2;
            projRec.X = (int)pos.X;
            projRec.Y = (int)pos.Y;

            //calls a method to set the speed of the projectile
            SetSpeed(target);
        }

        /// <summary>
        /// checks if the projectile is off the map in case it misses the enemy
        /// </summary>
        public void CheckIfOffMap()
        {
            //if the projectile is off the map deactivates it
            if (pos.X + projRec.Width <= 0 || pos.X >= Game1.WIDTH || pos.Y + projRec.Height <= 0 || pos.Y >= Game1.HEIGHT)
            {
                //deactivates the projectile
                isActive = false;
            }
        }

        /// <summary>
        /// sets the speed of the projectile towards the target enemy
        /// </summary>
        /// <param name="target"></param> the position of the enemy that is being targeted
        public void SetSpeed(Enemy target)
        {                        
            //sets the y direction of the enemy
            if (target.Rec.Y > pos.Y)
            {
                //sets the y direction to positive
                yDir = 1;
            }
            else if(target.Rec.Y < pos.Y)
            {
                //sets the y direction to negative
                yDir = -1;
            }
            else
            {
                yDir = 0;
            }

            //sets the x direction of the enemy
            if (target.Rec.X > pos.X)
            {
                //sets the x direction to positive
                xDir = 1;
            }
            else if(target.Rec.X < pos.X)
            {
                //sets the x direction to negative
                xDir = -1;
            }
            else
            {
                xDir = 0;
            }

            //finds the angle between the current position and the target position
            double angle = Math.Atan2(Math.Abs((target.Rec.Y + Enemy.HEIGHT / 2) - (pos.Y + projRec.Height / 2)), Math.Abs((target.Rec.X + Enemy.WIDTH / 2) - (pos.X + projRec.Width / 2)));

            //finds the x and y speed using trigonometry
            maxYSpeed = (float)(MAX_SPEED * Math.Sin(angle));
            maxXSpeed = (float)(MAX_SPEED * Math.Cos(angle));

            //if the x or the y direction is really small the projoectile does not move in that dimension
            if (Math.Abs((target.Rec.X + Enemy.WIDTH / 2) - (pos.X + projRec.Width / 2)) <= MIN_DISTANCE)
            {
                //sets the x speed to 0
                maxXSpeed = 0;
            }
            else if(Math.Abs((target.Rec.Y + Enemy.HEIGHT / 2) - (pos.Y + projRec.Height / 2)) <= MIN_DISTANCE)
            {
                //sets the y speed to 0
                maxYSpeed = 0;
            }            
        }

        /// <summary>
        /// moves the projectile towards a given enemy
        /// </summary>
        /// <param name="enemy"></param> an enemy the projectile is aimed against
        public void MoveProj(GameTime gameTime)
        {
            //moves the projectile's real time position and its destination rectangle
            xSpeed = xDir * (maxXSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            ySpeed = yDir * (maxYSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            pos.X = pos.X + xSpeed;
            pos.Y = pos.Y + ySpeed;
            projRec.X = (int)pos.X;
            projRec.Y = (int)pos.Y;
        }

        /// <summary>
        /// checks whether the enemy and the projectile collide
        /// </summary>
        public void CollideProj()
        {
            //if the projectile collides with the enemy the enemy gets damaged
            if (Helper.Util.Intersects(projRec, target.Rec) == true)
            {
                //reduces the health of the enemy by the damage of the projectile
                //target.Health -= DAMAGE;
                target.DamageEnemy(DAMAGE);

                //deactivates the projectile
                isActive = false;
            }
        }

        /// <summary>
        /// property that returns whether the projectile is currently active
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <summary>
        /// draws the projectile
        /// </summary>
        /// <param name="spriteBatch"></param> SpriteBatch class that allows for drawing
        public void DrawProj(SpriteBatch spriteBatch)
        {
            //if the projectile is active it gets drawn
            if (isActive)
            {
                //draws the projectile 
                spriteBatch.Draw(projImg, projRec, color);
            }            
        }
    }
}
