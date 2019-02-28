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
    class Meter : GameObj
    {
        public Meter()
        {
            Value = 0;
        }
        public float Value
        {
            set;
            get;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 DrawOffset, float layer)
        {
            //spriteBatch.Draw(Gfx, new Rectangle((int)base.Position.X,
            //    (int)base.Position.Y,
            //    (int)this.Value, Gfx.Height), Color.White, (1 / layer) * (float)Position.Y);
            //spriteBatch.Draw(Gfx, new Rectangle((int)base.Position.X,
            //    (int)base.Position.Y,
            //    (int)this.Value, Gfx.Height), Color.White);
            spriteBatch.Draw(Gfx,
                 new Vector2(214, 42), new Rectangle((int)base.Position.X,
                (int)base.Position.Y,
                (int)this.Value, Gfx.Height),
                Color.White, 0,
                new Vector2(Gfx.Width / 2, Gfx.Height / 2), 1.0f,
                SpriteEffects.None, layer);
            
            
            
        }
    }
}
