/*******************************************************************************************************************//**
 * @file Game1.cs
 * @note This game is now dependent on the Farseer Physics Engine.
 * For information see http://farseerphysics.codeplex.com/
 * @author (For all textures) Joe Brooksbank
 **********************************************************************************************************************/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SuperSmashPolls.Characters;
using SuperSmashPolls.GameItemControl;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.Levels;
using SuperSmashPolls.MenuControl;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls {

    /***************************************************************************************************************//** 
     * This is the main type of the game.
     ******************************************************************************************************************/ 
    public class Game1 : Microsoft.Xna.Framework.Game {

        /* The total size of the screen */
        private static Vector2 ScreenSize;
        /* The most basic Functioning WorldUnit */
        private readonly WorldUnit EmptyUnit;
        /* The display size for the floor */
        private readonly Vector2 FloorDisplaySize;
        /* The scale of how many pixels are equal to one meter */
        private readonly float PixelToMeterScale;

        /*   Characters   */

            /** The one, the only, the Donald */
            private Character TheDonald;

        /*  Levels  */

            private LevelHandler Temple;

            private LevelHandler Space;

        /* Manages graphics. */
        private GraphicsDeviceManager Graphics;
        /* Used to draw multiple 2D textures at one time */
        private SpriteBatch Batch;
        /* A basic font to use for essentially everything in the game */
        private SpriteFont GameFont;
        /* Yarr, dis here be da world */
        private World GameWorld;
        /* The center of the screen */
        private Vector2 ScreenCenter;

        /* Menu system for the game to use */
        private MenuItem Menu;

        /** The player's in this game */
        private PlayerClass PlayerOne, PlayerTwo, PlayerThree, PlayerFour;
        /* The number of players in the game */
        private int NumPlayers;

        /** Handles the different states that the game can be in */
        enum GameState {

            Menu,           //The menu is open
            GameLevel,      //The first level of the game
            ScoreScreen,
            LoadSave,
            SaveGame

        };
        /** Variable to hold the state of the game */
        private GameState State = GameState.Menu;

        /* Holds levels for matching from a save and for selection */
        private List<Tuple<LevelHandler, string>> LevelStringPairs;
        /* Holds characters for matching from a save and for selection*/
        private List<Tuple<Character, string>> CharacterStringPairs;

        /***********************************************************************************************************//** 
         * Constructs the game's class
         * TODO clean up constructor
         **************************************************************************************************************/
        public Game1() {
            /* !!! The size of the screen for the game !!! (this should be saved in options) */
            ScreenSize = new Vector2(640, 360);

            LevelStringPairs     = new List<Tuple<LevelHandler, string>>();
            CharacterStringPairs = new List<Tuple<Character, string>>();

            /* This is the player's screen controller */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen = false,
                PreferredBackBufferHeight = (int) ScreenSize.Y,
                PreferredBackBufferWidth  = (int) ScreenSize.X
            };

            /* This is to import pictures and sounds and stuff */
            Content.RootDirectory = "Content";

            EmptyUnit = new WorldUnit(ref ScreenSize, new Vector2(0, 0));

            //FloorDisplaySize = new Vector2(ScreenSize.Y * 0.05F, ScreenSize.X);

            //This is equal to how many pixels are in one meter
            PixelToMeterScale = ScreenSize.X/25;

        }

        /***********************************************************************************************************//** 
         * Get's the meters of something drawn in a 640x360 scale
         **************************************************************************************************************/
        private float InMeters(float pixels) {

            return (pixels/640)*25;

        }

        /***********************************************************************************************************//** 
         * Allows the game to perform any initialization it needs to before starting to run. 
         * This is where it can query for any required services and load any non-graphic related content. Calling 
         * base.Initialize will enumerate through any components and initialize them as well.
         **************************************************************************************************************/
        protected override void Initialize() {

            /*********************************** Initialization for Physics things ************************************/

            // This sets the width of the screen equal to 25m in the physics engine
            ConvertUnits.SetDisplayUnitToSimUnitRatio(PixelToMeterScale);

            GameWorld = new World(new Vector2(0F, 9.80F)); //Creates the GameWorld with 9.82m/s^2 as downward acceleration

            ScreenCenter = ScreenSize / 2F;

            /************************************ Initialization for Level things *************************************/

            Temple = new LevelHandler(Vector2.Zero, new Vector2(4, 0), new Vector2(6, 0), new Vector2(8, 0),
                new Vector2(13.5F, 0));

            Texture2D TempleRock = Content.Load<Texture2D>("TempleRock");

            Temple.AssignToWorld(ref GameWorld,
                new Tuple<Texture2D, Vector2, Vector2>(TempleRock, new Vector2(0,
                        InMeters(360) - InMeters(TempleRock.Height)),
                    new Vector2(InMeters(TempleRock.Width), InMeters(TempleRock.Height))));

            Texture2D SpaceBackground = Content.Load<Texture2D>("space");

            Temple.SetBackground(SpaceBackground, new Vector2(SpaceBackground.Width, SpaceBackground.Height)/ScreenSize);

            LevelStringPairs.Add(new Tuple<LevelHandler, string>(Temple, "Temple"));

            //Space = new LevelHandler();

            //Texture2D TempBackground = Content.Load<Texture2D>("space");

            //Space.SetBackground(TempBackground,
            //    new Vector2(TempBackground.Width/ScreenSize.X, TempBackground.Height/ScreenSize.Y));

            /************************************* Initialization for Menu things *************************************/
            //@note Some menus hold items for other things to make the menu system more compact, don't worry about it.

            Menu = new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0, 0)), "", true,
                new WorldUnit(ref ScreenSize, new Vector2(0, 0)), false);

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), "Local Game", false,
                EmptyUnit, true, true, MenuCommands.SingleplayerMenu));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "New Game", true, EmptyUnit, true, true, MenuCommands.StartGame));

                    //This holds the in game pause screen for any amount of players
                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "One Player", false, 
                                     EmptyUnit, true, true, MenuCommands.OnePlayer));

                    //This holds the level selection screen for any amount of players
                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Two Player", false,
                                     EmptyUnit, true, true, MenuCommands.TwoPlayer));

                        Menu.ContainedItems[0].ContainedItems[0].ContainedItems[1].AddItem(
                            new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "Temple", false,
                                EmptyUnit, true, true, MenuCommands.PlayTemple));

            //This holds character selection for any amount of players
                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Three Player", false, 
                                     EmptyUnit, true, true, MenuCommands.ThreePlayer));

                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.5F)), "Four Player", false, 
                                     EmptyUnit, true, true, MenuCommands.FourPlayer));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)),
                    "Load Game", false, EmptyUnit, true, true, MenuCommands.LoadSave));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)),
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.50F, 0.30F)), "Multi Player", false,
                EmptyUnit, true, true, MenuCommands.MultiplayerMenu));

                Menu.ContainedItems[1].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Exit", false,
                EmptyUnit, true, true, MenuCommands.ExitGame));

            /************************************** Initialization for Players ****************************************/

            PlayerOne   = new PlayerClass(PlayerIndex.One);
            PlayerTwo   = new PlayerClass(PlayerIndex.Two);
            PlayerThree = new PlayerClass(PlayerIndex.Three);
            PlayerFour  = new PlayerClass(PlayerIndex.Four);

            /************************************* Initialization for Characters **************************************/

            //!@note These values are based off of the real Donald
            TheDonald = new Character(ref ScreenSize, ConvertUnits.ToDisplayUnits(new Vector2(1.88F, 0.6F)), 89F, 0.5F,
                0.01F, 500F, 25F, 0.1F, 1F, "TheDonald");

            CharacterStringPairs.Add(new Tuple<Character, string>(TheDonald, "TheDonald"));

            TheDonald.CreateBody(ref GameWorld, new Vector2(0, 0)); //TODO move this to after world selection

            base.Initialize();

        }

        /***********************************************************************************************************//** 
         * LoadContent will be called once per game and is the place to load all of your content.
         * @note The menu is created here
         **************************************************************************************************************/
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(GraphicsDevice);

            GameFont = Content.Load<SpriteFont>("SpriteFont1"); //Load the font in the game

            TheDonald.AddCharacterActions(
                new CharacterAction(2, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-stand")),
                new CharacterAction(1, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-jump")),
                new CharacterAction(1, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-walk")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")));
            //TODO finish animations for TheDonald

            Menu.SetFontForAll(GameFont);

        }

        /***********************************************************************************************************//** 
         * UnloadContent will be called once per game and is the place to unload all content.
         **************************************************************************************************************/
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /***********************************************************************************************************//** 
         * Allows the game to run logic such as updating the world, checking for collisions, gathering input, and 
         * playing audio.
         **************************************************************************************************************/
        protected override void Update(GameTime gameTime) {
                
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (State) {

                case GameState.Menu: { /* The player has the menu open */

                    MenuCommands CurrentCommand = Menu.UpdateMenu(PlayerIndex.One);

                    switch (CurrentCommand) {

                        case MenuCommands.OnePlayer:
                            NumPlayers = 1;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.TwoPlayer:
                            NumPlayers = 2;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.ThreePlayer:
                            NumPlayers = 3;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.FourPlayer:
                            NumPlayers = 4;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.LoadSave:
                            State = GameState.LoadSave;
                            break;
                        case MenuCommands.StartGame:
                            State = GameState.GameLevel;
                            Menu.ContainedItems[0].ContainedItems[0].Text = "Continue";  //Changes New Game
                            Menu.ContainedItems[0].ContainedItems[2].Text = "Main Menu"; //Changes Back

                            Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].AddItem(
                                new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "Save game", false,
                                    EmptyUnit, true, true, MenuCommands.SaveGame));

                            Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].AddItem(
                                new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Continue", false,
                                    EmptyUnit, true, true, MenuCommands.ResumeGame));

                            Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].SetFontForAll(GameFont);

                            PlayerOne.SetCharacter(TheDonald); //debugging
                            PlayerTwo.SetCharacter(new Character(TheDonald, GameWorld, new Vector2(8, 0)));
                            PlayerThree.SetCharacter(TheDonald);
                            PlayerFour.SetCharacter(TheDonald);

                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 0;

                            break;
                        case MenuCommands.BackToMainMenu:
                            Menu.DrawDown = -1;
                            break;
                        case MenuCommands.MultiplayerMenu:
                            Menu.DrawDown = 1;
                            break;
                        case MenuCommands.SingleplayerMenu:
                            Menu.DrawDown = 0;
                            break;
                        case MenuCommands.Nothing:
                            break;
                        case MenuCommands.SaveGame:
                            State = GameState.SaveGame;
                            break;
                        case MenuCommands.ResumeGame:
                            State = GameState.GameLevel;
                            break;
                        case MenuCommands.ExitGame:
                            this.Exit();
                            break;
                        default:
                            break;
                    } 

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                    State = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                            GameState.Menu : State;

                    switch (NumPlayers) {

                        case 4:
                            PlayerFour.UpdatePlayer(Temple.RespawnPoint);
                            goto case 3;
                        case 3:
                            PlayerThree.UpdatePlayer(Temple.RespawnPoint);
                            goto case 2;
                        case 2:
                            PlayerTwo.UpdatePlayer(Temple.RespawnPoint);
                            goto default;
                        default:
                            PlayerOne.UpdatePlayer(Temple.RespawnPoint);
                            break;

                    }

                    GameWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    break;

                } case GameState.ScoreScreen: {
                    break;
                } case GameState.SaveGame: {

                    try {

                        StreamWriter FileWriter = new StreamWriter("C:\\Users\\Public\\SmashPollsSave.txt");

                        FileWriter.WriteLine(NumPlayers);

                        switch (NumPlayers) {

                            case 4: {
                                PlayerFour.WriteInfo(ref FileWriter);
                                goto case 3;
                            } case 3: {
                                PlayerThree.WriteInfo(ref FileWriter);
                                goto case 2;
                            } case 2: { 
                                PlayerTwo.WriteInfo(ref FileWriter);
                                goto default;
                            } default: {
                                PlayerOne.WriteInfo(ref FileWriter);
                                break;
                            }

                        }

                        FileWriter.Close();

                    } catch (Exception e) {

                        Console.WriteLine("Exception: " + e.Message);

                    }

                    State = GameState.GameLevel;

                    break;

                } case GameState.LoadSave: {

                    try {

                        StreamReader FileReader = new StreamReader("C:\\Users\\Public\\SmashPollsSave.txt");

                        NumPlayers = int.Parse(FileReader.ReadLine());

                        switch (NumPlayers) {

                            case 4: {
                                PlayerFour.ReadInfo(ref FileReader, CharacterStringPairs, GameWorld);
                                goto case 3;
                            } case 3: {
                                PlayerThree.ReadInfo(ref FileReader, CharacterStringPairs, GameWorld);
                                goto case 2;
                            } case 2: {
                                PlayerTwo.ReadInfo(ref FileReader, CharacterStringPairs, GameWorld);
                                goto default;
                            } default: {
                                PlayerOne.ReadInfo(ref FileReader, CharacterStringPairs, GameWorld);
                                break;
                            }

                        }

                        FileReader.Close();

                        State = GameState.GameLevel;

                    } catch (Exception e) {

                        Console.WriteLine("Exception: " + e.Message);

                    }


                    break;

                } default: {

                    break;

                }

            }

            base.Update(gameTime);

        }

        /***********************************************************************************************************//** 
         * This is where the game draw's the screen.
         **************************************************************************************************************/
        protected override void Draw(GameTime gameTime) {

            Batch.Begin();

                switch (State) {

                    case GameState.Menu: {

                        Menu.DisplayMenu(ref Batch);

                        break;

                    } case GameState.GameLevel: {

                        Temple.DrawLevel(Batch);

                        switch (NumPlayers) {

                            case 4:
                                PlayerFour.DrawPlayer(ref Batch);
                                goto case 3;
                            case 3:
                                PlayerThree.DrawPlayer(ref Batch);
                                goto case 2;
                            case 2:
                                PlayerTwo.DrawPlayer(ref Batch);
                                goto case 1;
                            case 1:
                                PlayerOne.DrawPlayer(ref Batch);
                                break;

                        }

                        break;

                    } case GameState.SaveGame: {
                        Batch.DrawString(GameFont, "Saving game data...", ScreenCenter, Color.Black);
                        break;
                    } case GameState.LoadSave: {
                        Batch.DrawString(GameFont, "Loading game data...", ScreenCenter, Color.Black);
                        break;
                    } default: {

                        break;

                    }

                }

            Batch.End();

            base.Draw(gameTime);

        }

    }

}
