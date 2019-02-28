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
    class MovingGameObj : GameObj
    {
        public Vector2 Direction //Riktning 
        {
            get;
            set;
        }
        public Vector2 ShotDirection //Riktning for skotten
        {
            get;
            set;
        }
        public Vector2 ShotDirection2 //Riktning for skotten
        {
            get;
            set;
        }
        public float Speed //Hastighet
        {
            get;
            set;
        }
        public virtual void Update(GameTime gameTime)
        {
            Position += Direction * Speed;
        }
    }
}
