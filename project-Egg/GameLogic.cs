using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using System.Drawing; //Added to take care of the KeyboardState and enum Key
using Microsoft.DirectX.AudioVideoPlayback;

namespace project_Egg
{
    /// <summary>
    /// This class controls the main game flow and the
    /// interaction between all other game classes.
    /// </summary>

    /// <summary>
    /// Holds the various game states.
    /// </summary>
    public enum GameStates
    {
        Run = 0,
        Exit = 1
    }

    class GameLogic
    {

        // User-Defined DirectX class objects.
        protected DxInitGraphics graphics;
        protected DxKeyboard input;
        protected DxMouse mInput;


        // Various game Image Objects i.e. Backgrounds, Sprites
        protected GameBackground gameBackground;
        protected GameCursor gameCursor;
        protected GameButtons gameButtons;
        protected GameRubberband gameRubberband;
        protected GameHand gameHand;
        protected GameMap gameMap;
        protected GameWalls boWalls;
        protected GameRope boRope;
        protected GameEggPiles boEggPile;
        protected GameDinosaurs gameDinos;
        protected GameBackgroundEggs gameBackgroundEggs;
        protected GameFlyingEggs gameFlyingEggs;
        protected GameFallenEggs gameFallenEggs;
        protected GameFoot gameFoot;
        protected GameScore gameScore;
        // Game Variables
        protected GameStates gameState;
        public static ScreenState screenState;
        protected Control target;
        protected double dLoopDuration; // Duration of one game loop in milliseconds.
        protected double cursorX, cursorY;
        protected bool bMouseHandling = false;
        protected int r, c;
        protected List<GameEggs> listSameColor;
        protected List<GameEggs> listRemainEggs;
        protected List<GameEggs> listFallEggs;
        protected DxImageObject boSidebar;
        protected DxImageObject boSidebarMenuOverlap;
        protected Exception wrongPositionException;
        //protected Audio audioEggLands;
        //protected string audioFilePath ="Resources\\";
        protected static DxSound soundEggLands;
        protected static DxSound soundEggBroken;
        protected static DxSound soundBlastit;
        protected static DxSound soundClick;
        protected static DxSound soundExplosion;
        public static DxSound soundFootLands;
        public static DxSound soundFootFalls;
        public static DxSound soundWarning;
        /// <summary>
        /// Constructor. Initializes the general graphics 
        /// and starts the game loop.
        /// </summary>
        /// <param name="RenderTarget">The target control to render to.</param>
        public GameLogic(Control RenderTarget)
        {
            // Save a reference to the target Control
            this.target = RenderTarget;

            // Add an eventhandler for the GotFocus event
            this.target.GotFocus += new System.EventHandler(this.restore);
            this.target.MouseUp += target_MouseUp;
            this.target.MouseEnter += target_MouseEnter;
            this.target.MouseLeave += target_MouseLeave;
            this.target.MouseMove += target_MouseMove;
            // Create a new graphics handler
            this.graphics = new DxInitGraphics(this.target);

            // Create a new input handler
            this.input = new DxKeyboard(this.target);
            this.mInput = new DxMouse(this.target, this.graphics);

            // All done - set the game state to initialized
            this.gameState = GameStates.Run;


            // Initialize Game Objects
            this.initGameObjects();
            screenState = ScreenState.IntroScreen;

            try
            {
                // Initialize Timer
                DxTimer.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while initializing: " + ex.Message);
                return;
            }

            // Start game loop
            this.gameLoop();
        }

