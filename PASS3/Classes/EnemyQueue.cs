/* Author: Dani Ratonyi
 * File name: EnemyQueue.cs
 * Project name: PASS3
 * Date Created: Dec 20th 2021
 * Date Modified: Jan 23rd 2022
 * Description: queue of the enemies that are currently off the screen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace PASS3
{
    class EnemyQueue
    {
        //list of the enemies in the queue
        private List<Enemy> enemies = new List<Enemy>();

        /// <summary>
        /// constructs an instance of the EnemyQueue
        /// </summary>
        public EnemyQueue()
        {
         
        }

        /// <summary>
        /// adds an enemy to the back of the queue
        /// </summary>
        /// <param name="enemy"></param> an Enemy that will be added to the queue
        public void Enqueue(Enemy enemy)
        {            
            //adds an enemy to the queue
            enemies.Add(enemy);
        }

        /// <summary>
        /// removes and returns the first enemy in the queue
        /// </summary>
        /// <returns></returns> the enemy that was removed from the front of the queue
        public Enemy Dequeue(ContentManager content)
        {
            //if there are no more enemies in the queue enques one more
            if (Size == 0)
            {
                //enqueues an enemy
                Enqueue(new Enemy(Game1.Rng.Next(Play.GROUND, 11), content));
            }
            
            //temporary variable for the first enemy in queue
            Enemy temp = enemies[0];

            //removes the first enemy from the queue
            enemies.RemoveAt(0);

            //reduces the size of the queue
            //size--;

            //returns the first enemy in queue
            return temp;
        }

        /// <summary>
        /// returns the top element in the queue
        /// </summary>
        /// <returns></returns> returns the enemy at the front of the queue
        public Enemy Peak()
        {
            //retuens the enemy at the front of the queue
            return enemies[0];
        }

        /// <summary>
        /// property that returns the current size of the queue
        /// </summary>
        public int Size
        {
            get { return enemies.Count; }
        }
    }
}
