using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{

    class GameButtons
    {
        //Variables
        protected DxInitGraphics graphics;
        protected DxImageObject newGameButton;
        protected DxImageObject optionButton;
        protected DxImageObject exitButton;

        //Properties
        public DxImageObject NewGameButton
        {
            get { return newGameButton; }
        }
        public DxImageObject OptionButton
        {
            get { return optionButton; }
        }
        public DxImageObject ExitButton
        {
            get { return exitButton; }
        }

        public GameButtons(DxInitGraphics graphics)
        {
            this.graphics = graphics;

            newGameButton = new DxImageObject(GameResources.SidebarButtons, BitmapType.SOLID, 0, this.graphics.DDDevice);
            newGameButton.XPosition = 23;
            newGameButton.YPosition = 148;
            newGameButton.Width = 103;
            newGameButton.Height = 53;
            newGameButton.BtState = ButtonState.Normal;

            optionButton = new DxImageObject(GameResources.SidebarButtons, BitmapType.SOLID, 0, this.graphics.DDDevice);
            optionButton.XPosition = 29;
            optionButton.YPosition = 244;
            optionButton.Width = 91;
            optionButton.Height = 33;
            optionButton.BtState = ButtonState.Normal;

            exitButton = new DxImageObject(GameResources.SidebarButtons, BitmapType.SOLID, 0, this.graphics.DDDevice);
            exitButton.XPosition = 25;
            exitButton.YPosition = 415;
            exitButton.Width = 97;
            exitButton.Height = 29;
            exitButton.BtState = ButtonState.Normal;
        }
        public void Draw()
        {
            check();

            this.newGameButton.DrawBitmap(this.graphics.RenderSurface,
                                            (float)this.newGameButton.XPosition,
                                            (float)this.newGameButton.YPosition,
                                            (int)this.newGameButton.SourceX,
                                            (int)this.newGameButton.SourceY,
                                            this.newGameButton.Width,
                                            this.newGameButton.Height);

            this.optionButton.DrawBitmap(this.graphics.RenderSurface,
                                            (float)this.optionButton.XPosition,
                                            (float)this.optionButton.YPosition,
                                            (int)this.optionButton.SourceX,
                                            (int)this.optionButton.SourceY,
                                            this.optionButton.Width,
                                            this.optionButton.Height);

            this.exitButton.DrawBitmap(this.graphics.RenderSurface,
                                            (float)this.exitButton.XPosition,
                                            (float)this.exitButton.YPosition,
                                            (int)this.exitButton.SourceX,
                                            (int)this.exitButton.SourceY,
                                            this.exitButton.Width,
                                            this.exitButton.Height);
        }
        public void check()
        {
            if (newGameButton.BtState == ButtonState.Normal)
            {
                newGameButton.SourceX = 0;
                newGameButton.SourceY = 0;
            }
            else if (newGameButton.BtState == ButtonState.Hovered)
            {
                newGameButton.SourceX = 103;
                newGameButton.SourceY = 0;
            }
            if (optionButton.BtState == ButtonState.Normal)
            {
                optionButton.SourceX = 206;
                optionButton.SourceY = 0;
            }
            else if (optionButton.BtState == ButtonState.Hovered)
            {
                optionButton.SourceX = 206;
                optionButton.SourceY = 33;
            }
            if (exitButton.BtState == ButtonState.Normal)
            {
                exitButton.SourceX = 0;
                exitButton.SourceY = 53;
            }
            else if (exitButton.BtState == ButtonState.Hovered)
            {
                exitButton.SourceX = 97;
                exitButton.SourceY = 53;
            }
        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.newGameButton.RestoreSurface();
            this.optionButton.RestoreSurface();
            this.exitButton.RestoreSurface();
        }
        public void dispose()
        {
            this.newGameButton.Dispose();
            this.optionButton.Dispose();
            this.exitButton.Dispose();
        }
    }
}
