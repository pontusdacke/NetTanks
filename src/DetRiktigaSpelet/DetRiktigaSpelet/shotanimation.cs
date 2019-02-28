using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
namespace DetRiktigaSpelet
{
    class shotanimation : GameObj
    {
        public shotanimation()
        {
            Time = 0;
            Frame = 0;
            Active = true;
            AnimationSpeed = 30;
            Angle = 0;
        }
 
        public int Time
        {
            get;
            set;
        }
        public int Frame
        {
            get;
            set;
        }
        public int AnimationSpeed
        {
            get;
            set;
        }
        public bool Active
        {
            get;
            set;
        }
        public void Update(GameTime gameTime)
        {
            Time += gameTime.ElapsedGameTime.Milliseconds;
            if (Time > AnimationSpeed)
            {
                Time = 0;
                Frame++;
                if (Frame > 4)
                {
                    Active = false;
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 DrawOffset, float levelHeight)
        {
            //Rectangle tmp = new Rectangle((Frame % 4) * 64, (Frame / 4) * 64, 64, 64);
 
            //spriteBatch.Draw(Gfx, 
            //    Position - DrawOffset + new Vector2(400, 300), 
            //    tmp, Color.White, base.Angle,
            //    new Vector2(32, 32), 1.0f, SpriteEffects.None, 0);
            Rectangle tmp = new Rectangle((Frame % 5) * 11, (Frame / 5) * 11, 11, 11);
            spriteBatch.Draw(Gfx,
                Position - DrawOffset + new Vector2(400, 300),
                tmp, Color.White, base.Angle3,
                new Vector2(11/2, 11/2), 1.0f, SpriteEffects.None, (1 / levelHeight) * (float)Position.Y);
        }
    }
}
