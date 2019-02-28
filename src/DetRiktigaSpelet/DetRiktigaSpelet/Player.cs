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
    class Player : MovingGameObj
    {
        float timer10 = 0;
        public Player()
        {
            MaxSpeed = 7F;  //Karaktärens rörelsehastighet
            slowspeed = 3.5f;   //karaktärens hastighet vid slow, från sekundarattack
            prevKs = Keyboard.GetState();   //Tangentbordets nuvarande läge
            prevKs2 = Keyboard.GetState();
            prevMs = Mouse.GetState();  //Musens nuvarande läge (Huvudattack)
            prevMS2 = Mouse.GetState(); //musens nuvarande läge (sekundarattack)
        
            Life = 200F;
            
            Angle = (float)(Math.PI / 2);   //Sätter startvinkeln för karaktären
            ShotAngle = 0;  //Sätter  startvinkeln för skottvinkeln
            ShotAngle2 = 0;
            //VIKTIGT! Måste ändras om man ändrar startpositionen för karaktären.
            Position2 = new Vector2(400, 300);  //sätter startposition för skottvinkelns vektor.
            Position3 = new Vector2(400, 300);  //sätter startposition för skottvinkelns vektor (sekundar)
            sekundarstop = true;
            Frame = 0;
            Angle3 = 0;
            Time = 0;
            AnimationSpeed = 3;
            framediagonal = false;
            framevanlig = false;
            Kills = 0; //antal gånger man har dödat motståndaren
        }
        //vector för mittpunkt på tanken som ShotAngle fungerar runt
        public Vector2 Position2
        {
            get;
            set;
        }
        //
        public bool sekundarstop
        {
            get;
            set;
        }   
        //vector för mittpunkt på tanken som ShotAngle fungerar runt
        public Vector2 Position3
        {
            get;
            set;
        }
        public bool Enemy
        {
            get;
            set;
        }
        public float slowspeed
        {
            get;
            set;
        }
        public float MaxSpeed
        {
            get;
            set;
        }
        public float ShotPower
        {
            get;
            set;
        }
        public int WeaponType
        {
            get;
            set;
        }
        public bool ShotFired
        {
            get;
            set;
        }
        public bool ShotFired2
        {
            get;
            set;
        }
        public float Life
        {
            get;
            set;
        }
        public int Kills
        {
            get;
            set;
        }
        public bool framediagonal
        {
            get;
            set;
        }
        public bool framevanlig
        {
            get;
            set;
        }
        
        protected KeyboardState prevKs;
        protected KeyboardState prevKs2;
        protected MouseState prevMs;    //För huvudattack
        protected MouseState prevMS2;   //För sekundarattack
        public void Respawn(Vector2 respawn)
        {
            Life = 200F;
            Position = respawn;
            Angle = (float)(Math.PI / 2);
        }
        private void Styrning()
        {
            MouseState mouse = Mouse.GetState();
            
            //Hämtar tangentbordets nuvarande läge
            KeyboardState ks = Keyboard.GetState();
            
           
            // Gå framåt
            if (ks.IsKeyDown(Keys.W))
            {
                Angle = -(float)(Math.PI / 2);
                Speed = MaxSpeed;
                
                
            }
            //Gör så att karaktären stannar när man släpper "W" knappen, och inte fortsätter att röra på sig
            if (ks.IsKeyUp(Keys.W) && prevKs.IsKeyDown(Keys.W) && Speed > 0)
            {
                Speed = 0;
            }
            //Gå bakåt
            if (ks.IsKeyDown(Keys.S))
            {
                Angle = (float)(Math.PI / 2);
                Speed = MaxSpeed;
             
                
                
               
            }
            //Gör så att karaktären stannar när man släpper "S" knappen, och inte fortsätter att röra på sig
            if (ks.IsKeyUp(Keys.S) && prevKs.IsKeyDown(Keys.S) && Speed > 0)
            {
                Speed = 0;
            }
            // Gå till höger
            if (ks.IsKeyDown(Keys.D))
            {
                Angle = 0;
                Speed = MaxSpeed;
            }
            //Gör så att karaktären stannar när man släpper "D" knappen, och inte fortsätter att röra på sig
            if (ks.IsKeyUp(Keys.D) && prevKs.IsKeyDown(Keys.D) && Speed > 0)
            {
                Speed = 0;
            }
            // Gå till vänster
            if (ks.IsKeyDown(Keys.A))
            {
                Angle = (float)(Math.PI);
                Speed = MaxSpeed;
            }
            //Gör så att karaktären stannar när man släpper "A" knappen, och inte fortsätter att röra på sig
            if (ks.IsKeyUp(Keys.A) && prevKs.IsKeyDown(Keys.A) && Speed > 0)
            {
                Speed = 0;
            }
        
            //Höger upp
            if (ks.IsKeyDown(Keys.D) && ks.IsKeyDown(Keys.W))
            {
                Angle = -(float)(Math.PI) / 4;
                Speed = MaxSpeed;
            }
            //Höger ner
            if (ks.IsKeyDown(Keys.D) && ks.IsKeyDown(Keys.S))
            {
                Angle = (float)(Math.PI) / 4;
                Speed = MaxSpeed;
            }
            //Vänaster ner
            if (ks.IsKeyDown(Keys.A) && ks.IsKeyDown(Keys.S))
            {
                Angle = (-(float)(Math.PI) / 4) - (4 * (-(float)(Math.PI) / 4));
                Speed = MaxSpeed;
            }
            
            //Vänaster upp
            if (ks.IsKeyDown(Keys.A) && ks.IsKeyDown(Keys.W))
            {
                Angle = (-(float)(Math.PI) / 4) + (2 * (-(float)(Math.PI) / 4));
                Speed = MaxSpeed;
            }
            //Kollar vad förra tangentbordstryckningen var
            prevKs = ks;
            //räknar ut vilken riktning som karaktären har
            Direction = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
        }
        public void huvudattack()
        {
            //Kollar musens nuvarande läge, för huvudattac
            MouseState mouse = Mouse.GetState();
            //avfyrar primärt vapen
            if (mouse.LeftButton == ButtonState.Pressed && prevMs.LeftButton == ButtonState.Released)
            {
                ShotFired = true;
                
            }
            //Tar ut skillnaden i position mellan muspekaren och karaktären, för att kunna räkna ut vinkeln.
            float XDistance = mouse.X - Position2.X;
            float YDistance = mouse.Y - Position2.Y;
            //Matematisk beräkning som tar reda på vinkeln mellan karaktären och muspekaren.
            ShotAngle = (float)Math.Atan2(YDistance, XDistance);
            //Använder sig av ovanstående uträkning för att ta reda på vilken vinkel man siktar med musen.
            ShotDirection = new Vector2((float)Math.Cos(ShotAngle), (float)Math.Sin(ShotAngle));
            //Kollar vilket det föra mussklicket var, för huvudattack 
            prevMs = mouse;
            //Kör funktionen för styrningen
        }
        public void sekundarattack()
        {
          
            //Kollar musens nuvarande läge, för sekundarattack
            MouseState mouse2 = Mouse.GetState();
            if (sekundarstop == true)
            {
                //avfyrar sekundart vapen
                if (mouse2.RightButton == ButtonState.Pressed && prevMS2.RightButton == ButtonState.Released)
                {
                    ShotFired2 = true;
                }
            }
            //Tar ut skillnaden i position mellan muspekaren och karaktären, för att kunna räkna ut vinkeln.
            float XDistance = mouse2.X - Position2.X;
            float YDistance = mouse2.Y - Position2.Y;
            //Matematisk beräkning som tar reda på vinkeln mellan karaktären och muspekaren.
            ShotAngle = (float)Math.Atan2(YDistance, XDistance);
            //Använder sig av ovanstående uträkning för att ta reda på vilken vinkel man siktar med musen.
            ShotDirection = new Vector2((float)Math.Cos(ShotAngle), (float)Math.Sin(ShotAngle));
            //Kollar vilket det föra mussklicket var, för sekundarattack
            prevMS2 = mouse2;
            //Kör funktionen för styrningen
        
        }
        public void animationer()
        {
            int fart = 130;
            KeyboardState ks2 = Keyboard.GetState();
            //if (timer10 > 20)
            //{
                //animation om man går ner
                if (ks2.IsKeyDown(Keys.S) && ks2.IsKeyUp(Keys.A) && ks2.IsKeyUp(Keys.D))
                {
                    framediagonal = false;
                    framevanlig = true;
                    if (Time >= fart)
                    {
                        if (Frame < 1 || Frame >= 4)
                            Frame = 1;
                        else
                            Frame++;
                        Time = 0;
                    }
                }
                //animation om man går upp
                if (ks2.IsKeyDown(Keys.W) && ks2.IsKeyUp(Keys.A) && ks2.IsKeyUp(Keys.D))
                {
                    framediagonal = false;
                    framevanlig = true;
                    if (Time >= fart)
                    {
                        if (Frame < 21 || Frame >= 24)
                            Frame = 21;
                        else
                            Frame++;
                        Time = 0;
                    }
                }
                //animation om man går till vänster
                if (ks2.IsKeyDown(Keys.A) && ks2.IsKeyUp(Keys.W) && ks2.IsKeyUp(Keys.S))
                {
                    framediagonal = false;
                    framevanlig = true;
                    if (Time >= fart)
                    {
                        if (Frame < 11 || Frame >= 14)
                            Frame = 11;
                        else
                            Frame++;
                        Time = 0;
                    }
                }
                //animation om man går till Höger
                if (ks2.IsKeyDown(Keys.D) && ks2.IsKeyUp(Keys.W) && ks2.IsKeyUp(Keys.S))
                {
                    framediagonal = false;
                    framevanlig = true;
                    if (Time >= fart)
                    {
                        if (Frame < 31 || Frame >= 34)
                            Frame = 31;
                        else
                            Frame++;
                        Time = 0;
                    }
                }
            //}
            //Höger upp
            if (ks2.IsKeyDown(Keys.D) && ks2.IsKeyDown(Keys.W))
            {
                framediagonal = true;
                framevanlig = false;
                if (Time >= fart)
                {
                    if (Frame < 36 || Frame >= 39)
                        Frame = 36;
                    else
                        Frame++;
                    Time = 0;
                }
                timer10 = 0;
            }
            //Vänster upp
            if (ks2.IsKeyDown(Keys.A) && ks2.IsKeyDown(Keys.W))
            {
                framediagonal = true;
                framevanlig = false;
                if (Time >= fart)
                {
                    if (Frame < 16 || Frame >= 19)
                        Frame = 16;
                    else
                        Frame++;
                    Time = 0;
                }
                timer10 = 0;
            }
            //Vänster Ner
            if (ks2.IsKeyDown(Keys.A) && ks2.IsKeyDown(Keys.S))
            {
                framediagonal = true;
                framevanlig = false;
                if (Time >= fart)
                {
                    if (Frame < 6 || Frame >= 9)
                        Frame = 6;
                    else
                        Frame++;
                    Time = 0;
                }
                timer10 = 0;
            }
            //Höger Ner
            if (ks2.IsKeyDown(Keys.S) && ks2.IsKeyDown(Keys.D))
            {
                framediagonal = true;
                framevanlig = false;
                if (Time >= fart)
                {
                    if (Frame < 26 || Frame >= 29)
                        Frame = 26;
                    else
                        Frame++;
                    Time = 0;
                }
                timer10 = 0;
            }
            //else
            //{
            //    framediagonal = false;
            //}
            //Gör så att bilden för att gubben står still visas
            if (ks2.IsKeyUp(Keys.W) && ks2.IsKeyUp(Keys.S) && ks2.IsKeyUp(Keys.A) && ks2.IsKeyUp(Keys.D))
            {
                if (Frame >= 1 && Frame <= 4)
                    Frame = 0;
                else if (Frame >= 6 && Frame <= 9)
                    Frame = 5;
                else if (Frame >= 11 && Frame <= 14)
                    Frame = 10;
                else if (Frame >= 16 && Frame <= 19)
                    Frame = 15;
                else if (Frame >= 21 && Frame <= 24)
                    Frame = 20;
                else if (Frame >= 26 && Frame <= 29)
                    Frame = 25;
                else if (Frame >= 31 && Frame <= 34)
                    Frame = 30;
                else if (Frame >= 36 && Frame <= 39)
                    Frame = 35;
            }
            prevKs2 = ks2;
        }
        public override void Update(GameTime gameTime)
        {
            //KeyboardState ks2 = Keyboard.GetState();
            //if (ks2.IsKeyDown(Keys.S))
            //{
            //    if (Frame < 1)
            //    {
            //        Frame = 1;
            //    }
            //    if (Frame > 2)
            //    {
            //        Frame = 1;
            //    }
            //    Time += gameTime.ElapsedGameTime.Milliseconds;
            //    if (Time >= 100)
            //    {
            //        Frame = 2;
            //    }
            //    else if (Time >= 0)
            //    {
            //        Frame = 1;
            //    }
            //    if (Time >= 200)
            //    {
            //        Time = 0;
            //    } 
            //}
           animationer();
            //prevKs2 = ks2;
            Time += gameTime.ElapsedGameTime.Milliseconds;
            timer10 += gameTime.ElapsedGameTime.Milliseconds;
            huvudattack();  //kör funktionen för huvudattacken
                
            sekundarattack();   //kör funktinen för sekundarattacken
   
            Styrning();     //Kör funktionen för styrningen
            base.Update(gameTime);
        }
        public int Frame
        {
            get;
            set;
        }
        public int Time
        {
            get;
            set;
        }
          public int AnimationSpeed
        {
            get;
            set;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 DrawOffset, float levelHeight)
        {
            //Rectangle tmp = new Rectangle((Frame % 4) * 64, (Frame / 4) * 64, 64, 64);
            //Rectangle tmp2 = new Rectangle((Frame % 3) * 42, (Frame / 9) * 58, 42, 58);
            //spriteBatch.Draw(Gfx,
            //    Position - DrawOffset + new Vector2(400, 300),
            //    tmp2, Color.White, base.Angle3,
            //    new Vector2(32, 32), 1.0f, SpriteEffects.None, 0);
            Rectangle tmp2 = new Rectangle((Frame % 5) * 42, (Frame / 5) * 58, 42, 58);
            spriteBatch.Draw(Gfx,
                Position - DrawOffset + new Vector2(400, 300),
                tmp2, Color.White, base.Angle3,
                new Vector2(21, 29), 1.0f, SpriteEffects.None, (1/levelHeight)*(float)Position.Y);
            
        }
        
    }
}