        void target_MouseMove(object sender, MouseEventArgs e)
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                if (this.gameMap.ListEggRows[0].YPosition <= 400) /* neu dong cuoi bi vuot qua muc 400 thi khong duoc cap van toc cho trung moi nua - neu khong se bi bug*/
                {
                    this.gameFlyingEggs.fly(this.cursorX, this.cursorY);
                }
            }
        }

        void target_MouseUp(object sender, MouseEventArgs e)
        {
            soundClick.Play();
            // click on new game button
            if (this.cursorX >= this.gameButtons.NewGameButton.XPosition &&
                this.cursorX <= this.gameButtons.NewGameButton.XPosition + this.gameButtons.NewGameButton.Width)
            {
                if (this.cursorY >= this.gameButtons.NewGameButton.YPosition &&
                    this.cursorY <= this.gameButtons.NewGameButton.YPosition + this.gameButtons.NewGameButton.Height)
                {
                    DisposeOldObject();
                    this.initGameObjects();
                    screenState = ScreenState.JungleScreen;
                    GameScore.Score = 0;
                    // play sound blastit
                    soundBlastit.Play();
                }
            }
            // click on exit button
            if (this.cursorX >= this.gameButtons.ExitButton.XPosition &&
                this.cursorX <= this.gameButtons.ExitButton.XPosition + this.gameButtons.ExitButton.Width)
            {
                if (this.cursorY >= this.gameButtons.ExitButton.YPosition &&
                    this.cursorY <= this.gameButtons.ExitButton.YPosition + this.gameButtons.ExitButton.Height)
                {
                    this.gameState = GameStates.Exit;
                }
            }
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                // Animation shot egg
                //Khi ban trung flying egg se bay di va lap tuc addNewFlyingEggs
                if (cursorX >= 150 &&
                    GameRubberband.BandStatus == BandStatus.Ready)
                {
                    GameRubberband.BandStatus = BandStatus.Finished;
                    GameBackgroundEggs.EggOnRubberband.EggStatus = EggStatus.Idle;
                    GameBackgroundEggs.EggOnRubberband.Color = GameBackgroundEggs.EggOnSteggy.Color;
                    this.gameBackgroundEggs.checkCurrentFrameByColor();

                    // trang thai return ve normal sau 200ms khi ban duoc attach vao ham Draw() cua class GameRubberband

                    if (this.gameMap.ListEggRows[0].YPosition <= 400
                        && this.gameFlyingEggs.ListFlyingEggs.Count - 1 >= 0) /* neu dong cuoi bi vuot qua muc 400 thi khong duoc tao them trung moi nua - neu khong se bi bug*/
                    {
                        this.gameFlyingEggs.ListFlyingEggs[this.gameFlyingEggs.ListFlyingEggs.Count - 1].EggStatus = EggStatus.Fly;
                        this.gameFlyingEggs.addFlyingEgg();
                    }
                }
            }
        }

        private void DisposeOldObject()
        {
            this.gameBackground.dispose();
            this.gameButtons.dispose();
            this.gameCursor.dispose();
            this.gameRubberband.dispose();
            this.gameHand.dispose();
            this.boWalls.dispose();
            this.boRope.dispose();
            this.boEggPile.dispose();
            this.boSidebar.Dispose();
            this.boSidebarMenuOverlap.Dispose();
            this.gameDinos.dispose();
            this.gameMap.dispose();
            this.gameBackgroundEggs.dispose();
            this.gameFlyingEggs.dispose();
            this.gameFallenEggs.dispose();
            this.gameFoot.dispose();
            this.gameScore.dispose();
        }

        /// <summary>
        /// This method handles the GotFocus event of the render form.
        /// If it runs fullscreen and loses focus, the device gets 
        /// not restored when switching back. So this has to be done
        /// manually.
        /// </summary>
        /// <param name="Sender">The sender (main form here)</param>
        /// <param name="e">EventParameter</param>
        protected void restore(object Sender, System.EventArgs e)
        {
            // Recreate graphics handler
            this.graphics = new DxInitGraphics(this.target);
            this.gameBackground.restore(this.graphics);
            this.gameCursor.restore(this.graphics);
            this.gameButtons.restore(this.graphics);
            this.gameRubberband.restore(this.graphics);
            this.gameHand.restore(this.graphics);
            this.boRope.restore(this.graphics);
            this.boWalls.restore(this.graphics);
            this.boSidebar.RestoreSurface();
            this.boSidebarMenuOverlap.RestoreSurface();
            this.boEggPile.restore(this.graphics);
            this.gameDinos.restore(this.graphics);
            this.gameMap.restore(this.graphics);
            this.gameBackgroundEggs.restore(this.graphics);
            this.gameFlyingEggs.restore(this.graphics);
            this.gameFallenEggs.restore(this.graphics);
            this.gameFoot.restore(this.graphics);
            this.gameScore.restore(this.graphics);
        }


        /// <summary>
        /// This method controls the main game loop. While the target
        /// control is created, it will render the game content to it.
        /// </summary>
        protected void gameLoop()
        {
            // To check the elapsed time
            dLoopDuration = 0.0;

            // Start Timer
            DxTimer.Start();


            // Check if target control is still available
            while (target.Created)
            {
                // If the target control has got no focus,
                // there's no need to render or calculate the
                // game parameters.
                if (!target.Focused)
                {
                    // Let the thread sleep a bit
                    System.Threading.Thread.Sleep(100);

                    // Allow the application to handle Windows messages
                    Application.DoEvents();

                    // Do the next loop
                    continue;
                }


#if DEBUG
                // Check elapsed milliseconds
                dLoopDuration += DxTimer.GetElapsedMilliseconds();

                if (dLoopDuration > 1000.0 / 60.0)
                {
#else
                dLoopDuration = DxTimer.GetElapsedMilliseconds();
#endif


                    // React on user input
                    this.processInput();

                    // If the gameState is set to exit, shut down game
                    if (gameState == GameStates.Run)
                    {

                        // Draw Background
                        this.gameBackground.Draw();


                        // check eggs in game
                        updateFlyEgg();
                        this.collisionTest();
                        this.removeEmptyRows();



                        //Draw Eggs
                        this.gameMap.Draw();

                        //Draw fallen eggs
                        this.gameFallenEggs.Draw();

                        //Draw Flying Eggs
                        this.gameFlyingEggs.Draw();


                        //Draw Rope
                        this.boRope.Draw();

                        //Draw walls
                        this.boWalls.Draw();

                        //Draw eggs on steggy and band
                        this.gameBackgroundEggs.Draw();

                        //Draw Dinos
                        this.gameDinos.Draw();

                        //Draw eggpiles
                        this.boEggPile.Draw();

                        //Draw sidebar
                        this.boSidebar.DrawBitmap(this.graphics.RenderSurface, (float)this.boSidebar.XPosition, (float)this.boSidebar.YPosition);
                        this.boSidebarMenuOverlap.DrawBitmap(this.graphics.RenderSurface, (float)this.boSidebarMenuOverlap.XPosition, (float)this.boSidebarMenuOverlap.YPosition);

                        //Check buttons state before draw
                        this.buttonState();
                        this.gameButtons.Draw();

                        //Draw rubberband
                        this.DrawRubberband();

                        //Draw hand
                        this.DrawHand();

                        //Draw Foot
                        this.gameFoot.Draw();

                        //Draw Score
                        this.gameScore.Draw();

                        //Draw cursor
                        this.gameCursor.Draw();
                    }
                    // If the gameState is set to exit, shut down game
                    if (gameState == GameStates.Exit)
                    {
                        return;
                    }

#if DEBUG
                    dLoopDuration = 0.0;
                }
#endif
                // Draw everything to the screen
                graphics.Flip();

                // Allow application to handle Windows messages
                Application.DoEvents();
            }
        }



        /// <summary>
        /// Retrieves the input state from the input handler
        /// and reacts depending on the user input.
        /// </summary>
        protected void processInput()
        {
            // This will save the current keyboard state
            KeyboardState state;

            // Get the keyboard state
            state = this.input.GetKeyboardState();

            // process the keyboard state
            if (state != null)
            {
                // on escape -> exit
                if (state[Key.Escape])
                {
                    gameState = GameStates.Exit;
                }
                if (state[Key.Left])
                {

                }

                if (state[Key.Right])
                {

                }
                if (state[Key.Space])
                {

                }
            }
            if (this.bMouseHandling)
            {
                MouseState mState = mInput.GetMouseState();
                mInput.buttonPressed = mState.GetMouseButtons();
            }
            cursorX = mInput.X;
            cursorY = mInput.Y;
            this.gameCursor.updateCursorPos(cursorX, cursorY);
        }
        protected void target_MouseLeave(object sender, EventArgs e)
        {
            if (this.cursorX >= 640 || this.cursorY >= 480)
            {
                this.bMouseHandling = false;
            }
        }
        protected void target_MouseEnter(object sender, EventArgs e)
        {
            this.bMouseHandling = true;
        }
        private void buttonState()
        {
            this.gameButtons.NewGameButton.BtState = ButtonState.Normal;
            this.gameButtons.OptionButton.BtState = ButtonState.Normal;
            this.gameButtons.ExitButton.BtState = ButtonState.Normal;
            // check newgame hover?
            if (this.cursorX >= this.gameButtons.NewGameButton.XPosition &&
                this.cursorX <= this.gameButtons.NewGameButton.XPosition + this.gameButtons.NewGameButton.Width)
            {
                if (this.cursorY >= this.gameButtons.NewGameButton.YPosition &&
                    this.cursorY <= this.gameButtons.NewGameButton.YPosition + this.gameButtons.NewGameButton.Height)
                {
                    this.gameButtons.NewGameButton.BtState = ButtonState.Hovered;
                }
            }
            // check option hover?
            if (this.cursorX >= this.gameButtons.OptionButton.XPosition &&
                this.cursorX <= this.gameButtons.OptionButton.XPosition + this.gameButtons.OptionButton.Width)
            {
                if (this.cursorY >= this.gameButtons.OptionButton.YPosition &&
                    this.cursorY <= this.gameButtons.OptionButton.YPosition + this.gameButtons.OptionButton.Height)
                {
                    this.gameButtons.OptionButton.BtState = ButtonState.Hovered;
                }
            }
            // check exit hover?
            if (this.cursorX >= this.gameButtons.ExitButton.XPosition &&
                this.cursorX <= this.gameButtons.ExitButton.XPosition + this.gameButtons.ExitButton.Width)
            {
                if (this.cursorY >= this.gameButtons.ExitButton.YPosition &&
                    this.cursorY <= this.gameButtons.ExitButton.YPosition + this.gameButtons.ExitButton.Height)
                {
                    this.gameButtons.ExitButton.BtState = ButtonState.Hovered;
                }
            }
        }
        private void DrawRubberband()
        {
            this.gameRubberband.calcucalteGocAlpha(this.cursorX, this.cursorY);
            this.gameRubberband.calculateHandleX(this.cursorX, this.cursorY);
            this.gameRubberband.calculateHeSoGoc_left();
            this.gameRubberband.calculateHeSoGoc_right();
            this.gameRubberband.Draw();
        }
        private void DrawHand()
        {
            this.gameHand.setHandleX(GameRubberband.HandleX);
            this.gameHand.setHandleY(GameRubberband.HandleY);
            this.gameHand.Draw();
        }
        private void collisionTest()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                for (int i = 0; i < this.gameFlyingEggs.ListFlyingEggs.Count; i++)
                {
                    if (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.YPosition <= 0)
                    {
                        this.gameMap.addNewRowOnTop();
                        this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.YPosition = this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.LastYPosition;
                        this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition = this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.LastXPosition;
                    }
                    for (int j = 0; j < this.gameMap.ListEggRows.Count; j++)
                    {
                        for (int k = 0; k < this.gameMap.ListEggRows[j].ListEggs.Count; k++)
                        {
                            //Su dung Minkowski sum
                            //float w = 0.5 * (A.width() + B.width());
                            //float h = 0.5 * (A.height() + B.height());
                            //float dx = A.centerX() - B.centerX();
                            //float dy = A.centerY() - B.centerY();

                            //if (abs(dx) <= w && abs(dy) <= h)
                            //{
                            //    /* collision! */
                            //    float wy = w * dy;
                            //    float hx = h * dx;

                            //    if (wy > hx)
                            //        if (wy > -hx)
                            //            /* collision at the top */
                            //        else
                            //            /* on the left */
                            //    else
                            //        if (wy > -hx)
                            //            /* on the right */
                            //        else
                            //            /* at the bottom */
                            //}
                            if (this.gameMap.ListEggRows[j].ListEggs[k].EggStatus != EggStatus.Normal)
                            {
                                continue;
                            }
                            double w = 0.5 * (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width + this.gameMap.ListEggRows[j].ListEggs[k].BoEgg.Width);
                            double h = 0.5 * (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Height + this.gameMap.ListEggRows[j].ListEggs[k].BoEgg.Height);

                            //dx, dy la khoang cach giua 2 tam Rec
                            double dx = this.gameFlyingEggs.ListFlyingEggs[i].centerX() - this.gameMap.ListEggRows[j].ListEggs[k].centerX();
                            double dy = this.gameFlyingEggs.ListFlyingEggs[i].centerY() - this.gameMap.ListEggRows[j].ListEggs[k].centerY();

                            if (Math.Abs(dx) <= w && Math.Abs(dy) <= h)
                            {
                                /* collision! */
                                //check if Rectangle B belongs to the last row of gameMap(first of the listEggRow)
                                /* if yes , add new Row at last(insert at 0 index)
                                /* if no, check position of rec A, check its origin to see which eggRow it belongs to, and which postion on the row the add new egg with same Color as recA*/
                                int l = j;
                                if (j == 0)
                                {
                                    this.gameMap.addNewRowAtBottom();
                                    //j++;
                                    l++;
                                }
                                double wy = w * dy;
                                double hx = h * dx;

                                if (wy > hx)
                                {
                                    if (wy > -hx)
                                    {
                                        /* collision at the top */
                                        Console.WriteLine("top");
                                        //Console.WriteLine(l + " " + k);
                                        if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Even)
                                        {
                                            // k va k + 1
                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() > this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k + 1 < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k + 1;
                                                }
                                                else if (k >= 0
                                                             && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() <= this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k >= 0
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k;
                                                }
                                                else if (k + 1 < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                             && (this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k + 1;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else
                                            {
                                                throw wrongPositionException;
                                            }
                                        }
                                        else if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Odd)
                                        {
                                            // k va k - 1
                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() <= this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k - 1 >= 0
                                                     && (this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k - 1;
                                                }
                                                else if (k < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                    && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() > this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k;
                                                }
                                                else if (k - 1 >= 0
                                                            && (this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k - 1;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        /* on the right */
                                        Console.WriteLine("right");
                                        if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Even)
                                        {

                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() < this.gameMap.ListEggRows[l].ListEggs[k].centerY()
                                                && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                            {
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                r = l + 1;
                                                c = k;
                                            }
                                            else
                                            {
                                                if (k - 1 >= 0
                                                    && (this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l;
                                                    c = k - 1;
                                                }
                                                else if (l - 1 >= 0
                                                            && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l - 1;
                                                    c = k;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                        }
                                        else if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Odd)
                                        {
                                            // k va k -1

                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() < this.gameMap.ListEggRows[l].ListEggs[k].centerY()
                                                && k - 1 >= 0
                                                && (this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                            {
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].checkColor();
                                                this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                r = l + 1;
                                                c = k - 1;
                                            }
                                            else
                                            {
                                                if (k - 1 >= 0
                                                    && (this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l;
                                                    c = k - 1;
                                                }
                                                else
                                                {
                                                    if (k - 1 >= 0
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k - 1;
                                                    }
                                                    else if (l - 1 >= 0
                                                                && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (wy > -hx)
                                    {
                                        /* on the left */
                                        Console.WriteLine("left");
                                        //Console.WriteLine("dong " + l + " cot " + k);
                                        if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Even)
                                        {

                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() < this.gameMap.ListEggRows[l].ListEggs[k].centerY())
                                            {

                                                if (l + 1 < this.gameMap.ListEggRows.Count)
                                                {
                                                    if (k + 1 < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                    && (this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {

                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l + 1;
                                                        c = k + 1;

                                                    }
                                                    else if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (l - 1 >= 0
                                                                && k + 1 < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                                      && (this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k + 1;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException; // khong the xay ra voi truong hop even
                                                    }
                                                }
                                                else
                                                {
                                                    if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                           && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (l - 1 >= 0
                                                             && k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                                && (this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k + 1;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException;
                                                    }
                                                }
                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() > this.gameMap.ListEggRows[l].ListEggs[k].centerY())
                                            {
                                                if (l - 1 >= 0)
                                                {
                                                    if (k + 1 < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                            && (this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k + 1;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException; // khong the xay ra voi even
                                                    }
                                                }
                                                else
                                                {
                                                    throw wrongPositionException; // truong hop co the cung khong xay ra
                                                }
                                            }
                                            else
                                            {
                                                throw wrongPositionException;
                                            }
                                        }
                                        else if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Odd)
                                        {
                                            // k va k -1
                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() < this.gameMap.ListEggRows[l].ListEggs[k].centerY()) // xet ban len
                                            {
                                                if (l + 1 < this.gameMap.ListEggRows.Count)
                                                {
                                                    if (k < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                    && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {

                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l + 1;
                                                        c = k;
                                                    }
                                                    else if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                                && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (l - 1 >= 0
                                                        && k < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException; // da gan nhu xet het truong hop, can kiem tra lai neu lai vao truong hop nay
                                                    }
                                                }
                                                else // xet truong hop neu ko ban len duoc thi ban o duoi
                                                {
                                                    if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                         && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (k < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                                 && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException;
                                                    }
                                                }


                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerY() >= this.gameMap.ListEggRows[l].ListEggs[k].centerY()) // xet ban xuong
                                            {
                                                if (l - 1 >= 0)
                                                {
                                                    if (k < this.gameMap.ListEggRows[l - 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l - 1;
                                                        c = k;
                                                    }
                                                    else if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                                && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (l + 1 < this.gameMap.ListEggRows.Count
                                                              && k < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                                && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l + 1;
                                                        c = k + 1;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException;
                                                    }
                                                }
                                                else // truong hop ban xuong ko duoc thi ban len , it, co the ko xay ra
                                                {
                                                    if (k + 1 < this.gameMap.ListEggRows[l].ListEggs.Count
                                                               && (this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].checkColor();
                                                        this.gameMap.ListEggRows[l].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                        r = l;
                                                        c = k + 1;
                                                    }
                                                    else if (l + 1 < this.gameMap.ListEggRows.Count
                                                        && k < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                    {
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                        this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                        r = l + 1;
                                                        c = k;
                                                    }
                                                    else
                                                    {
                                                        throw wrongPositionException;
                                                    }
                                                }

                                            }

                                        }
                                        else
                                        {
                                            throw wrongPositionException;
                                        }
                                    }

                                    else
                                    {
                                        /* at the bottom */
                                        Console.WriteLine("bottom");
                                        if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Even)
                                        {

                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() > this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k + 1 < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k + 1;
                                                }
                                                else if (k >= 0
                                                             && (this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l - 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() <= this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k >= 0
                                                        && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k;
                                                }
                                                else if (k + 1 < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                             && (this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k + 1].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k + 1;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else
                                            {
                                                throw wrongPositionException;
                                            }
                                        }
                                        else if (this.gameMap.ListEggRows[l].EggRowStatus == EggRowStatus.Odd)
                                        {
                                            if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() <= this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k - 1 >= 0
                                                     && (this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k - 1;
                                                }
                                                else if (k < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                    && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            else if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() > this.gameMap.ListEggRows[l].ListEggs[k].centerX())
                                            {
                                                if (k < this.gameMap.ListEggRows[l + 1].ListEggs.Count
                                                        && (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k;
                                                }
                                                else if (k - 1 >= 0
                                                            && (this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Idle || this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Explose))
                                                {
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].checkColor();
                                                    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                                    r = l + 1;
                                                    c = k - 1;
                                                }
                                                else
                                                {
                                                    throw wrongPositionException;
                                                }
                                            }
                                            //if (this.gameFlyingEggs.ListFlyingEggs[i].centerX() <= this.gameMap.ListEggRows[l].ListEggs[k].centerX()
                                            //    && k - 1 >= 0
                                            //    && this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Idle)
                                            //{
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].checkColor();
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                            //    r = l + 1;
                                            //    c = k - 1;
                                            //}

                                            //else if (k == this.gameMap.ListEggRows[l + 1].ListEggs.Count - 1
                                            //            && this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus == EggStatus.Idle)
                                            //{
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].checkColor();
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k - 1].EggStatus = EggStatus.Normal;
                                            //    r = l + 1;
                                            //    c = k - 1;
                                            //}
                                            //else if (this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus == EggStatus.Idle)
                                            //{
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k].Color = this.gameFlyingEggs.ListFlyingEggs[i].Color;
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k].checkColor();
                                            //    this.gameMap.ListEggRows[l + 1].ListEggs[k].EggStatus = EggStatus.Normal;
                                            //    r = l + 1;
                                            //    c = k;
                                            //}
                                            //else
                                            //{
                                            //    throw wrongPositionException;
                                            //}
                                        }
                                    }
                                }

                                soundEggLands.Play();

                                this.listSameColor = new List<GameEggs>();
                                this.listRemainEggs = new List<GameEggs>();
                                this.listFallEggs = new List<GameEggs>();
                                this.sameColorExplose(r, c);

                                //after got row, column use them to add the new egg into gameMap
                                //this.gameMap.updateGameMapAfterCollised(this.boFlyingEggs.ListFlyingEggs[i]);
                                //After add the collised flyingEgg into GameMap, remove it from the list of flyingEggs
                                this.gameFlyingEggs.ListFlyingEggs.Remove(this.gameFlyingEggs.ListFlyingEggs[i]);
                                // sau khi co duoc mang cung mau thi cho trung no
                                if (this.listSameColor.Count >= 3)
                                {
                                    this.gameFallenEggs.duplicateEggsFromListSameColor(this.listSameColor);
                                    // cho trung no
                                    for (int n = 0; n < this.listSameColor.Count; n++)
                                    {
                                        this.listSameColor[n].initBoExplosion();
                                        this.listSameColor[n].EggStatus = EggStatus.Explose;

                                    }
                                    //this.removeEmptyRows();
                                    // kiem tra cac trung con lai va cac trung se rot cung voi cac trung bi no
                                    // play audio trung be
                                    soundEggBroken.Play();
                                    soundExplosion.Play();
                                    this.addRemainEggsIntoList();
                                    //Console.WriteLine("remain " + this.listRemainEggs.Count);
                                    this.addFallEggsIntoList();
                                    //Console.WriteLine("fall " + this.listFallEggs.Count);
                                    this.gameFallenEggs.duplicateEggsFromListFallen(this.listFallEggs);

                                    //update score
                                    for (int s = 0; s < this.listSameColor.Count; s++)
                                    {
                                        GameScore.Score += 1;
                                    }
                                    for (int s = 0; s < this.listFallEggs.Count; s++)
                                    {
                                        GameScore.Score += 1;
                                    }
                                    gameScore.converseScoreToImageObjects();
                                }

                                return;
                            }

                        }
                    }

                }

            }
        }
        private void sameColorExplose(int r, int c)
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                EggColor tempColor = this.gameMap.ListEggRows[r].ListEggs[c].Color;

                //Current row
                if (this.gameMap.ListEggRows[r].ListEggs[c].EggStatus == EggStatus.Normal
                    && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c]) < 0)
                {
                    this.listSameColor.Add(this.gameMap.ListEggRows[r].ListEggs[c]);
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r].ListEggs[c].BoEgg.XPosition;
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r].ListEggs[c].BoEgg.YPosition;
                }
                if (c - 1 >= 0
                    && this.gameMap.ListEggRows[r].ListEggs[c - 1].EggStatus == EggStatus.Normal
                    && this.gameMap.ListEggRows[r].ListEggs[c - 1].Color == tempColor
                    && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c - 1]) < 0)
                {
                    this.listSameColor.Add(this.gameMap.ListEggRows[r].ListEggs[c - 1]);
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r].ListEggs[c - 1].BoEgg.XPosition;
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r].ListEggs[c - 1].BoEgg.YPosition;
                    this.sameColorExplose(r, c - 1);

                }
                if (c + 1 < this.gameMap.ListEggRows[r].ListEggs.Count
                         && this.gameMap.ListEggRows[r].ListEggs[c + 1].EggStatus == EggStatus.Normal
                         && this.gameMap.ListEggRows[r].ListEggs[c + 1].Color == tempColor
                         && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c + 1]) < 0)
                {
                    this.listSameColor.Add(this.gameMap.ListEggRows[r].ListEggs[c + 1]);
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r].ListEggs[c + 1].BoEgg.XPosition;
                    this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r].ListEggs[c + 1].BoEgg.YPosition;
                    this.sameColorExplose(r, c + 1);
                }

                //Upper row
                if (r + 1 < this.gameMap.ListEggRows.Count)
                {
                    if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Even)
                    {
                        if (this.gameMap.ListEggRows[r + 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c].BoEgg.YPosition;
                            this.sameColorExplose(r + 1, c);
                        }
                        if (this.gameMap.ListEggRows[r + 1].ListEggs[c + 1].EggStatus == EggStatus.Normal
                                 && this.gameMap.ListEggRows[r + 1].ListEggs[c + 1].Color == tempColor
                                 && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c + 1]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c + 1]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c + 1].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c + 1].BoEgg.YPosition;
                            this.sameColorExplose(r + 1, c + 1);
                        }
                    }
                    else if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Odd)
                    {
                        if (c - 1 >= 0
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c - 1].EggStatus == EggStatus.Normal
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c - 1].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c - 1]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c - 1]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c - 1].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c - 1].BoEgg.YPosition;
                            this.sameColorExplose(r + 1, c - 1);
                        }
                        if (c <= this.gameMap.ListEggRows[r].ListEggs.Count - 2
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c].EggStatus == EggStatus.Normal /* -2 vi c phai nho hon hoac bang so trung cua hang even phia tren */
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r + 1].ListEggs[c].BoEgg.YPosition;
                            this.sameColorExplose(r + 1, c);
                        }
                    }
                }
                //Lower row
                if (r - 1 >= 0)
                {
                    if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Even)
                    {
                        if (this.gameMap.ListEggRows[r - 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c].BoEgg.YPosition;
                            this.sameColorExplose(r - 1, c);
                        }
                        if (this.gameMap.ListEggRows[r - 1].ListEggs[c + 1].EggStatus == EggStatus.Normal
                                 && this.gameMap.ListEggRows[r - 1].ListEggs[c + 1].Color == tempColor
                                 && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c + 1]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c + 1]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c + 1].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c + 1].BoEgg.YPosition;
                            this.sameColorExplose(r - 1, c + 1);
                        }
                    }
                    else if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Odd)
                    {
                        if (c - 1 >= 0
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c - 1].EggStatus == EggStatus.Normal
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c - 1].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c - 1]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c - 1]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c - 1].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c - 1].BoEgg.YPosition;
                            this.sameColorExplose(r - 1, c - 1);
                        }
                        if (c <= this.gameMap.ListEggRows[r].ListEggs.Count - 2
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c].EggStatus == EggStatus.Normal /* -2 vi c phai nho hon hoac bang so trung cua hang even phia tren */
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c].Color == tempColor
                            && this.listSameColor.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c]) < 0)
                        {
                            this.listSameColor.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c]);
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.XPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c].BoEgg.XPosition;
                            this.listSameColor[this.listSameColor.Count - 1].BoEgg.YPosition = this.gameMap.ListEggRows[r - 1].ListEggs[c].BoEgg.YPosition;
                            this.sameColorExplose(r - 1, c);
                        }
                    }
                }
                //Console.WriteLine(this.listSameColor.Count);

            }
        }
        public void removeEmptyRows()
        {
            bool empty = true;
            while (empty)
            {
                if (this.gameMap.ListEggRows.Count == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        this.gameMap.addNewRowOnTop();
                        empty = false;
                    }
                    return;
                }
                for (int i = 0; i < this.gameMap.ListEggRows[0].ListEggs.Count; i++)
                {
                    if (this.gameMap.ListEggRows[0].ListEggs[i].EggStatus != EggStatus.Idle)
                    {
                        empty = false;
                        return;
                    }
                }
                if (empty)
                {
                    this.gameMap.ListEggRows.RemoveAt(0);
                }
            }
        }
        private void addRemainEggsIntoList(int r = - 1, int c = 0)
        {
            if (r == -1)
            {
                r = this.gameMap.ListEggRows.Count - 1;
            }
            if (this.gameMap.ListEggRows[r].ListEggs[c].EggStatus == EggStatus.Normal
                && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c]) < 0)
            {
                this.listRemainEggs.Add(this.gameMap.ListEggRows[r].ListEggs[c]);
                //current row
                if (c - 1 >= 0
                    && this.gameMap.ListEggRows[r].ListEggs[c - 1].EggStatus == EggStatus.Normal
                    && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c - 1]) < 0)
                {
                    //this.listRemainEggs.Add(this.gameMap.ListEggRows[r].ListEggs[c - 1]);
                    addRemainEggsIntoList(r, c - 1);
                }
                if (c + 1 < this.gameMap.ListEggRows[r].ListEggs.Count
                    && this.gameMap.ListEggRows[r].ListEggs[c + 1].EggStatus == EggStatus.Normal
                    && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r].ListEggs[c + 1]) < 0)
                {
                    //this.listRemainEggs.Add(this.gameMap.ListEggRows[r].ListEggs[c + 1]);
                    addRemainEggsIntoList(r, c + 1);
                }
                //upper row
                if (r + 1 < this.gameMap.ListEggRows.Count)
                {
                    if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Even)
                    {
                        if (this.gameMap.ListEggRows[r + 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c]);
                            addRemainEggsIntoList(r + 1, c);
                        }
                        if (this.gameMap.ListEggRows[r + 1].ListEggs[c + 1].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c + 1]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c]);
                            addRemainEggsIntoList(r + 1, c + 1);
                        }
                    }
                    else if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Odd)
                    {
                        if (c - 1 >= 0
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c - 1].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c - 1]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c - 1]);
                            addRemainEggsIntoList(r + 1, c - 1);
                        }
                        if (c < this.gameMap.ListEggRows[r + 1].ListEggs.Count
                            && this.gameMap.ListEggRows[r + 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r + 1].ListEggs[c]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r + 1].ListEggs[c]);
                            addRemainEggsIntoList(r + 1, c);
                        }
                    }
                }

                //lower row
                if (r - 1 >= 0)
                {
                    if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Even)
                    {
                        if (this.gameMap.ListEggRows[r - 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c]);
                            addRemainEggsIntoList(r - 1, c);
                        }
                        if (this.gameMap.ListEggRows[r - 1].ListEggs[c + 1].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c + 1]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c + 1]);
                            addRemainEggsIntoList(r - 1, c + 1);
                        }
                    }
                    else if (this.gameMap.ListEggRows[r].EggRowStatus == EggRowStatus.Odd)
                    {
                        if (c - 1 >= 0
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c - 1].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c - 1]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c - 1]);
                            addRemainEggsIntoList(r - 1, c - 1);
                        }
                        // bug down
                        if (c < this.gameMap.ListEggRows[r - 1].ListEggs.Count
                            && this.gameMap.ListEggRows[r - 1].ListEggs[c].EggStatus == EggStatus.Normal
                            && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r - 1].ListEggs[c]) < 0)
                        {
                            //this.listRemainEggs.Add(this.gameMap.ListEggRows[r - 1].ListEggs[c]);
                            addRemainEggsIntoList(r - 1, c);
                        }
                    }

                }
            }

            if (r == this.gameMap.ListEggRows.Count - 1)
            {
                for (int i = 0; i < this.gameMap.ListEggRows[r].ListEggs.Count; i++)
                {
                    if (this.gameMap.ListEggRows[this.gameMap.ListEggRows.Count - 1].ListEggs[i].EggStatus == EggStatus.Normal
                        && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[r].ListEggs[i]) < 0)
                    {
                        addRemainEggsIntoList(r, i);
                        //this.listRemainEggs.Add(this.gameMap.ListEggRows[this.gameMap.ListEggRows.Count - 1].ListEggs[i]);
                    }
                }
            }


        }
        private void addFallEggsIntoList()
        {
            for (int i = 0; i < this.gameMap.ListEggRows.Count; i++)
            {
                for (int j = 0; j < this.gameMap.ListEggRows[i].ListEggs.Count; j++)
                {
                    if (this.gameMap.ListEggRows[i].ListEggs[j].EggStatus == EggStatus.Normal
                        && this.listRemainEggs.IndexOf(this.gameMap.ListEggRows[i].ListEggs[j]) < 0)
                    {
                        this.listFallEggs.Add(this.gameMap.ListEggRows[i].ListEggs[j]);
                        this.gameMap.ListEggRows[i].ListEggs[j].EggStatus = EggStatus.Idle;
                    }
                }
            }
        }
        private void updateFlyEgg()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {

                for (int i = 0; i < this.gameFlyingEggs.ListFlyingEggs.Count; i++)
                {
                    if (this.gameFlyingEggs.ListFlyingEggs[i].EggStatus == EggStatus.Fly)
                    {
                        this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition += this.gameFlyingEggs.ListFlyingEggs[i].VX;
                        this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.YPosition += this.gameFlyingEggs.ListFlyingEggs[i].VY;
                        // kiem tra cho trung doi huong khi cham tuong
                        if (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition <= 250
                            || this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition + this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width >= 250 + this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width * 9)
                        {
                            if (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition <= 250) /*250 la toa do wall ben trai */
                            {
                                this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition = 250;
                            }
                            else if (this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition + this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width >= 250 + this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width * 9) /* 9 la tong so trung max tren 1 dong odd */
                            {
                                this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.XPosition = 250 + this.gameFlyingEggs.ListFlyingEggs[i].BoEgg.Width * 8;
                            }
                            this.gameFlyingEggs.ListFlyingEggs[i].VX *= -1;
                        }
                    }

                }
            }
        }
        protected void initGameObjects()
        {
            //this.boBackground = new DxImageObject(GameResources.back, BitmapType.SOLID, 0, this.graphics.DDDevice);
            this.gameBackground = new GameBackground(this.graphics);
            this.gameCursor = new GameCursor(this.graphics);
            this.gameButtons = new GameButtons(this.graphics);
            this.gameRubberband = new GameRubberband(this.graphics);
            this.gameHand = new GameHand(this.graphics);
            this.gameMap = new GameMap(this.graphics);
            this.boWalls = new GameWalls(this.graphics);
            this.boRope = new GameRope(this.graphics);
            this.boEggPile = new GameEggPiles(this.graphics);

            this.boSidebar = new DxImageObject(GameResources.sidebar, BitmapType.SOLID, 0, this.graphics.DDDevice);
            this.boSidebar.XPosition = 0;
            this.boSidebar.YPosition = 0;

            this.boSidebarMenuOverlap = new DxImageObject(GameResources.menuoverlap, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boSidebarMenuOverlap.XPosition = 150;
            this.boSidebarMenuOverlap.YPosition = 0;

            this.gameDinos = new GameDinosaurs(this.graphics);
            this.gameBackgroundEggs = new GameBackgroundEggs(this.graphics);
            this.gameFlyingEggs = new GameFlyingEggs(this.graphics);
            this.gameFallenEggs = new GameFallenEggs(this.graphics);
            this.gameFoot = new GameFoot(this.graphics);
            this.gameScore = new GameScore(this.graphics);
            this.wrongPositionException = new Exception("sai vi tri khi trung tiep xuc");

            soundEggLands = new DxSound("EggLands.wav");
            soundEggBroken = new DxSound("BreakEgg.wav");
            soundBlastit = new DxSound("cached_blastit.wav");
            soundClick = new DxSound("cached_click.wav");
            soundExplosion = new DxSound("cached_explosion.wav");
            soundWarning = new DxSound("cached_aooga.wav");
            //soundFootFalls = new DxSound("cached_footfall.wav");
            soundFootLands = new DxSound("cached_footlands.wav");
        }
    }
}
