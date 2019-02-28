using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework.Storage;
namespace DetRiktigaSpelet
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //installningar for att kunna ansluta till servern.
        TcpClient client;   //Mojliggor anvandning av TCP client, som skoter det mesta med servern.
        int PORT = 1490;    //Porten till servern som spelet ansluter till.
        int BUFFER_SIZE = 8192;
        byte[] readBuffer;
        //Gamestates
        enum GameState { Start, Game, victory, lost, draw };
        GameState currentGameState = GameState.Start;
        //Spelar respawn punkter
        int player1Number = 1;
        int player2Number = 1;
        Vector2 p1Spawn;
        GameObj daloSpawn;
        Vector2 p2Spawn;
        GameObj ltdSpawn;
        Texture2D spawnPlate;
        //Spelargrafik
        Texture2D daloPlayer, ltdPlayer;
        //Levelhöjd, nödvändig för layers
        float levelHeight;
        int playerIndex;
        MemoryStream readStream, writeStream;
        BinaryReader reader;
        BinaryWriter writer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //skapa tva spelare
        Player Player1, Player2;
        //Laddar in sekundaraskott
        List<Shot> allsekundar, allsekundar2;
        //Laddar in skott
        List<Shot> allShots, allShots2;
        //Grafik till primaryskotten(shot1Gfx) och secondaryskotten(shot1Gfx2)
        Texture2D shot1Gfx, shot1Gfx2;
        //Text font
        SpriteFont segoe;
        //Grafik for hpbarbakgrunden
        Texture2D hpBarBGGfx;
        Vector2 hpBarBG;
        //grafik för kills/tidpanelen
        Texture2D killbarGfx;
        Vector2 killbarvector;
        //Laddar in hpmataren
        Meter hpBar;
        //grafik for i speletmeny
        Texture2D menyGfx;
        Vector2 menyVector;
        Texture2D menyKontrollerGfx;
        Vector2 menyKontrollerVector;
        //grafik för spelövermenyer
        Texture2D menylostGfx;
        Vector2 menylostVector;
        Texture2D menyvictoryGfx;
        Vector2 menyvictoryVector;
        Texture2D menydrawGfx;
        Vector2 menydrawVector;
        //grafik för lobbyn
        Texture2D lobbyGfx;
        Vector2 lobbyVector;
        //grafik för text som visas när man dödar eller dör
        Texture2D statustextGfx;
        Texture2D statustextGfx2;
        Vector2 statustextVector;
        Vector2 statustextVector2;
        //grafik för när en spelare är redo
        Texture2D readyGfx;
        Vector2 readyvector;
        Vector2 readyvector2;
        //grafik för ändra antal kills knapp
        Texture2D nextkillGfx;
        Texture2D nextkillGfx2;
        Texture2D nextkillGfx_down;
        Texture2D nextkillGfx2_down;
        
        Vector2 nextkillVector;
        Vector2 nextkillVector2;
        Vector2 nextkillVector3;
        Vector2 nextkillVector4;
        bool nextboxdownCheck1 = false;
        bool nextboxdownCheck2 = false;
        bool nextbox2downCheck1 = false;
        bool nextbox2downCheck2 = false;
        SpriteFont font;
        SpriteFont font2;
        SpriteFont fontkillsplayer1, finishgame; //spritefont för player 1 kills
        SpriteFont startaspellobby;
        //andrar muspekarens utseende till ett sikte
        Texture2D cursorTex;
        Texture2D cursorTex2;
        Vector2 cursorPos;
        Vector2 cursorPos2;
        
        //Bool som kollar ifall en till spelare ar ansluten eller ej
        bool player2Connected = false;
        GameObj Bana; //Skapa ett GameObj som ska anvandas som bana
        string displayMessage = ""; //Text som skall skrivas ut
        //timer for hur ofta man kan anvanda sekundaraattacken
        float timer = 5;
        //bool for hur ofta man kan anvanda sekundaraattacken
        bool sekundarisready = false;
        //timer for hur lange man ar slowad
        float timer1 = 6;
        bool slow1 = false;
        //randoms som har hand om skadan pa huvudattacken
        Random skada1 = new Random();
        Random skada2 = new Random();
        //Bool for om menyn ar aktiverad
        bool iMeny = false;
        bool iMenyKontroller = false;
        protected KeyboardState prevKs;
        bool kanskicka = true;
        float timerkanskicka = 0; //timer för player 2 animationer
        //variabler för vinst/förlust
        int killstowin = 5; //antal kills för att vinna
        float timerforgameover = 0; //så lång som en spelrunda kommer att vara
        float timerforgame = 0; //timer för att räkna ut tiden kvar
        float timeleft = 30; //tiden kvar
        float timeleftMinuter = 0;
        bool rundaslut = true;
        protected KeyboardState prevks2;
        protected MouseState premvms2;
        int killedtext = 0; //Timer för hur länge text visas som säger att du dödade motståndaren
        int diedtext = 0; //Timer för hur länge text visas som säger att du blev dödad av motståndaren
        float timerstatustext = 0;
        //in för om båda är redo att spela
        int player1ready = 0;
        int player2ready = 0;
        
        protected KeyboardState prevks3; //Belägen i case start, för att starta spelet.
        
        //Rita ut banan
        Texture2D tile_gfx;
        Texture2D tile_gfx2;
        Texture2D tile_gfx3;
        Texture2D tile_gfxReal;
        List<Vector2> tile_position = new List<Vector2>();
        List<int> tile_type = new List<int>();
        List<walls> tilepositions, tilepositionsGolv, tilepositions2;
        int level_number = 1;
        int layernumber = 0;
        int bild1 = 0;
        //Timer for freezetime before game starts, and on new round
        float freezeTimer = 5;
        //text för freeze counter
        SpriteFont countFont;
        Texture2D countTex;
        Texture2D countTex1;
        Texture2D countTex2;
        Texture2D countTex3;
        Vector2 counttexVector;
        //freeze timer
        enum freezecounter
        {
            three,
            two,
            one,
            go,
            none
        };
        freezecounter threeTwoOneGo = freezecounter.none;
        //Ljud
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue huvudattack1;
        Cue huvudattack2;
        Cue huvudattack3;
        Random huvudattackljud = new Random();
        Cue huvudattack1Hit;
        Cue huvudattack2Hit;
        Cue huvudattack3Hit;
        Random huvudattackOnHitLjud;
        Cue sekundärattack;
        Cue sekundärattackHit;
        //starta spelet timer i lobbyn
        float timerlobby = 0;
        int tidförstart = 4;
        bool timerstarta = false;
        //grafik för timerlobby
        Texture2D lobbytimerGfx;
        Vector2 lobbytimerVector;
        //bana id för att man ska kunna byta bana
        int banaID = 1;
        //cooldowntimer för sekundärattack
        int cooldownSekundär = 5;
        float timerSekundärCooldown = 0;
        Texture2D slowreadyGfx;
        Texture2D slownotreadyGfx;
        Vector2 slowreadyVector;
        //animation
        List<shotanimation> allAnimations, allAnimations2;
        Texture2D AnimationGfx;
        //musknappar för lobbyn
        Texture2D LobbyKnappBackGfx;
        Vector2 LobbyKnappBackVector;
        bool lobbybackknappsynlig = false;
        Texture2D LobbyKnappEnterGfx;
        Vector2 LobbyKnappEnterVector;
        bool lobbyEnterknappsynlig = false;
        //musklick-knappar, i speletmenyn
        Texture2D ispeletmenyKnappExitGfx;
        Texture2D ispeletmenyKnappKontrollerGfx;
        Texture2D ispeletmenyKnappMenyGfx;
        Texture2D ispeletmenyKnappBackGfx;
        Vector2 ispeletmenyKnappExitVector;
        Vector2 ispeletmenyKnappKontrollerVector;
        Vector2 ispeletmenyKnappMenyVector;
        bool ispeletmenyKnappExitSynlig = false;
        bool ispeletmenyKnappKontrollerSynlig = false;
        bool ispeletmenyKnappMenySynlig = false;
        bool ispeletmenyKnappBackSynlig = false;
        protected MouseState PrevMsingameMeny;
        bool ispeletmenyfårtrycka = true;
        //musknappar för endscreens
        Texture2D EndscreenTillLobbyGfx;
        Vector2 EndscreenTillLobbyVector;
        Texture2D EndscreenTillExitGfx;
        Vector2 EndscreenTillExitVector;
        bool EndscreenTillLobbySynlig = false;
        bool EndscreenTillExitSynlig = false;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
        }
        protected override void Initialize()
        {
            readStream = new MemoryStream();    //Laser en datastrom
            writeStream = new MemoryStream();   //Skriver en datastrom sa den kan lasas.
            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);
            //initiera spelare
            Player1 = new Player();
            Player2 = new Player();
            //initiera skott
            allShots = new List<Shot>();
            allShots2 = new List<Shot>();
            //initiera sekundaraskott
            allsekundar = new List<Shot>();
            allsekundar2 = new List<Shot>();
            //bana
            tilepositions = new List<walls>();
            tilepositionsGolv = new List<walls>();
            tilepositions2 = new List<walls>();
            //animation
            allAnimations = new List<shotanimation>();
            allAnimations2 = new List<shotanimation>();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tile_gfxReal = Content.Load<Texture2D>("tileset");
            //ljud
            audioEngine = new AudioEngine("Content\\sounds.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
            huvudattack1 = soundBank.GetCue("Lazorz");
            huvudattack2 = soundBank.GetCue("Lazorz2");
            huvudattack3 = soundBank.GetCue("Lazorz3");
            huvudattack1Hit = soundBank.GetCue("LazorzHit");
            huvudattack2Hit = soundBank.GetCue("LazorzHit2");
            huvudattack3Hit = soundBank.GetCue("LazorzHit3");
            sekundärattack = soundBank.GetCue("Orbz");
            sekundärattackHit = soundBank.GetCue("OrbzHit");
            //ritar ut banan
            tile_gfx = Content.Load<Texture2D>("tile1");
            tile_gfx2 = Content.Load<Texture2D>("tile1_top");
            tile_gfx3 = Content.Load<Texture2D>("tile2");
            LaddaLevel(level_number);
            //laddar in grafik för musklickknappar i endscreensen
            EndscreenTillLobbyGfx = Content.Load<Texture2D>("EndscreenTillLobby");
            EndscreenTillLobbyVector = new Vector2(163, 409);
            EndscreenTillExitGfx = Content.Load<Texture2D>("EndscreenTillExit");
            EndscreenTillExitVector = new Vector2(439, 409);
            //Laddar in grafik för musklick-knappar i ingame menyn
            ispeletmenyKnappExitGfx = Content.Load<Texture2D>("ispeletmenyKnappExit");
            ispeletmenyKnappExitVector = new Vector2(53, 533);
            ispeletmenyKnappKontrollerGfx = Content.Load<Texture2D>("ispeletmenyKnappKontroller");
            ispeletmenyKnappKontrollerVector = new Vector2(53, 436);
            ispeletmenyKnappMenyGfx = Content.Load<Texture2D>("ispeletmenyKnappMeny");
            ispeletmenyKnappMenyVector = new Vector2(53, 334);
            ispeletmenyKnappBackGfx = Content.Load<Texture2D>("ispeletmenyKnappBack");
            //Laddar in grafik för musklick-knappar i lobbyn
            LobbyKnappBackGfx = Content.Load<Texture2D>("lobbyknapp_back");
            LobbyKnappBackVector = new Vector2(505, 462);
            LobbyKnappEnterGfx = Content.Load<Texture2D>("lobbyknapp_enter");
            LobbyKnappEnterVector = new Vector2(365, 273);
            
            //Laddar in grafik för skottanimation
            AnimationGfx = Content.Load<Texture2D>("shot_animation");
            //grafik för sekundärförmåga, cooldown
            slowreadyGfx = Content.Load<Texture2D>("slow_ready");
            slownotreadyGfx = Content.Load<Texture2D>("slow_notready");
            slowreadyVector = new Vector2(329, -6);
            //laddar in grafik för freeze timer
            countTex = Content.Load<Texture2D>("GO2");
            countTex1 = Content.Load<Texture2D>("tid1");
            countTex2 = Content.Load<Texture2D>("tid2");
            countTex3 = Content.Load<Texture2D>("tid3");
            counttexVector = new Vector2((graphics.PreferredBackBufferWidth / 2) - (countTex.Width / 2), (graphics.PreferredBackBufferHeight / 2) - (countTex.Height / 2));
            countFont = Content.Load<SpriteFont>("countfont");
            //laddar in grafik för lobbytimer
            lobbytimerGfx = Content.Load<Texture2D>("lobbytimer");
            lobbytimerVector = new Vector2(390, 275);
            //laddar in stats text för när man dör eller dödar
            statustextGfx = Content.Load<Texture2D>("du_dödade_fienden");
            statustextGfx2 = Content.Load<Texture2D>("fienden_dödade_dig");
            statustextVector = new Vector2(170, 330);
            statustextVector2 = new Vector2(170, 330);
            //laddar in font som ska visa kills och tid kvar i vinst/förlustmenyerna
            finishgame = Content.Load<SpriteFont>("spelavslutat");
            //laddar in font som visar countdown timern i lobbyn innan spelet startar
            startaspellobby = Content.Load<SpriteFont>("lobbytimer1");
            //laddar in font för player 1 kills
            fontkillsplayer1 = Content.Load<SpriteFont>("player1kills");
            //laddar in font som visar vilken knapp man trycker pa for att oppna menyn
            font2 = Content.Load<SpriteFont>("SpriteFont2");
            //Laddar in font for hp
            font = Content.Load<SpriteFont>("SpriteFont1");
            //laddar in grafik for hpbar
            Texture2D hpbargfx = Content.Load<Texture2D>("hpbar");
            hpBar = new Meter() { Gfx = hpbargfx, Position = new Vector2(108, 35) };
            //laddar in grafik for hpbarsbakgrund
            hpBarBGGfx = Content.Load<Texture2D>("hpbar_bakgrund");
            hpBarBG = new Vector2(0, 0);
            //Laddar in grafik för kills/tidpanel
            killbarGfx = Content.Load<Texture2D>("kills_time_bar");
            killbarvector = new Vector2(420, 4);
            //Laddar in grafik för lobbyn
            lobbyGfx = Content.Load<Texture2D>("menyer_lobby");
            lobbyVector = new Vector2(0, 0);
            //laddar in redosakerna i lobbyn
            readyGfx = Content.Load<Texture2D>("ready_to_play");
            readyvector = new Vector2(105, 204);
            readyvector2 = new Vector2(580, 204);
            //laddar in grafik för öka antal kills för att vinna knappen
            nextkillGfx = Content.Load<Texture2D>("next_box");
            nextkillGfx2 = Content.Load<Texture2D>("next_box2");
            nextkillGfx_down = Content.Load<Texture2D>("next_box_down");
            nextkillGfx2_down = Content.Load<Texture2D>("next_box2_down");
            nextkillVector = new Vector2(257, 385);
            nextkillVector2 = new Vector2(102, 385);
            nextkillVector3 = new Vector2(257, 475);
            nextkillVector4 = new Vector2(102, 475);
            //laddar in grafik för spelöver menyer
            menyvictoryGfx = Content.Load<Texture2D>("menyer_victory");
            menyvictoryVector = new Vector2(0, 0);
            menylostGfx = Content.Load<Texture2D>("menyer_lost");
            menylostVector = new Vector2(0, 0);
            menydrawGfx = Content.Load<Texture2D>("menyer_draw");
            menydrawVector = new Vector2(0, 0);
            //laddar in grafik for i speletmeny
            menyGfx = Content.Load<Texture2D>("ispeletmeny");
            menyVector = new Vector2(0, 228);
            menyKontrollerGfx = Content.Load<Texture2D>("ispeletmeny_kontroll");
            menyKontrollerVector = new Vector2(0, 228);
           
            //laddar grafik for muspekaren
            cursorTex = Content.Load<Texture2D>("reticle");
            cursorTex2 = Content.Load<Texture2D>("muspekare2"); //muspekare för lobbyn
            
            //laddar font
            segoe = Content.Load<SpriteFont>("segoe");
            //Ladar spelargrafiken
            Texture2D greenTankGfx = Content.Load<Texture2D>("green_tank");
            Texture2D redTankGfx = Content.Load<Texture2D>("red_tank");
            //Laddar in bangrafiken
            //Texture2D level1Gfx = Content.Load<Texture2D>("level");
            Texture2D level1Gfx = Content.Load<Texture2D>("bana_golv");
            //Laddar in skottgrafiken
            shot1Gfx = Content.Load<Texture2D>("shot_test");
            //Laddar in sekundarskottgrafiken
            shot1Gfx2 = Content.Load<Texture2D>("slow_shot");
            //Laddar grafiken for spelare 1, test
            ltdPlayer = Content.Load<Texture2D>("Player_LTD");
            
            //Laddar grafiken for spelare 1
            //Player1.Gfx = Content.Load<Texture2D>("green_tank");
            Player1.Position = new Vector2(400, 300);
            //Laddar grafiken for spelare 2
            daloPlayer = Content.Load<Texture2D>("Player_DALO");
            //Laddar grafiken för spawnplattor
            spawnPlate = Content.Load<Texture2D>("spawn");
            //Laddar grafiken for banan, farn GameObj klassen
            Bana = new GameObj() { Gfx = level1Gfx, Position = new Vector2(0, 0), Angle = 0 };
            //IP-adressen som spelet ansluter till.
            StreamReader readIP = File.OpenText("IP.txt"); 
            string connectIP = readIP.ReadLine();
            string IP = connectIP;
            //Ansluter till server
            client = new TcpClient();
            client.NoDelay = true;
            client.Connect(IP, PORT);
            readBuffer = new byte[BUFFER_SIZE];
            client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
        }
        protected override void Update(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Start:
                   MouseState mouseState2 = Mouse.GetState();
                   var mousePosition = new Point(mouseState2.X, mouseState2.Y);
                   cursorPos2 = new Vector2(mouseState2.X, mouseState2.Y);
                    Rectangle morekillbox = new Rectangle((int)nextkillVector.X , (int)nextkillVector.Y, 40, 40); //box runt knappen för att öka antal kills som behövs för att vinna
                    Rectangle morekillbox2 = new Rectangle((int)nextkillVector2.X, (int)nextkillVector2.Y, 40, 40); //box runt knappen för att minska antal kills som behövs för att vinna
                    Rectangle morekillbox3 = new Rectangle((int)nextkillVector3.X, (int)nextkillVector3.Y, 40, 40); //box runt knappen för att öka tidsbegransningen på en spelrunda
                    Rectangle morekillbox4 = new Rectangle((int)nextkillVector4.X, (int)nextkillVector4.Y, 40, 40); //box runt knappen för att minska tidsbegransningen på en spelrunda
                    Rectangle backbox = new Rectangle((int)LobbyKnappBackVector.X, (int)LobbyKnappBackVector.Y, 184, 57); //box runt knappen för exit
                    Rectangle enterbox = new Rectangle((int)LobbyKnappEnterVector.X, (int)LobbyKnappEnterVector.Y, 124, 56); //box runt knappen för enter
                    int killsneed = killstowin;
                    float timeforround = timeleft;
                    //musklickfunktion för att stänga lobbyn
                    if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (backbox.Contains(mousePosition))
                        {
                            Application.Exit();
                        }
                    }
                    //Gör så att en bild visas så att det ser ut som man trycker på knappen, när man trycker på kanppen för att stänga lobbyn
                    if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (backbox.Contains(mousePosition))
                        {
                            lobbybackknappsynlig = true;
                        }
                    }
                    else
                    {
                        lobbybackknappsynlig = false;
                    }
                    //musklickfunktion för att starta spelet, enter knappen
                    if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (enterbox.Contains(mousePosition))
                        {
                            if (player1ready <= 0)
                            {
                                player1ready += 1;
                            }
                            else if (player1ready >= 1)
                            {
                                player1ready -= 1;
                            }
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.PlayerReady);
                            writer.Write(player1ready);
                            SendData(GetDataFromMemoryStream(writeStream));
                        }
                    }
                    //Gör så att en bild visas så att det ser ut som man trycker på knappen, när man trycker på kanppen för att starta spelet (enter)
                    if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (enterbox.Contains(mousePosition))
                        {
                            lobbyEnterknappsynlig = true;
                        }
                    }
                    else
                    {
                        lobbyEnterknappsynlig = false;
                    }
                    if (player2Connected)
                    {
                        //gör så det ser ut som att man trycker på knapparna, för att ändra antalet kills och tid
                        if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                        {
                            if (morekillbox.Contains(mousePosition))
                            {
                                nextboxdownCheck1 = true;
                            }
                            else if (morekillbox2.Contains(mousePosition))
                            {
                                nextboxdownCheck2 = true;
                            }
                            else if (morekillbox3.Contains(mousePosition))
                            {
                                nextbox2downCheck1 = true;
                            }
                            else if (morekillbox4.Contains(mousePosition))
                            {
                                nextbox2downCheck2 = true;
                            }
                        }
                        else
                        {
                            nextboxdownCheck1 = false;
                            nextboxdownCheck2 = false;
                            nextbox2downCheck1 = false;
                            nextbox2downCheck2 = false;
                            
                        }
                        //öka antal kills för att vinna
                        if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released))
                        {
                            if (morekillbox.Contains(mousePosition))
                            {
                                killstowin += 5;
                            }
                        }
                        //Minska antal kills för att vinna
                        if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released))
                        {
                            if (morekillbox2.Contains(mousePosition))
                            {
                                killstowin -= 5;
                                if (killstowin < 5)
                                {
                                    killstowin = 5;
                                }
                            }
                        }
                        //öka tidsbegränsningen för spelomgången
                        if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released))
                        {
                            if (morekillbox3.Contains(mousePosition))
                            {
                                timeleft += 5;
                            }
                        }
                        //minska tidsbegränsningen för spelomgången
                        if (mouseState2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed) && premvms2.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released))
                        {
                            if (morekillbox4.Contains(mousePosition))
                            {
                                timeleft -= 5;
                            }
                        }
                        if (timeleft >= 60)
                        {
                            timeleftMinuter += 1;
                            timeleft = 0;
                        }
                        else if (timeleft < 0)
                        {
                            timeleftMinuter -= 1;
                            timeleft = 55;
                        }
                        int killsneed2 = killstowin;
                        float timeforround2 = timeleft;
                        float timeforround2minuter = timeleftMinuter;
                        //synkar antal kills som behövs för att vinna med player 2
                        if (killsneed2 != killsneed)
                        {
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.Killstowinchanged);
                            writer.Write(killsneed2);
                            SendData(GetDataFromMemoryStream(writeStream));
                        }
                        if (timeforround2 != timeforround)
                        {
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.Timeforroundchanged);
                            writer.Write(timeforround2);
                            writer.Write(timeforround2minuter);
                            SendData(GetDataFromMemoryStream(writeStream));
                        }
                    }
                    premvms2 = mouseState2;
                    KeyboardState ks1 = Keyboard.GetState();
                    //stänger applikationen
                    if (ks1.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        Application.Exit();
                    }
                    if (ks1.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F12) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F12))
                    {
                        graphics.ToggleFullScreen();
                    }
                    
                    if (player2Connected)
                    {
                        //tryck pa "R" knappen for att bli spelare 1
                        if (ks1.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.R))
                        {
                            player1Number = 1;
                            player2Number = 2;
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.PlayerChange);
                            writer.Write(player1Number);
                            //writer.Write(player2Number);
                            SendData(GetDataFromMemoryStream(writeStream));
                        }
                        //tryck pa "T" knappen for att bli spelare 2
                        if (ks1.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.T) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.T))
                        {
                            player1Number = 2;
                            player2Number = 1;
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.PlayerChange);
                            writer.Write(player1Number);
                            //writer.Write(player2Number);
                            SendData(GetDataFromMemoryStream(writeStream));
                        }
                        prevKs = ks1; //kollar vad den forra knapptryckningen var
                    }
                    switch (player1Number)
                    {
                        case 1:
                            p1Spawn = daloSpawn.Position;
                            Player1.Position = p1Spawn;
                            Player1.Life = 200F;
                            Player1.Gfx = daloPlayer;
                            break;
                        case 2:
                            p1Spawn = ltdSpawn.Position;
                            Player1.Position = p1Spawn;
                            Player1.Life = 200F;
                            Player1.Gfx = ltdPlayer;
                            break;
                    }
                    switch (player2Number)
                    {
                        case 1:
                            p2Spawn = daloSpawn.Position;
                            Player2.Position = p2Spawn;
                            Player2.Life = 200F;
                            Player2.Gfx = daloPlayer;
                            break;
                        case 2:
                            p2Spawn = ltdSpawn.Position;
                            Player2.Position = p2Spawn;
                            Player2.Life = 200F;
                            Player2.Gfx = ltdPlayer;
                            break;
                    }
                    if (timerstarta == true)
                    {
                        timerlobby += gameTime.ElapsedGameTime.Milliseconds;
                        if (timerlobby >= 1000)
                        {
                            tidförstart -= 1;
                            timerlobby = 0;
                            
                        }
                    }
                    if (ks1.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) && prevks3.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter) && player1Number != player2Number)
                    {
                        if (player1ready <= 0)
                        {
                            player1ready += 1;
                        }
                        else if (player1ready >= 1)
                        {
                            player1ready -= 1;
                        }
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.PlayerReady);
                        writer.Write(player1ready);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                    if (player1ready >=1 && player2ready >=1)
                    {
                        timerstarta = true;
                      
                    }
                    else if (player1ready <=0 || player2ready <=0)
                    {
                        timerstarta = false;
                        tidförstart = 4;
                    }
                    if (tidförstart <= 0)
                    {
                        currentGameState = GameState.Game;
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.StateChange);
                        writer.Write(1);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                    prevks3 = ks1;
                    break;
                case GameState.Game:
                    //freeze timer
                    timerforgame += gameTime.ElapsedGameTime.Milliseconds;
                    timerkanskicka += gameTime.ElapsedGameTime.Milliseconds; // timer för player 2 animationer startas 
                    timerstatustext += gameTime.ElapsedGameTime.Milliseconds;
                    if (timerstatustext >= 1000)
                    {
                        killedtext -= 1;
                        diedtext -= 1;
                        timerstatustext = 0;
                    }
                     if (freezeTimer <= 0)
                    {
                    rundaslut = true;//gör så att rund timern börjar köras igen
                    KeyboardState ks = Keyboard.GetState();
                    MouseState mouseState3 = Mouse.GetState();
                    var mousePosition2 = new Point(mouseState3.X, mouseState3.Y);
                    Rectangle ispeletmenyKnappExitRect = new Rectangle((int)ispeletmenyKnappExitVector.X, (int)ispeletmenyKnappExitVector.Y, 178, 51);
                    Rectangle ispeletmenyKnappKontrollerRect = new Rectangle((int)ispeletmenyKnappKontrollerVector.X, (int)ispeletmenyKnappKontrollerVector.Y, 178, 51);
                    Rectangle ispeletmenyKnappMenyRect = new Rectangle((int)ispeletmenyKnappMenyVector.X, (int)ispeletmenyKnappMenyVector.Y, 178, 51);
                    //cursorPos2 = new Vector2(mouseState3.X, mouseState3.Y);
                    //Skiftar mellan fullscreen och windowed
                    if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F12) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F12))
                    {
                        graphics.ToggleFullScreen();
                    }
                    //om kontrolldelen av menyn ar oppen kan man inte stanga huvuddelen av menyn
                    if (iMenyKontroller == false)
                    {
                        //oppnar i speletmenyn med ett tryck pa "Q" knappen
                        if (iMeny == false)
                        {
                            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape))
                            {
                                iMeny = true;
                            }
                        }
                        //stanger i speletmenyn med ett tryck pa "Q" knappen
                        else if (iMeny == true)
                        {
                            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape))
                            {
                                iMeny = false;
                            }
                            //gör så att spelet återupptas om man trycker på knappen med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappMenyRect.Contains(mousePosition2))
                                {
                                    iMeny = false;
                                }
                            }
                            //gör så det ser ut som att man trycker på tillbakaknappen, när man trycker på den med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappMenyRect.Contains(mousePosition2))
                                {
                                    ispeletmenyKnappMenySynlig = true;
                                }
                            }
                            else
                            {
                                ispeletmenyKnappMenySynlig = false;
                            }
                        }
                        if (iMeny == true)
                        {
                            Player1.ShotFired = false;
                            //stänger av spelet
                            if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
                            {
                                Application.Exit();
                                ispeletmenyKnappExitSynlig = true;
                            }
                            else if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.R))
                            {
                                
                                iMenyKontroller = true;
                            }
                            //gör så att spelet stängs av om man trycker på kanppen med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappExitRect.Contains(mousePosition2))
                                {
                                    Application.Exit();
                                }
                            }
                            //gör så det ser ut som att man trycker på exit knappen, när man trycker på den med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappExitRect.Contains(mousePosition2))
                                {
                                    ispeletmenyKnappExitSynlig = true;
                                }
                            }
                            else
                            {
                                ispeletmenyKnappExitSynlig = false;
                            }
                            //gör så att menyn för kontroller öppnas när man trycker på knappen med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappKontrollerRect.Contains(mousePosition2))
                                {
                                    iMenyKontroller = true;
                                }
                            }
                            //gör så det ser ut som att man trycker på exit knappen, när man trycker på den med musen
                            if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                            {
                                if (ispeletmenyKnappKontrollerRect.Contains(mousePosition2))
                                {
                                    ispeletmenyKnappKontrollerSynlig = true;
                                }
                            }
                            else
                            {
                                ispeletmenyKnappKontrollerSynlig = false;
                            }
                        }
                    }
                    //stanger kontrollerdelen av menyn
                    else if (iMenyKontroller == true)
                    {
                        if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.R))
                        {
                            iMenyKontroller = false;
                        }
                            //går tillbaka till huvuddelen när man trycker på kanppen med musen
                        else if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                        {
                            if (ispeletmenyKnappExitRect.Contains(mousePosition2))
                            {
                                iMenyKontroller = false;
                            }
                        }
                        //gör så det ser ut som att man trycker på exit knappen, när man trycker på den med musen
                        if (mouseState3.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                        {
                            if (ispeletmenyKnappExitRect.Contains(mousePosition2))
                            {
                                ispeletmenyKnappBackSynlig = true;
                            }
                        }
                        else
                        {
                            ispeletmenyKnappBackSynlig = false;
                        }
                    }
                    PrevMsingameMeny = mouseState3;
                    prevKs = ks;
                    if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
                    {
                        MessageBox.Show(playerIndex.ToString());
                    }
                    //ger hpbaren ett varde
                    hpBar.Value = Player1.Life;
                    //Position for muspekarsiktet
                    MouseState mouseState = Mouse.GetState();   //Kollar musens nuvarande lage
                    cursorPos = new Vector2(mouseState.X - 20, mouseState.Y - 20);  //Staller in siktet pa muspekarens mittpunkt
                    //Player 1 ursprungliga position
                    Vector2 iPosition = new Vector2(Player1.Position.X, Player1.Position.Y);
                    //Spelare 1 vinkel fran borjan
                    float angle1 = Player1.Angle;
                    float farten1 = Player1.Speed;
                    Vector2 oldpos = Player1.Position;
                    Player1.Update(gameTime);   //Uppdatera spelare1 
                    float hastigheten = Player1.Speed;
                    if (hastigheten == 0)
                    {
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.PlayerStoped);
                        writer.Write(hastigheten);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                    float farten2 = Player1.Speed;
                    if (timerstatustext >= 1000)
                    {
                        killedtext -= 1;
                        diedtext -= 1;
                        timerstatustext = 0;
                    }
                    //Raknar utt karaktarens vinkel
                    float angle2 = Player1.Angle;   //Spelare 1 vinkel efter han har rort sig
                    float deltap2 = angle2 - angle1;     //Skillnad i vinkel efter att spelare 1 har rort sig
                    //Raknar ut karaktarens position
                    Vector2 nPosition = new Vector2(Player1.Position.X, Player1.Position.Y);    //Position efter update
                    Vector2 deltap = Vector2.Subtract(nPosition, iPosition);    //Skillnaden mellan den nya och gamla positionen
                    //Skickar data pa karaktarens nya vinkel till spelare2
                    if (angle2 != angle1)
                    {
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.PlayerRotated);
                        writer.Write(deltap2);
                        writer.Write(angle1);
                        
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                    //Skickar data pa karaktarens nuvarande position till spelare2
                    if (deltap != Vector2.Zero)
                    {
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.PlayerMoved);
                        writer.Write(deltap.X);
                        writer.Write(deltap.Y);
                        writer.Write(iPosition.X);
                        writer.Write(iPosition.Y);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                    timerSekundärCooldown += gameTime.ElapsedGameTime.Milliseconds;  
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (sekundarisready == false)
                    {
                        if (timerSekundärCooldown >= 1000)
                        {
                            cooldownSekundär -= 1;
                            timerSekundärCooldown = 0;
                        }
                    }
            if (timer >= 5)
            {
                Player1.sekundarstop = true;
                timer = 5;
                sekundarisready = true;
                timerSekundärCooldown = 0;
                cooldownSekundär = 5;
            }
            else if (timer <5)
            {
                Player1.sekundarstop = false;
                sekundarisready = false;
            }
                    //Huvudattack
                    if (player2Connected == true)
                    {
                        if (iMeny == false)//kan inte skjuta om menyn är öppen
                        {
                            //Ifall ett skott avfyrats
                            if (Player1.ShotFired)
                            {
                               int talet5 = huvudattackljud.Next(1, 4);
                                if (talet5 == 1)
                                {
                                    soundBank.PlayCue("Lazorz");
                                }
                                else if (talet5 == 2)
                                {
                                    soundBank.PlayCue("Lazorz2");
                                }
                                else if (talet5 == 3)
                                {
                                    soundBank.PlayCue("Lazorz3");
                                }
                                int shotType1 = 0;
                                Vector2 shotPos1 = new Vector2();
                                shotPos1 = Player1.Position;     //Positionen som skottet kommar att skjutas ifran
                                float shotAnglePi = Player1.ShotAngle;  //Vinkeln som skottet kommer att skjutas i
                                float shotSpeed = 13;    //Hastigheten pa skottet
                                float shotPower = 5;
                                Vector2 shotDirec = new Vector2();
                                shotDirec = Player1.ShotDirection;  //Riktningen som skottet kommer att skjutas i
                                allShots.Add(new Shot() //Lagger till ett nytt skott i listan
                                {
                                    Gfx = shot1Gfx,     //Tilldelar grafik for skottet
                                    Position = shotPos1,     //Tilldelar position for skottet
                                    Angle = shotAnglePi,    //Tilldelar vinkel for skottet   
                                    Speed = shotSpeed,      //Tilldelar hastighet for skottet
                                    Power = shotPower,
                                    Direction = shotDirec   //Tilldelar riktning for skottet
                                });
                                Player1.ShotFired = false;  //Sager till att ett skott har avfyrats, gor att skott inte skjuts konstant i en lang linje
                                //Skickar data om att ett skott har avfyrats till spelare 2
                                writeStream.Position = 0;
                                writer.Write((byte)Protocol.BulletCreated);
                                writer.Write(shotType1);
                                writer.Write(shotPos1.X);
                                writer.Write(shotPos1.Y);
                                writer.Write(shotAnglePi);
                                writer.Write(shotSpeed);
                                writer.Write(shotPower);
                                writer.Write(shotDirec.X);
                                writer.Write(shotDirec.Y);
                                SendData(GetDataFromMemoryStream(writeStream));
                            }
                        }
                    }
                   
                    //Sekundarattack
                    if (player2Connected == true)
                    {
                        if (sekundarisready == true)
                        {
                            //Ifall ett skott avfyrats
                            if (Player1.ShotFired2)
                            {
                                soundBank.PlayCue("Orbz");
                                timer = 0;
                                int shotType = 0;
                                Vector2 shotPos = new Vector2();
                                shotPos = Player1.Position;     //Positionen som skottet kommar att skjutas ifran
                                float shotAnglePi = Player1.ShotAngle;  //Vinkeln som skottet kommer att skjutas i
                                float shotSpeed = 12;    //Hastigheten pa skottet
                                float shotPower = 5;
                                Vector2 shotDirec = new Vector2();
                                shotDirec = Player1.ShotDirection;  //Riktningen som skottet kommer att skjutas i
                                allsekundar.Add(new Shot() //Lagger till ett nytt skott i listan
                                {
                                    Gfx = shot1Gfx2,     //Tilldelar grafik for skottet
                                    Position = shotPos,     //Tilldelar position for skottet
                                    Angle = shotAnglePi,    //Tilldelar vinkel for skottet   
                                    Speed = shotSpeed,      //Tilldelar hastighet for skottet
                                    Power = shotPower,
                                    Direction = shotDirec   //Tilldelar riktning for skottet
                                });
                                Player1.ShotFired2 = false;  //Sager till att ett skott har avfyrats, gor att skott inte skjuts konstant i en lang linje
                                //Skickar data om att ett skott har avfyrats till spelare 2
                                writeStream.Position = 0;
                                writer.Write((byte)Protocol.Sekundarcreated);
                                writer.Write(shotType);
                                writer.Write(shotPos.X);
                                writer.Write(shotPos.Y);
                                writer.Write(shotAnglePi);
                                writer.Write(shotSpeed);
                                writer.Write(shotPower);
                                writer.Write(shotDirec.X);
                                writer.Write(shotDirec.Y);
                                SendData(GetDataFromMemoryStream(writeStream));
                            }
                        }
                    }
                    Rectangle player1box = new Rectangle((int)Player1.Position.X - 24, (int)Player1.Position.Y - 29, 48, 58);   //rektangel runt player1
                    Rectangle player2box = new Rectangle((int)Player2.Position.X - 24, (int)Player2.Position.Y - 29, 48, 58);   //rektangel runt player2
                    if (slow1 == true)
                    {
                        timer1 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (player2Connected == true)
                    {
                        //sekundarattack for spelare 1
                        for (int i = 0; i < allsekundar.Count; i++)
                        {
                            allsekundar[i].Update(gameTime);
                            Rectangle shotslowbox1 = new Rectangle((int)allsekundar[i].Position.X - 6, (int)allsekundar[i].Position.Y - 6, 12, 12);    //rektanglar runt spelare1 skott
                            //Kollision
                            try
                            {
                                if (shotslowbox1.Intersects(player2box))    //Kollar kollision mellan spelare1 skott och player2
                                {
                                    soundBank.PlayCue("OrbzHit");
                                    allsekundar.RemoveAt(i);  //Tar bort skottet nar det traffar
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error code: 1020");
                            }
                        }
                        if (timer1 <= 5)    //sa lange timern ar minde ar en 5 sa ar man slwad
                        {
                            Player1.MaxSpeed = 3.5F;    //slowar fienden nar han traffas
                        }
                        else if (timer1 > 5)    //efter 5 sekunder far man normal fart
                        {
                            Player1.MaxSpeed = 7F;      // gor sa att man far normal fart nar man var slowad i 5 sekunder
                        }
                        //sekundarattack for spelare 2
                        for (int i = 0; i < allsekundar2.Count; i++)
                        {
                            allsekundar2[i].Update(gameTime);
                            Rectangle shotslowbox2 = new Rectangle((int)allsekundar2[i].Position.X - 6, (int)allsekundar2[i].Position.Y - 6, 12, 12);    //rektanglar runt spelare1 skott
                            //Kollision
                            try
                            {
                                if (shotslowbox2.Intersects(player1box))    //Kollar kollision mellan spelare2 skott och player1
                                {
                                    soundBank.PlayCue("OrbzHit");
                                    allsekundar2.RemoveAt(i);  //Tar bort skottet nar det traffar
                                    slow1 = true;   //startar timern
                                    timer1 = 0;     //Restar timern
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error code: 1030");
                            }
                        }
                    }
                    int skott = 0;
                    int skott2 = 0;
                    if (player2Connected == true)
                    //Huvudattack for spelare 1
                    {
                        for (int i = 0; i < allShots.Count; i++) //Loopar igenom alla skott for spelare 1 
                        {
                            allShots[i].Update(gameTime); //uppdaterar skott for spelare 1
                            Rectangle shotbox1 = new Rectangle((int)allShots[i].Position.X - 4, (int)allShots[i].Position.Y - 5, 9, 11);    //rektanglar runt spelare1 skott
                            try
                            {
                                for (int j = 0; j < tilepositions.Count; j++)
                                {
                                    Rectangle tileBox10 = new Rectangle((int)tilepositions[j].Position.X - 32, (int)tilepositions[j].Position.Y - 64, 64, 64);
                                    Rectangle tileBoxLowWallMiss = new Rectangle((int)tilepositions[j].Position.X - 32, (int)tilepositions[j].Position.Y -96, 64, 64);
                                    if (tilepositions[j].skottkollision == true) //kollar om det är en hög vägg, ifall skottet ska tas bort
                                    {
                                    if (shotbox1.Intersects(tileBox10))
                                    {
                                        tilepositions[j].skottisover = false;
                                        skott++;
                                        if (skott == 1) //kollar så att det inte sker två kollisioner på samma skott
                                        {
                                            soundBank.PlayCue("LazorzHit");
                                            allAnimations.Add(new shotanimation() 
                                            {
                                                Gfx = AnimationGfx, 
                                                Position = allShots[i].Position,
                                                Angle = allShots[i].ShotAngle
                                            });
                                            allShots.RemoveAt(i);
                                            //skott--;
                                        }
                                    }
                                    }
                                    if (tilepositions[j].skottkollision == false) //kollar om det är en hög vägg, ifall skottet ska tas bort
                                    {
                                        if (shotbox1.Intersects(tileBox10))
                                        {
                                            tilepositions[j].skottisover = true;
                                        }
                                    }
                                    else if (shotbox1.Intersects(tileBox10) == false)
                                    {
                                        
                                        tilepositions[j].skottisover = false;
                                    }
                                    else if (shotbox1.Intersects(tileBoxLowWallMiss))
                                    {
                                     tilepositions[j].skottisover = false;
                                    }
                                }
                                
                            }
                            catch
                            {
                                MessageBox.Show("Jag orkar inte mer, vill inte vara kvar, jag längtar efter er min kära mor och far.");
                            }
                                if (shotbox1.Intersects(player2box))    //Kollar kollision mellan spelare2 skott och player1
                                {
                                    soundBank.PlayCue("LazorzHit2");
                                    allAnimations.Add(new shotanimation()
                                    {
                                        Gfx = AnimationGfx,
                                        Position = allShots[i].Position,
                                        Angle = allShots[i].ShotAngle
                                    });
                                    allShots.RemoveAt(i);   //Tar bort skottet nar det traffar
                                }
                        }
                        //animation för spelare1 huvudattack
                        for (int i = 0; i < allAnimations.Count; i++)
                        {
                            allAnimations[i].Update(gameTime); //Uppdatera animation
                            //ta bort färdiga skottanimationer
                            if (allAnimations[i].Active == false)
                            {
                                allAnimations.RemoveAt(i);
                            }
                        }
                        //Huvudattack for spelare 2
                        for (int i = 0; i < allShots2.Count; i++) //Loopar igenom alla skott for spelare 2
                        {
                            allShots2[i].Update(gameTime); //uppdaterar skott for spelare 2
                            Rectangle shotbox2 = new Rectangle((int)allShots2[i].Position.X - 4, (int)allShots2[i].Position.Y - 5, 9, 11);  //rektanglar runt spelare2 skott
                            try
                            {
                                for (int j = 0; j < tilepositions.Count; j++)
                                {
                                    Rectangle tileBox11 = new Rectangle((int)tilepositions[j].Position.X - 32, (int)tilepositions[j].Position.Y - 64, 64, 64);
                                    if (tilepositions[j].skottkollision == true) //kollar om det är en hög vägg, ifall skottet ska tas bort
                                    {
                                        if (shotbox2.Intersects(tileBox11))
                                        {
                                            skott2++;
                                            if (skott2 == 1) //kollar så att det inte sker två kollisioner på samma skott
                                            {
                                                soundBank.PlayCue("LazorzHit");
                                                allAnimations2.Add(new shotanimation()
                                                {
                                                    Gfx = AnimationGfx,
                                                    Position = allShots2[i].Position,
                                                    Angle = allShots2[i].ShotAngle
                                                });
                                                allShots2.RemoveAt(i);
                                                //skott--;
                                            }
                                            //if (skott > 1)
                                            //{
                                            //    skott = 1;
                                            //}
                                        }
                                    }
                                    if (tilepositions[j].skottkollision == false) //kollar om det är en hög vägg, ifall skottet ska tas bort
                                    {
                                        if (shotbox2.Intersects(tileBox11))
                                        {
                                            tilepositions[j].skottisover = true;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Vilse i koden skriver han omkring han ändrar på ting men han fattar ingenting." +"/r" +" Han lever på cola nudlar och ris, han hittar inte felet vet inte vad han skall fucking göra");
                            }
                            //Kollision
                            try
                            {
                                if (shotbox2.Intersects(player1box))    //Kollar kollision mellan spelare2 skott och player1
                                {
                                    soundBank.PlayCue("LazorzHit2");
                                    allAnimations2.Add(new shotanimation()
                                    {
                                        Gfx = AnimationGfx,
                                        Position = allShots2[i].Position,
                                        Angle = allShots2[i].ShotAngle
                                    });
                                    allShots2.RemoveAt(i);  //Tar bort skottet nar det traffar
                                    Player1.Life -= skada2.Next(10, 21);     //hp-antelet som ett skott tar 
                                    if (Player1.Life <= 0)  //nar en spelare dor
                                    {
                                        allShots.Clear();
                                        allShots2.Clear();
                                        allAnimations.Clear();
                                        allAnimations2.Clear();
                                        Player2.Kills += 1;
                                        killedtext = 3; //gör så att text skrivs ut som meddelar att du dog
                                        timer1 = 6;
                                        
                                        writeStream.Position = 0;
                                        writer.Write((byte)Protocol.StateChange);
                                        writer.Write(2);
                                        SendData(GetDataFromMemoryStream(writeStream));
                                        Player1.Respawn(p1Spawn);
                                        Player2.Respawn(p2Spawn);
                                    }
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error code: 1010");
                            }
                        }
                        //animation för spelare1 huvudattack
                        for (int i = 0; i < allAnimations2.Count; i++)
                        {
                            allAnimations2[i].Update(gameTime); //Uppdatera animation
                            //ta bort färdiga skottanimationer
                            if (allAnimations2[i].Active == false)
                            {
                                allAnimations2.RemoveAt(i);
                            }
                        }
                    }
                    ////////////Kollision Höga väggar
                    Rectangle player1box2 = new Rectangle((int)Player1.Position.X - 24, (int)Player1.Position.Y - 5, 50, 10);   //rektangel runt player1
                    Rectangle player1box3 = new Rectangle((int)Player1.Position.X - 2, (int)Player1.Position.Y - 30, 4, 60);   //rektangel runt player1
                    bool nejnej = true; //kollar om man kan springa i sidled eller ej, om det är en vägg ivägen
                    bool nejnej2 = true; //kollar om man kan springa upp eller ner, om det är en vägg ivägen
                  
                    //kollision mellan spelare och vägg
                    for (int i = 0; i < tilepositions.Count; i++)
                    {
                        Rectangle tileBox1 = new Rectangle((int)tilepositions[i].Position.X - 32, (int)tilepositions[i].Position.Y - 64, 64, 64);
                        if (player1box2.Intersects(tileBox1) == true)//Gör så att det inte är möjligt när man röar sig snett nedåt efter en vägg att springa igenom den. För X leden
                        {
                            nejnej = false;
                            //Player1.Position = new Vector2(oldpos.X, oldpos.Y);
                        }
                        if (player1box3.Intersects(tileBox1) == true)//Gör så att det inte är möjligt när man röar sig snett nedåt efter en vägg att springa igenom den. För Y leden
                        {
                            nejnej2 = false;
                            //Player1.Position = new Vector2(oldpos.X, oldpos.Y);
                        }
                        if (player1box.Intersects(tileBox1) == true)
                        {
                           
                            Player1.Position = new Vector2(oldpos.X, oldpos.Y);
                            //vänster ner
                                if (Player1.Angle == (-(float)(Math.PI) / 4) - (4 * (-(float)(Math.PI) / 4)))
                                {
                                    //rörelse i sidled
                                    if (player1box.Bottom > tileBox1.Top && player1box.Top < tileBox1.Top && nejnej == true)
                                    {
                                        if (timer1 > 5)    //när man inte är slowad
                                        {
                                        Player1.Position = new Vector2(oldpos.X - 7, oldpos.Y);
                                        }
                                        else if (timer1 < 5) //när man är slowad
                                        {
                                            Player1.Position = new Vector2(oldpos.X - 3, oldpos.Y);
                                        }
                                    }
                                    //rörelse upp eller ner
                                    else if (player1box.Left <= tileBox1.Right && player1box.Right > tileBox1.Right)
                                    {
                                        if (timer1 > 5)    //när man inte är slowad
                                        {
                                            Player1.Position = new Vector2(oldpos.X, oldpos.Y + 7);
                                        }
                                        else if (timer1 < 5) //när man är slowad
                                        {
                                            Player1.Position = new Vector2(oldpos.X, oldpos.Y + 3);
                                        }
                                    }
                                }
                                // höger ner
                            if (Player1.Angle == (float)(Math.PI) / 4) 
                            {
                                //rörelse i sidled
                                if (player1box.Bottom > tileBox1.Top && player1box.Top < tileBox1.Top && nejnej == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X + 7, oldpos.Y);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X + 3, oldpos.Y);
                                    }
                                }
                                //rörelse upp eller ner
                               else if (player1box.Right >= tileBox1.Left && player1box.Left < tileBox1.Left && nejnej2 == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y + 7);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y + 3);
                                    }
                                } 
                            }
                            //vänster upp
                            if (Player1.Angle == (-(float)(Math.PI) / 4) + (2 * (-(float)(Math.PI) / 4)))
                            {
                                //rörelse i sidled
                                if (player1box.Top < tileBox1.Bottom && player1box.Bottom > tileBox1.Bottom && nejnej == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X - 7, oldpos.Y);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X - 3, oldpos.Y);
                                    }
                                }
                                //rörelse upp eller ner
                                if (player1box.Left <= tileBox1.Right && player1box.Right > tileBox1.Right && nejnej2 == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y - 7);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y - 3);
                                    }
                                }
                            }
                            //höger upp
                            if (Player1.Angle == -(float)(Math.PI) / 4)
                            {
                                //rörelse i sidled
                                if (player1box.Top < tileBox1.Bottom && player1box.Bottom > tileBox1.Bottom && nejnej == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X + 7, oldpos.Y);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X + 3, oldpos.Y);
                                    }
                                }
                                //rörelse upp eller ner
                                else if (player1box.Right >= tileBox1.Left && player1box.Left < tileBox1.Left && nejnej2 == true)
                                {
                                    if (timer1 > 5)    //när man inte är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y - 7);
                                    }
                                    else if (timer1 < 5) //när man är slowad
                                    {
                                        Player1.Position = new Vector2(oldpos.X, oldpos.Y - 3);
                                    }
                                } 
                            }
                        }
                    }
                    
                    
                    if (rundaslut == true)
                    {
                        if (timerforgame >= 1000)
                        {
                            timeleft -= 1;
                            timerforgame = 0;
                        }
                    }
                         //minskar minuter kvar
                    if (timeleft < 0)
                    {
                        timeleftMinuter -= 1;
                        timeleft = 59;
                    }
                    
                    //      if (timerforgameover >= timeleft)
                    //{
                    if (timeleftMinuter <0)
                    {
                        timeleft = 0;
                        timeleftMinuter = 0;
                        rundaslut = false; //stannar timern som sköter rundlängd
                        player1ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        player2ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        
                        //byter gamestate till en vinstskärm
                        if (Player1.Kills > Player2.Kills)
                        {
                            currentGameState = GameState.victory;
                        }
                        //byter gamestate till en förlustskärm
                        else if (Player2.Kills > Player1.Kills)
                        {
                            currentGameState = GameState.lost;
                        }
                        else if (Player1.Kills == Player2.Kills)
                        {
                            currentGameState = GameState.draw;
                        }
                    }
                    //byter gamestate till en vinstskärm
                    if (Player1.Kills >= killstowin)
                    {
                        player1ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        player2ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        rundaslut = false; //stannar timern som sköter rundlängd
                        
                        currentGameState = GameState.victory;
                    }
                    //byter gamestate till en förlustskärm
                    else if (Player2.Kills >= killstowin)
                    {
                        player1ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        player2ready = 0; //gör så att spelet inte startar direkt när man går tillbaka till lobbyn
                        rundaslut = false; //stannar timern som sköter rundlängd
                        
                        currentGameState = GameState.lost;
                    }
                    }
                     else
                     {
                         if (timerforgame >= 1000)
                         {
                             freezeTimer -= 1;
                             timerforgame = 0;
                             if (freezeTimer == 4)
                             {
                                 threeTwoOneGo = freezecounter.three;
                             }
                             else if (freezeTimer == 3)
                             {
                                 threeTwoOneGo = freezecounter.two;
                             }
                             else if (freezeTimer == 2)
                             {
                                 threeTwoOneGo = freezecounter.one;
                             }
                             else if (freezeTimer == 1)
                             {
                                 threeTwoOneGo = freezecounter.go;
                             }
                             else if (freezeTimer <= 0)
                             {
                                 threeTwoOneGo = freezecounter.none;
                             }
                         }
                     }
                    break;
                case GameState.victory:
                    KeyboardState ks2 = Keyboard.GetState();
                    if (ks2.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F12) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F12))
                    {
                        graphics.ToggleFullScreen();
                    }
                    if (ks2.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Q))
                    {
                        timerstarta = false;
                        player1ready = 0;
                        player2ready = 0;
                        Player1.Kills = 0;
                        Player2.Kills = 0;
                        timeleft = 30;
                        timeleftMinuter = 0;
                        allShots.Clear();
                        allShots2.Clear();
                        allsekundar.Clear();
                        allsekundar2.Clear();
                        allAnimations.Clear();
                        allAnimations2.Clear();
                        freezeTimer = 6;
                        //Application.Restart();
                        currentGameState = GameState.Start;
                        
                    }
                    MouseState mouseState6 = Mouse.GetState();
                    var mousePosition6 = new Point(mouseState6.X, mouseState6.Y);
                    cursorPos2 = new Vector2(mouseState6.X, mouseState6.Y);
                    Rectangle EndscreenTillLobbyRect3 = new Rectangle((int)EndscreenTillLobbyVector.X, (int)EndscreenTillLobbyVector.Y, 184, 57);
                    Rectangle EndscreenTillExitRect3 = new Rectangle((int)EndscreenTillExitVector.X, (int)EndscreenTillExitVector.Y, 184, 57);
                    //gör så att man kommer till lobbyn om man trycker på knappen med musen
                    if (mouseState6.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect3.Contains(mousePosition6))
                        {
                            timerstarta = false;
                            player1ready = 0;
                            player2ready = 0;
                            Player1.Kills = 0;
                            Player2.Kills = 0;
                            timeleft = 30;
                            timeleftMinuter = 0;
                            allShots.Clear();
                            allShots2.Clear();
                            allsekundar.Clear();
                            allsekundar2.Clear();
                            allAnimations.Clear();
                            allAnimations2.Clear();
                            freezeTimer = 6;
                            //Application.Restart();
                            //möjlig bättre lösning
                            //Application.Restart();
                            currentGameState = GameState.Start;
                        }
                    }
                    //gör så det ser ut som att man trycker på knappen till lobbyn, när man trycker på den med musen
                    if (mouseState6.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect3.Contains(mousePosition6))
                        {
                            EndscreenTillLobbySynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillLobbySynlig = false;
                    }
                    //////////////////////////////////
                    //gör så att spelet återupptas om man trycker på knappen med musen
                    if (mouseState6.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect3.Contains(mousePosition6))
                        {
                            Application.Exit();
                        }
                    }
                    //gör så det ser ut som att man trycker på tillbakaknappen, när man trycker på den med musen
                    if (mouseState6.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect3.Contains(mousePosition6))
                        {
                            EndscreenTillExitSynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillExitSynlig = false;
                    }
                    PrevMsingameMeny = mouseState6;
                    if (ks2.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        Application.Exit();
                    }
                    prevks2 = ks2;
                    break;
                case GameState.lost:
                    KeyboardState ks3 = Keyboard.GetState();
                    if (ks3.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F12) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F12))
                    {
                        graphics.ToggleFullScreen();
                    }
                    if (ks3.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Q))
                    {
                        timerstarta = false;
                        player1ready = 0;
                        player2ready = 0;
                        Player1.Kills = 0;
                        Player2.Kills = 0;
                        timeleft = 30;
                        timeleftMinuter = 0;
                        allShots.Clear();
                        allShots2.Clear();
                        allsekundar.Clear();
                        allsekundar2.Clear();
                        allAnimations.Clear();
                        allAnimations2.Clear();
                        freezeTimer = 6;
                        //Application.Restart();
                        currentGameState = GameState.Start;
                        
                    }
                     MouseState mouseState5 = Mouse.GetState();
                    var mousePosition5 = new Point(mouseState5.X, mouseState5.Y);
                    cursorPos2 = new Vector2(mouseState5.X, mouseState5.Y);
                    Rectangle EndscreenTillLobbyRect2 = new Rectangle((int)EndscreenTillLobbyVector.X, (int)EndscreenTillLobbyVector.Y, 184, 57);
                    Rectangle EndscreenTillExitRect2 = new Rectangle((int)EndscreenTillExitVector.X, (int)EndscreenTillExitVector.Y, 184, 57);
                    //gör så att man kommer till lobbyn om man trycker på knappen med musen
                    if (mouseState5.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect2.Contains(mousePosition5))
                        {
                            timerstarta = false;
                            player1ready = 0;
                            player2ready = 0;
                            Player1.Kills = 0;
                            Player2.Kills = 0;
                            timeleft = 30;
                            timeleftMinuter = 0;
                            allShots.Clear();
                            allShots2.Clear();
                            allsekundar.Clear();
                            allsekundar2.Clear();
                            allAnimations.Clear();
                            allAnimations2.Clear();
                            freezeTimer = 6;
                            //Application.Restart();
                            //möjlig bättre lösning
                            //Application.Restart();
                            currentGameState = GameState.Start;
                        }
                    }
                    //gör så det ser ut som att man trycker på knappen till lobbyn, när man trycker på den med musen
                    if (mouseState5.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect2.Contains(mousePosition5))
                        {
                            EndscreenTillLobbySynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillLobbySynlig = false;
                    }
                    //////////////////////////////////
                    //gör så att spelet återupptas om man trycker på knappen med musen
                    if (mouseState5.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect2.Contains(mousePosition5))
                        {
                            Application.Exit();
                        }
                    }
                    //gör så det ser ut som att man trycker på tillbakaknappen, när man trycker på den med musen
                    if (mouseState5.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect2.Contains(mousePosition5))
                        {
                            EndscreenTillExitSynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillExitSynlig = false;
                    }
                    PrevMsingameMeny = mouseState5;
                    if (ks3.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        Application.Exit();
                    }
                    prevks2 = ks3;
                    break;
                case GameState.draw:
                     KeyboardState ks4 = Keyboard.GetState();
                     if (ks4.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F12) && prevKs.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F12))
                     {
                         graphics.ToggleFullScreen();
                     }
                    if (ks4.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Q))
                    {
                        timerstarta = false;
                        player1ready = 0;
                        player2ready = 0;
                        Player1.Kills = 0;
                        Player2.Kills = 0;
                        timeleft = 30;
                        timeleftMinuter = 0;
                        allShots.Clear();
                        allShots2.Clear();
                        allsekundar.Clear();
                        allsekundar2.Clear();
                        allAnimations.Clear();
                        allAnimations2.Clear();
                        freezeTimer = 6;
                        //Application.Restart();
                        //möjlig bättre lösning
                        //Application.Restart();
                        currentGameState = GameState.Start;
                        
                    }
                     MouseState mouseState4 = Mouse.GetState();
                    var mousePosition4 = new Point(mouseState4.X, mouseState4.Y);
                    cursorPos2 = new Vector2(mouseState4.X, mouseState4.Y);
                    Rectangle EndscreenTillLobbyRect = new Rectangle((int)EndscreenTillLobbyVector.X, (int)EndscreenTillLobbyVector.Y, 184, 57);
                    Rectangle EndscreenTillExitRect = new Rectangle((int)EndscreenTillExitVector.X, (int)EndscreenTillExitVector.Y, 184, 57);
                    //gör så att man kommer till lobbyn om man trycker på knappen med musen
                    if (mouseState4.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect.Contains(mousePosition4))
                        {
                            timerstarta = false;
                            player1ready = 0;
                            player2ready = 0;
                            Player1.Kills = 0;
                            Player2.Kills = 0;
                            timeleft = 30;
                            timeleftMinuter = 0;
                            allShots.Clear();
                            allShots2.Clear();
                            allsekundar.Clear();
                            allsekundar2.Clear();
                            allAnimations.Clear();
                            allAnimations2.Clear();
                            freezeTimer = 6;
                            //Application.Restart();
                            //möjlig bättre lösning
                            //Application.Restart();
                            currentGameState = GameState.Start;
                        }
                    }
                    //gör så det ser ut som att man trycker på knappen till lobbyn, när man trycker på den med musen
                    if (mouseState4.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillLobbyRect.Contains(mousePosition4))
                        {
                            EndscreenTillLobbySynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillLobbySynlig = false;
                    }
                    //////////////////////////////////
                    //gör så att spelet återupptas om man trycker på knappen med musen
                    if (mouseState4.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Released) && PrevMsingameMeny.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect.Contains(mousePosition4))
                        {
                            Application.Exit();
                        }
                    }
                    //gör så det ser ut som att man trycker på tillbakaknappen, när man trycker på den med musen
                    if (mouseState4.LeftButton == (Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                    {
                        if (EndscreenTillExitRect.Contains(mousePosition4))
                        {
                            EndscreenTillExitSynlig = true;
                        }
                    }
                    else
                    {
                        EndscreenTillExitSynlig = false;
                    }
                    PrevMsingameMeny = mouseState4;
                    
                    
                    if (ks4.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && prevks2.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        Application.Exit();
                    }
                    prevks2 = ks4;
                    break;
            }
            base.Update(gameTime);
        }
       
        //servern tar mot data
        private void StreamReceived(IAsyncResult ar)
        {
            int byteRead = 0;
            try
            {
                lock (client.GetStream())
                {
                    byteRead = client.GetStream().EndRead(ar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (byteRead == 0)
            {
                client.Close();
                return;
            }
            byte[] data = new byte[byteRead];
            for (int i = 0; i < byteRead; i++)
                data[i] = readBuffer[i];
            ProcessData(data);
            client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
        }
        private void ProcessData(byte[] data)
        {
            readStream.SetLength(0);
            readStream.Position = 0;
            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;
            Protocol p;
            try
            {
                p = (Protocol)reader.ReadByte();
                if (p == Protocol.Connected)
                {
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    //MessageBox.Show(string.Format("Player has Connected:   {0}  the ip adress is:  {1}", id, ip));
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.GetID);
                    writer.Write(id);
                    SendData(GetDataFromMemoryStream(writeStream));
                    if (!player2Connected)  //Variabler som stalls in nar en till spelare ansluter
                    {
                        player2Connected = true;    //En bool som sager till att en spelare har anslutigt
                        Player2.Position = new Vector2(400, 100);   //satter player2 startposition
                        Player2.Angle = (float)(Math.PI);   //Satter player2 startvinkel
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.Connected);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                }
                else if (p == Protocol.Disconnected)    //Kors om en spelare lamnart spelet
                {
                    
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    player2Connected = false;
                    MessageBox.Show("Motståndaren har lämnat spelsessionen.");
                    //resetar variabler när en motståndare lämnar spelet
                    timerstarta = false;
                    player1ready = 0;
                    player2ready = 0;
                    Player1.Kills = 0;
                    Player2.Kills = 0;
                    timeleft = 10;
                    timeleftMinuter = 0;
                    allShots.Clear();
                    allShots2.Clear();
                    freezeTimer = 6;
                    //Application.Restart();
                    currentGameState = GameState.Start;
                }
                else if (p == Protocol.PlayerMoved)     //kors om spelare1 ror pa sig, uppdaterar sa att spelare2 ser att den andra har rort pa sig
                {
                    float px = reader.ReadSingle();
                    float py = reader.ReadSingle();
                    Vector2 sentPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    if (sentPos != Player2.Position)    //Kollar ifall positionen stammer, om inte sa uppdateras positionen
                    {
                        Player2.Position = sentPos;
                    }
                    Player2.Position = new Vector2(Player2.Position.X + px, Player2.Position.Y + py);   //Registrerar den nya rorelsen, gor att den andra spelaren kan se att karaktaren har rort sig
                    int fart = 100;
                    // Gå framåt
                    if (Player2.Angle == -(float)(Math.PI / 2))
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 21 || Player2.Frame >= 24)
                                Player2.Frame = 21;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //Gå bakåt
                    else if (Player2.Angle == (float)(Math.PI / 2))
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 1 || Player2.Frame >= 4)
                                Player2.Frame = 1;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    // Gå till höger
                    else if (Player2.Angle == 0)
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 31 || Player2.Frame >= 34)
                                Player2.Frame = 31;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    // Gå till vänster
                    else if (Player2.Angle == (float)(Math.PI))
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 11 || Player2.Frame >= 14)
                                Player2.Frame = 11;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //Vänaster upp
                    else if (Player2.Angle == (-(float)(Math.PI) / 4) + (2 * (-(float)(Math.PI) / 4)))
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 16 || Player2.Frame >= 19)
                                Player2.Frame = 16;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //höger upp
                    else if (Player2.Angle == -(float)(Math.PI) / 4)
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 36 || Player2.Frame >= 39)
                                Player2.Frame = 36;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //Höger ner
                    else if (Player2.Angle == (float)(Math.PI) / 4)
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 26 || Player2.Frame >= 29)
                                Player2.Frame = 26;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //Vänaster ner
                    else if (Player2.Angle == (-(float)(Math.PI) / 4) - (4 * (-(float)(Math.PI) / 4)))
                    {
                        if (timerkanskicka >= fart)
                        {
                            if (Player2.Frame < 6 || Player2.Frame >= 9)
                                Player2.Frame = 6;
                            else
                                Player2.Frame++;
                            timerkanskicka = 0;
                        }
                    }
                    //                    och skickar en extra byte om ifall det ska läsas eller inte
                    //[11:20:38] Holmis: och då behöverf man en timer
                    //[11:21:14] Miraboreasu So Ryuu: System.Timers.Timer myTimer = new System.Timers.Timer
                    //[11:21:30] Miraboreasu So Ryuu: mytimer.Interval = x antal ms
                    //[11:22:18] Miraboreasu So Ryuu: eller nått :P
                    //[11:23:09] Antti Lindén: Vi animerade med typ en improviserad timer som inte är en sån där funktionell timer.
                    //[11:23:35] Antti Lindén: Ha att den skiftar frames som den ska om hastigheten inte är 0.
                }
                else if (p == Protocol.PlayerRotated)   //Kors om spelare1 byter riktning pa sin karaktar
                {
                    float pi = reader.ReadSingle();
                    float origPi = reader.ReadSingle();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                   
                    if (Player2.Angle != origPi)    //Kollar om riktningen stammer, annars uppdateras den
                    {
                        Player2.Angle = origPi;
                    }
                    Player2.Angle = Player2.Angle + pi;     //Gor sa att karaktaren andrar riktning for spelare2 om spelare1 har andrat riktning
                    //nytt ta bort om ej fungerar
                    System.Timers.Timer mytimer = new System.Timers.Timer();
                    mytimer.Interval = 1000;
                    mytimer.Start();
                    //if (timerkanskicka >= 40)
                    //{
                    //    MessageBox.Show("timer works");
                    //}
                    //if (timerkanskicka >= 80)
                    //{
                    //    MessageBox.Show("timer works2");
                    //    timerkanskicka = 0;
                    //}
                }
                else if (p == Protocol.BulletCreated)   //Ifall spelare1 skjuter gor detta sa att det visas for spelare2
                {
                    Texture2D shotGfx;  //Satter grafiken for skottet
                    int shotType1 = reader.ReadInt32();
                    //Poitionen for karaktaren som skjuter
                    Vector2 shotPos1 = new Vector2();
                    shotPos1.X = reader.ReadSingle();
                    shotPos1.Y = reader.ReadSingle();
                    float shotAnglePi = reader.ReadSingle();    //Vinkel for skottet
                    float shotSpeed = reader.ReadSingle();      //Hastighet for skottet
                    float shotPower = reader.ReadSingle();
                    //Riktning for skottet   
                    Vector2 shotDirec = new Vector2();
                    shotDirec.X = reader.ReadSingle();
                    shotDirec.Y = reader.ReadSingle();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    if (shotType1 == 0)
                    {
                        shotGfx = shot1Gfx;
                    }
                    else
                    {
                        shotGfx = shot1Gfx;
                    }
                    allShots2.Add(new Shot()    //Lagger till ett nytt skott i spelare2 forloopen
                    {
                        Gfx = shot1Gfx,     //Tilldelar grafik for skottet
                        Position = shotPos1,     //Tilldelar position for skottet
                        Angle = shotAnglePi,    //Tilldelar vinkel for skottet   
                        Speed = shotSpeed,      //Tilldelar hastighet for skottet
                        Power = shotPower,
                        Direction = shotDirec   //Tilldelar riktning for skottet
                    });
                    Player2.ShotFired = false;     //Sager till att ett skott har avfyrats, gor att skott inte skjuts konstant i en lang linje 
                }
                else if (p == Protocol.Sekundarcreated)
                {
                    Texture2D shotGfx2;  //Satter grafiken for skottet
                    int shotType = reader.ReadInt32();
                    //Poitionen for karaktaren som skjuter
                    Vector2 shotPos = new Vector2();
                    shotPos.X = reader.ReadSingle();
                    shotPos.Y = reader.ReadSingle();
                    float shotAnglePi = reader.ReadSingle();    //Vinkel for skottet
                    float shotSpeed = reader.ReadSingle();      //Hastighet for skottet
                    float shotPower = reader.ReadSingle();
                    //Riktning for skottet   
                    Vector2 shotDirec = new Vector2();
                    shotDirec.X = reader.ReadSingle();
                    shotDirec.Y = reader.ReadSingle();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    if (shotType == 0)
                    {
                        shotGfx2 = shot1Gfx2;
                    }
                    else
                    {
                        shotGfx2 = shot1Gfx2;
                    }
                    allsekundar2.Add(new Shot()    //Lagger till ett nytt skott i spelare2 forloopen
                    {
                        Gfx = shot1Gfx2,     //Tilldelar grafik for skottet
                        Position = shotPos,     //Tilldelar position for skottet
                        Angle = shotAnglePi,    //Tilldelar vinkel for skottet   
                        Speed = shotSpeed,      //Tilldelar hastighet for skottet
                        Power = shotPower,
                        Direction = shotDirec   //Tilldelar riktning for skottet
                    });
                    Player2.ShotFired2 = false;     //Sager till att ett skott har avfyrats, gor att skott inte skjuts konstant i en lang linje 
                }
                else if (p == Protocol.GetID)
                {
                    playerIndex = reader.ReadByte();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                }
                //else if (p == Protocol.PlayerChange)
                //{
                //    player2Number = reader.ReadByte();
                //}
                else if (p == Protocol.PlayerChange)
                {
                    int tal1 = reader.ReadByte();
                    player2Number = tal1;
                    if (player2Number == 1)
                    {
                        player1Number = 2;
                    }
                    else if (player2Number == 2)
                    {
                        player1Number = 1;
                    }
                    //player1Number = player1Number + 1;
                }
                else if (p == Protocol.StateChange)
                {
                    int stateswitch = reader.ReadByte();
                    switch (stateswitch)
                    {
                        case 1: //Startar spelet
                            currentGameState = GameState.Game;
                            break;
                        case 2: //Mostandaren har dott
                            Player1.Position = p1Spawn;
                            Player2.Position = p2Spawn;
                            Player1.Life = 200F;
                            Player2.Life = 200F;
                            allShots.Clear();
                            allShots2.Clear();
                            allAnimations.Clear();
                            allAnimations2.Clear();
                            Player1.Kills += 1;
                            diedtext = 3; //gör så att text skrivs ut som meddelar att du dödade motståndaren
                            timer1 = 6;
                            break;
                    }
                }
                else if (p == Protocol.Killstowinchanged)
                {
                    int winkills = reader.ReadByte();
                    if (killstowin != winkills)
                    {
                        killstowin = winkills;
                    }
                }
                else if (p == Protocol.Timeforroundchanged)
                {
                    float timelefttokill = reader.ReadSingle();
                    float timelefttokillminuter = reader.ReadSingle();
                    if (timeleft != timelefttokill)
                    {
                        timeleft = timelefttokill;
                    }
                    if (timeleftMinuter != timelefttokillminuter)
                    {
                    timeleftMinuter = timelefttokillminuter;
                    }
                }
                else if (p == Protocol.PlayerReady)
                {
                    int redo = reader.ReadByte();
                    if (player2ready != redo)
                    {
                        player2ready = redo;
                    }
                }
                else if (p == Protocol.PlayerStoped)    //Ifall spelaren stannar så uppdateras framen på player2
                {
                    float farten = reader.ReadSingle();
                    if (farten == Player2.Speed)
                    {
                        // Gå framåt
                        if (Player2.Angle == -(float)(Math.PI / 2))
                        {
                            Player2.Frame = 20;
                        }
                        //Gå bakåt
                        else if (Player2.Angle == (float)(Math.PI / 2))
                        {
                            Player2.Frame = 0;
                        }
                        // Gå till höger
                        else if (Player2.Angle == 0)
                        {
                            Player2.Frame = 30;
                        }
                        // Gå till vänster
                        else if (Player2.Angle == (float)(Math.PI))
                        {
                            Player2.Frame = 10;
                        }
                        //Vänaster upp
                        else if (Player2.Angle == (-(float)(Math.PI) / 4) + (2 * (-(float)(Math.PI) / 4)))
                        {
                            Player2.Frame = 15;
                        }
                        //höger upp
                        else if (Player2.Angle == -(float)(Math.PI) / 4)
                        {
                            Player2.Frame = 35;
                        }
                        //Höger ner
                        else if (Player2.Angle == (float)(Math.PI) / 4)
                        {
                            Player2.Frame = 25;
                        }
                        //Vänaster ner
                        else if (Player2.Angle == (-(float)(Math.PI) / 4) - (4 * (-(float)(Math.PI) / 4)))
                        {
                            Player2.Frame = 5;
                        }
                    
                    }
                
                }
               
               
                
                    
                
            }
            catch (Exception ex)    //Ifall nagot blir fel gor detta sa att spelet inte kraschar, utan ett felmeddelande visas istallet
            {
                //MessageBox.Show("Error code 2001: Det blev fel någonstans bland protokollen");
                //MessageBox.Show(ex.Message);
            }
        }
        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;
            //Async method called this, so lets lock the object to make sure other threads/async calls need to wait to use it.
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];
                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }
            return result;
        }
        /// <summary>
        /// Code to actually send the data to the client
        /// </summary>
        /// <param name="b">Data to send</param>
        public void SendData(byte[] b)
        {
            //Try to send the data.  If an exception is thrown, disconnect the client
            try
            {
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch
            {
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            
            string p1Number = "Player " + player1Number;
            string p2Number = "Player " + player2Number;
            string killsneed = killstowin.ToString();
           
            string shit ;
            if (timeleftMinuter < 10 && timeleftMinuter >= 0)
            {
                shit = "0";
            }
            else
            {
                shit = null;
            }
            string tidspel = shit + timeleftMinuter.ToString() + ":" + timeleft.ToString();
            string tidspel2 = shit + timeleftMinuter.ToString() + ":" + "0" + timeleft.ToString();
            
            
                
            switch (currentGameState)
            {
                case GameState.Start:
                    //ritar ut bakgrunden
                    spriteBatch.Draw(lobbyGfx, lobbyVector, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f);
                    //ritar ut grafik för att visa att spelarna är redo
                    if (player1ready >= 1)
                    {
                        spriteBatch.Draw(readyGfx, readyvector, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);
                    }
                    if (player2ready >= 1)
                    {
                        spriteBatch.Draw(readyGfx, readyvector2, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);
                    }
    
                    spriteBatch.DrawString(segoe, killsneed, new Vector2(180, 391), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);//ritar ut text för hur många kills som man måste ha för att vinna.
                 
                    if (timeleft < 10 && timeleft >= 0)
                    {
                        spriteBatch.DrawString(segoe, tidspel2, new Vector2(170, 482), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);//ritar ut text för hur lång en spelrunda är
                    }
                    else
                    {
                        spriteBatch.DrawString(segoe, tidspel, new Vector2(170, 482), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);//ritar ut text för hur lång en spelrunda är
                    }
                    spriteBatch.DrawString(segoe, p1Number, new Vector2(114, 220), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f); //ritar ut text som säger vilken spelare man är 
                    
                    if (player2Connected)
                        spriteBatch.DrawString(segoe, p2Number, new Vector2(588, 220), Color.White,
                            0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f); //ritar ut text som säger vilken spelare man är 
                    spriteBatch.Draw(nextkillGfx, nextkillVector, null,Color.White,0,Vector2.Zero,1f,SpriteEffects.None,0.5f);
                    spriteBatch.Draw(nextkillGfx2, nextkillVector2, null,Color.White,0,Vector2.Zero, 1f,SpriteEffects.None,0.5f);
                    spriteBatch.Draw(nextkillGfx, nextkillVector3, null, Color.White,0, Vector2.Zero, 1f,SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(nextkillGfx2, nextkillVector4, null,Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                    
                    if (nextboxdownCheck1 == true)
                    {
                        spriteBatch.Draw(nextkillGfx_down, nextkillVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.51f);
                    }
                    if (nextboxdownCheck2 == true)
                    {
                        spriteBatch.Draw(nextkillGfx2_down, nextkillVector2, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.51f);
                    }
                    if (nextbox2downCheck1 == true)
                    {
                        spriteBatch.Draw(nextkillGfx_down, nextkillVector3, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.51f);
                    }
                    if (nextbox2downCheck2 == true)
                    {
                        spriteBatch.Draw(nextkillGfx2_down, nextkillVector4, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.51f);
                    }
                    
                    //ritar ut countdowntimer i lobby för att starta spelet
                    string gogogo = tidförstart.ToString();
                    if (timerstarta == true)
                    {
                        //ritar ut grafik för countdown timer
                        spriteBatch.Draw(lobbytimerGfx, lobbytimerVector, null, Color.White, 0, new Vector2(lobbytimerGfx.Width / 2, lobbytimerGfx.Height / 2), 1.0f, SpriteEffects.None, 0.9999f);
                        spriteBatch.DrawString(startaspellobby, gogogo, new Vector2(362, 205), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.99999f);
                    }
                    //Ritar ut mucklick-knappar i lobbyn
                    if (lobbybackknappsynlig == true)//back knappen
                    {
                        spriteBatch.Draw(LobbyKnappBackGfx, LobbyKnappBackVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.999f);
                    }
                    
                    if (lobbyEnterknappsynlig == true)//enter knappen
                    {
                        spriteBatch.Draw(LobbyKnappEnterGfx, LobbyKnappEnterVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.999f);
                    }
                    
                    //Ritar ut muspekare
                    spriteBatch.Draw(cursorTex2, cursorPos2, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    
                    break;
                case GameState.Game:
                    Bana.Draw(spriteBatch, Bana.Position,0f);
                    //daloSpawn.Draw(spriteBatch, Player1.Position,0.001f);
                    //ltdSpawn.Draw(spriteBatch, Player1.Position,0.001f);
                    for (int i = 0; i < tilepositionsGolv.Count; i++)
                    {
                        tilepositionsGolv[i].Draw(spriteBatch, Player1.Position, 0);
                    }
                  
                        if (Player1 != null) Player1.Draw(spriteBatch, Player1.Position, levelHeight);   //Om spelare1 existerar sa ritas grafiken for den ut
                    
                    if (player2Connected) Player2.Draw(spriteBatch, Player1.Position, levelHeight);  //Om spelare2 ansluter sa ritas grafiken for den ut, annars syns den inte
                    
                    ////ritar ut banan
                    //for (int i = 0; i < tilepositions.Count; i++)
                    //{
                    //    tilepositions[i].Draw(spriteBatch, Player1.Position);
                    //}
                    
                    for (int i = 0; i < tilepositions.Count; i++)
                    {
                        tilepositions[i].Draw(spriteBatch, Player1.Position, levelHeight);
                        
                    }
                    for (int i = 0; i < tilepositions2.Count; i++)
                    {
                        tilepositions2[i].Draw(spriteBatch, Player1.Position, levelHeight);
                    }
                    //Loopar igenom alla skott och ritar ut dem
                    if (player2Connected == true)
                    {
                        for (int i = 0; i < allShots.Count; i++)
                        {
                            allShots[i].Draw(spriteBatch, Player1.Position, levelHeight);
                                for (int j = 0; j < tilepositions.Count; j++)
                                {
                                    if (tilepositions[j].skottisover == true)
                                    {
                                        //allShots[i].Draw(spriteBatch, Player1.Position, levelHeight - tilepositions[j].Position.Y);
                                        allShots[i].Draw(spriteBatch, Player1.Position, levelHeight - allShots[i].Position.Y);
                                    }
                                    
                                }
                           
                        }
                        for (int i = 0; i < allShots2.Count; i++)
                        {
                            allShots2[i].Draw(spriteBatch, Player1.Position, levelHeight);
                            for (int j = 0; j < tilepositions.Count; j++)
                            {
                                if (tilepositions[j].skottisover == true)
                                {
                                    allShots2[i].Draw(spriteBatch, Player1.Position, levelHeight - allShots2[i].Position.Y);
                                }
                            }
                        }
                    }
                    if (player2Connected == true)
                    {
                        //sekundarattack for spelare 1
                        for (int i = 0; i < allsekundar.Count; i++)
                        {
                            allsekundar[i].Draw(spriteBatch, Player1.Position, levelHeight);
                        }
                        //sekundarattack for spelare 2
                        for (int i = 0; i < allsekundar2.Count; i++)
                        {
                            allsekundar2[i].Draw(spriteBatch, Player1.Position, levelHeight);
                        }
                    }
                    
                    // ritar ut animation för spelare1, huvudattack
                    for (int i = 0; i < allAnimations.Count; i++)
                    {
                        allAnimations[i].Draw(spriteBatch, Player1.Position, levelHeight);
                    }
                    // ritar ut animation för spelare2, huvudattack
                    for (int i = 0; i < allAnimations2.Count; i++)
                    {
                        allAnimations2[i].Draw(spriteBatch, Player1.Position, levelHeight);
                    }
                    
                    //ritar ut hpbarbakgrunden
                    spriteBatch.Draw(hpBarBGGfx, hpBarBG, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.99f);
                    //ritar ut hpbar
                    hpBar.Draw(spriteBatch, new Vector2(0, 0), 1F);
                   
                    //Ritar ut mussiktet
                    spriteBatch.Draw(cursorTex, cursorPos, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                1f);
                    // ritar ut hp
                    string nameFormat = "{0}";
                    displayMessage = string.Format(nameFormat, Player1.Life);
                    spriteBatch.DrawString(font, displayMessage, new Vector2(28, 20), Color.Black,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.999f);
                    //ritar ut Cooldown på sekundärattack
                    if (sekundarisready == false)
                    {
                        string CD = cooldownSekundär.ToString();
                        spriteBatch.DrawString(font, CD, new Vector2(370, 17), Color.Black, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.99999f);
                        spriteBatch.Draw(slownotreadyGfx, slowreadyVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.999f);
                    }
                    if (sekundarisready == true)
                    {
                        spriteBatch.Draw(slowreadyGfx, slowreadyVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.999f);
                    }
                    //ritar ut kills/tidpanelen
                    spriteBatch.Draw(killbarGfx, killbarvector, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.951f);
                   //ritar ut antal kills för player 1
                    string spelare1kills = Player1.Kills.ToString();
                    spriteBatch.DrawString(fontkillsplayer1, spelare1kills, new Vector2(463, 37), Color.Black,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.999f);
                    //ritar ut antal kills för player 2
                    string spelare2kills = Player2.Kills.ToString();
                    spriteBatch.DrawString(fontkillsplayer1, spelare2kills, new Vector2(597, 37), Color.Black,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.999f);
                    //ritar ut hur mycket tid som är kvar i sekunder
                    //string timerforgameend = timeleftMinuter.ToString() + ":" + timeleft.ToString();
                      string shit2 ;
                      if (timeleftMinuter < 10 && timeleftMinuter >= 0)
                        {
                             shit2 = "0";
                        }
                     else
                        {
                             shit2 = null;
                        }
            string timerforgameend = shit2 + timeleftMinuter.ToString() + ":" + timeleft.ToString();
            string timerforgameend2 = shit2 + timeleftMinuter.ToString() + ":" + "0" + timeleft.ToString();
            if (timeleft < 10 && timeleft >= 0)
            {
                spriteBatch.DrawString(fontkillsplayer1, timerforgameend2, new Vector2(713, 37), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.999f);
            }
            else
            {
                spriteBatch.DrawString(fontkillsplayer1, timerforgameend, new Vector2(713, 37), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.999f);
            }
                    //ritar ut text som säger att du dog
                    if (killedtext > 0)
                    {
                        spriteBatch.Draw(statustextGfx2, statustextVector2, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.999f);
                        freezeTimer = 6;
                    }
                    //ritar ut text som säger att du dödade
                    if (diedtext > 0)
                    {
                        spriteBatch.Draw(statustextGfx, statustextVector, null,
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.999f);
                        freezeTimer = 6;
                    }
                    //ritar ut freeze timer
                    switch (threeTwoOneGo)
                    {
                        case freezecounter.three:
                            spriteBatch.Draw(countTex3, counttexVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            //spriteBatch.DrawString(countFont, "3", new Vector2((graphics.PreferredBackBufferWidth / 2) - 80, (graphics.PreferredBackBufferHeight / 2) - 180), Color.DarkGray * 0.8f, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
                            break;
                        case freezecounter.two:
                            spriteBatch.Draw(countTex2, counttexVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            //spriteBatch.DrawString(countFont, "2", new Vector2((graphics.PreferredBackBufferWidth / 2) - 80, (graphics.PreferredBackBufferHeight / 2) - 180), Color.DarkGray * 0.8f, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
                            break;
                        case freezecounter.one:
                            spriteBatch.Draw(countTex1, counttexVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            //spriteBatch.DrawString(countFont, "1", new Vector2((graphics.PreferredBackBufferWidth / 2) - 80, (graphics.PreferredBackBufferHeight / 2) - 180), Color.DarkGray * 0.8f, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
                            break;
                        case freezecounter.go:
                            spriteBatch.Draw(countTex, counttexVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            break;
                        default:
                            break;
                    }
                    //ritar ut i speletmenyn
                    if (iMeny == true)
                    {
                        spriteBatch.Draw(menyGfx, menyVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                        if (ispeletmenyKnappExitSynlig == true)
                        {
                            spriteBatch.Draw(ispeletmenyKnappExitGfx, ispeletmenyKnappExitVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9999f);
                        }
                        if (ispeletmenyKnappKontrollerSynlig == true)
                        {
                            spriteBatch.Draw(ispeletmenyKnappKontrollerGfx, ispeletmenyKnappKontrollerVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9999f);
                        }
                        if (ispeletmenyKnappMenySynlig == true)
                        {
                            spriteBatch.Draw(ispeletmenyKnappMenyGfx, ispeletmenyKnappMenyVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9999f);
                        }
                    }
                    //ritar ut kontrollinfo delen av menyn
                    if (iMenyKontroller == true)
                    {
                        spriteBatch.Draw(menyKontrollerGfx, menyKontrollerVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99999f);
                        if (ispeletmenyKnappBackSynlig == true)
                        {
                            spriteBatch.Draw(ispeletmenyKnappBackGfx, ispeletmenyKnappExitVector, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.999999f);
                        }
                    }
                    
                    break;
                case GameState.victory:
                    spriteBatch.Draw(menyvictoryGfx, menyvictoryVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    //Ritar ut muspekare
                    spriteBatch.Draw(cursorTex2, cursorPos2, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    //ritar ut musknappar
                    if (EndscreenTillLobbySynlig == true)
                    {
                        spriteBatch.Draw(EndscreenTillLobbyGfx, EndscreenTillLobbyVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    if (EndscreenTillExitSynlig == true)
                    {
                        spriteBatch.Draw(EndscreenTillExitGfx, EndscreenTillExitVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    //ritar ut antal kills för player 1
                    string victoryspelare1kills =Player1.Kills.ToString();
                    spriteBatch.DrawString(finishgame , victoryspelare1kills, new Vector2(227, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut antal kills för player 2
                    string victoryspelare2kills =Player2.Kills.ToString();
                    spriteBatch.DrawString(finishgame, victoryspelare2kills, new Vector2(542, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut hur mycket tid som är kvar i sekunder
                    //string victorytimerforgameend = timeleft.ToString();
                    //spriteBatch.DrawString(finishgame, victorytimerforgameend, new Vector2(380, 300), Color.White,
                    //    0,
                    //    Vector2.Zero,
                    //    1,
                    //    SpriteEffects.None,
                    //    0.99f);
                     string shit3 ;
                      if (timeleftMinuter < 10 && timeleftMinuter >= 0)
                        {
                             shit3 = "0";
                        }
                     else
                        {
                             shit3 = null;
                        }
                    string victorytimerforgameend = shit3 + timeleftMinuter.ToString() + ":" + timeleft.ToString();
                    string victorytimerforgameend2 = shit3 + timeleftMinuter.ToString() + ":" + "0" + timeleft.ToString();
            if (timeleft < 10 && timeleft >= 0)
            {
                spriteBatch.DrawString(finishgame, victorytimerforgameend2, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            }
            else
            {
                spriteBatch.DrawString(finishgame, victorytimerforgameend, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            }
                    break;
                case GameState.lost:
                    spriteBatch.Draw(menylostGfx, menylostVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    //Ritar ut muspekare
                    spriteBatch.Draw(cursorTex2, cursorPos2, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    //ritar ut musknappar
                    if (EndscreenTillLobbySynlig == true)
                    {
                        spriteBatch.Draw(EndscreenTillLobbyGfx, EndscreenTillLobbyVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    if (EndscreenTillExitSynlig == true)
                    {
                        spriteBatch.Draw(EndscreenTillExitGfx, EndscreenTillExitVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    //ritar ut antal kills för player 1
                    string lostspelare1kills = Player1.Kills.ToString();
                    spriteBatch.DrawString(finishgame, lostspelare1kills, new Vector2(227, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut antal kills för player 2
                    string lostspelare2kills = Player2.Kills.ToString();
                    spriteBatch.DrawString(finishgame, lostspelare2kills, new Vector2(542, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut hur mycket tid som är kvar i sekunder
                    //string losttimerforgameend = timeleft.ToString();
                    //spriteBatch.DrawString(finishgame, losttimerforgameend, new Vector2(380, 300), Color.White,
                    //    0,
                    //    Vector2.Zero,
                    //    1,
                    //    SpriteEffects.None,
                    //    0.99f);
                    
                     string shit4 ;
                      if (timeleftMinuter < 10 && timeleftMinuter >= 0)
                        {
                             shit4 = "0";
                        }
                     else
                        {
                             shit4 = null;
                        }
                      string losttimerforgameend = shit4 + timeleftMinuter.ToString() + ":" + timeleft.ToString();
                      string losttimerforgameend2 = shit4 + timeleftMinuter.ToString() + ":" + "0" + timeleft.ToString();
                      if (timeleft < 10 && timeleft >= 0)
                      {
                          spriteBatch.DrawString(finishgame, losttimerforgameend2, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                      }
                      else
                      {
                          spriteBatch.DrawString(finishgame, losttimerforgameend, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                      }
                    break;
                case GameState.draw:
                    spriteBatch.Draw(menydrawGfx, menydrawVector, null,Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    //Ritar ut muspekare
                    spriteBatch.Draw(cursorTex2, cursorPos2, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    //ritar ut musknappar
                    if (EndscreenTillLobbySynlig == true)
                    {
                    spriteBatch.Draw(EndscreenTillLobbyGfx, EndscreenTillLobbyVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    if (EndscreenTillExitSynlig == true)
                    {
                        spriteBatch.Draw(EndscreenTillExitGfx, EndscreenTillExitVector, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    //ritar ut antal kills för player 1
                    string drawspelare1kills = Player1.Kills.ToString();
                    spriteBatch.DrawString(finishgame, drawspelare1kills, new Vector2(227, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut antal kills för player 2
                    string drawspelare2kills = Player2.Kills.ToString();
                    spriteBatch.DrawString(finishgame, drawspelare2kills, new Vector2(542, 300), Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.99f);
                    //ritar ut hur mycket tid som är kvar i sekunder
                    //string drawtimerforgameend = timeleft.ToString();
                    //spriteBatch.DrawString(finishgame, drawtimerforgameend, new Vector2(380, 300), Color.White,
                    //    0,
                    //    Vector2.Zero,
                    //    1,
                    //    SpriteEffects.None,
                    //    0.99f);
                       string shit5 ;
                      if (timeleftMinuter < 10 && timeleftMinuter >= 0)
                        {
                             shit5 = "0";
                        }
                     else
                        {
                             shit5 = null;
                        }
                      string drawtimerforgameend = shit5 + timeleftMinuter.ToString() + ":" + timeleft.ToString();
                      string drawtimerforgameend2 = shit5 + timeleftMinuter.ToString() + ":" + "0" + timeleft.ToString();
                      if (timeleft < 10 && timeleft >= 0)
                      {
                          spriteBatch.DrawString(finishgame, drawtimerforgameend2, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                      }
                      else
                      {
                          spriteBatch.DrawString(finishgame, drawtimerforgameend, new Vector2(358, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                      }
                    break;
            }
           
            spriteBatch.End();
            base.Draw(gameTime);
        }
        public void LaddaLevel( int nummer )
        {
            string banaNamn = "hej.txt";
           
                StreamReader readbana = File.OpenText(banaNamn);
                string bana = readbana.ReadToEnd();
            
            
            int temp_positionY = 1;
            int temp_positionX = 0;
            //int xoffset = -750;
            //int yoffset = -400;
            int xoffset = 0;
            int yoffset = 0;
            
            for (int i = 0; i < bana.Length; i++)
            {
                tilepositionsGolv.Add(new walls() //Ritar ut golv på varje tile, men de plaseras alltid längst ner
                {
                    Gfx = tile_gfxReal,
                    Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),     
                    Angle = -(float)(Math.PI / 2),
                    bild = 24,
                });
                
                    switch (bana[i])
                    {
                        case ' ':
                           
                            temp_positionX++;
                            break;
                        case '@'://Spawnpoint för Spelare 1
                            daloSpawn = new GameObj()
                            {
                                Gfx = spawnPlate,
                                Position = new Vector2(xoffset + temp_positionX * 64, yoffset + temp_positionY * 64),
                            };
                            temp_positionX++;
                            break;
                        case '&'://Spawnpoint för Spelare 2
                            ltdSpawn = new GameObj()
                            {
                                Gfx = spawnPlate,
                                Position = new Vector2(xoffset + temp_positionX * 64, yoffset + temp_positionY * 64),
                            };
                            temp_positionX++;
                            break;
                        case '0':
                        case '1'://Låg vägg, vänsterkant
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 43,
                                skottkollision = false,
                                bildTop = 40,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case '2': //låg vägg, horisontell mitt
                          tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 44,
                                skottkollision = false,
                                bildTop = 41,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case '3': //låg vägg, högerkant
                            
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 45,
                                skottkollision = false,
                                bildTop = 42,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case '4': //låg vägg, topp/botten
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 48,
                                skottkollision = false,
                                bildTop = 46,
                                wall = true,
                            });
                            temp_positionX++;
                           
                            break;
                        case '5':  //Låg vägg, mitt/botten
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 48,
                                skottkollision = false,
                                bildTop = 47,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'A':  //Hög vägg, vänsterdel
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 0,
                                skottkollision = true,
                                bildTop = 5,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'B':  //Hög vägg, mittendel horisontell
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 1,
                                skottkollision = true,
                                bildTop = 6,
                                wall = true,
                            });
                            temp_positionX++;
                            
                            break;
                        case 'C':  //Hög vägg, högerdel
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 2,
                                skottkollision = true,
                                bildTop = 7,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'N':  //Hög vägg, toppdel
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 49,
                                skottkollision = true,
                                bildTop = 17,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'L':  //Hög vägg, mittendel horisontell
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 49,
                                skottkollision = true,
                                bildTop = 15,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'M':  //Hög vägg, bottendel
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 3,
                                skottkollision = true,
                                bildTop = 16,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'H':  //Hög vägg, NÖ hörn
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 49,
                                skottkollision = true,
                                bildTop = 11,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'I':  //Hög vägg, NV hörn
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 49,
                                skottkollision = true,
                                bildTop = 12,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'J':  //Hög vägg, SÖ hörn
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 2,
                                skottkollision = true,
                                bildTop = 13,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case 'K':  //Hög vägg, SV hörn
                            tilepositions.Add(new walls()
                            {
                                Gfx = tile_gfxReal,
                                Position = new Vector2((xoffset + temp_positionX * 64), (yoffset + temp_positionY * 64)),
                                Angle = -(float)(Math.PI / 2),
                                bild = 0,
                                skottkollision = true,
                                bildTop = 14,
                                wall = true,
                            });
                            temp_positionX++;
                            break;
                        case '\n':
                            temp_positionY++;
                            temp_positionX = 0;
                            break;
                        
                    }
                    levelHeight = (float)temp_positionY * 64f;
            }
        }
    }
}
