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
    class walls : MovingGameObj
    {
        public walls()
        {
            skottkollision = true;
            skottisover = false;
        }
        public int bild
        {
            get;
            set;
        }
        public int bildTop
        {
            get;
            set;
        }
        public bool skottkollision
        {
            get;
            set;
        }
        public bool wall
        {
            get;
            set;
        }
        public bool skottisover
        {
            get;
            set;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 DrawOffset, float levelHeight)
        {
           
            Rectangle tmp2 = new Rectangle((bild % 5) * 64, (bild / 5) * 64, 64, 64);
            spriteBatch.Draw(Gfx,
                Position - DrawOffset + new Vector2(400, 300),
                tmp2, Color.White, base.Angle3,
                new Vector2(32, 32), 1.0f, SpriteEffects.None, (1/levelHeight)*(float)Position.Y);
            if (wall)
            {
                Rectangle tmp3 = new Rectangle((bildTop % 5) * 64, (bildTop / 5) * 64, 64, 64);
                spriteBatch.Draw(Gfx,
                    Position - DrawOffset + new Vector2(400, 236),
                    tmp3, Color.White, base.Angle3,
                    new Vector2(32, 32), 1.0f, SpriteEffects.None, (1/levelHeight)*(float)Position.Y);
            }
        }
    }
}
