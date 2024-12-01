using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public enum ScreenState
    {
        IntroScreen,
        JungleScreen,
        GameOver,
    }
    class GameBackground
    {
        protected DxInitGraphics graphics;
        protected DxImageObject boIntroScreen;
        protected DxImageObject boJungleScreen;
        //protected DxImageObject boSidebar;
        //protected DxImageObject boSidebarMenuOverlap;
        //protected GameWalls boWalls;
        //protected GameRope boRope;
        //protected GameEggPiles boEggPile;
        protected GameSlingshot boSlingshot;
        //protected GameDinosaurs boDinos;

        public GameBackground(DxInitGraphics graphics)
        {
            this.graphics = graphics;

            this.boIntroScreen = new DxImageObject(GameResources.introscreen, BitmapType.SOLID, 0, this.graphics.DDDevice);
            this.boIntroScreen.XPosition = 150;
            this.boIntroScreen.YPosition = 0;

            this.boJungleScreen = new DxImageObject(GameResources.jungleback, BitmapType.SOLID, 0, this.graphics.DDDevice);
            this.boJungleScreen.XPosition = 150;
            this.boJungleScreen.YPosition = 0;

            //this.boSidebar = new DxImageObject(GameResources.sidebar, BitmapType.SOLID, 0, this.graphics.DDDevice);
            //this.boSidebar.XPosition = 0;
            //this.boSidebar.YPosition = 0;

            //this.boSidebarMenuOverlap = new DxImageObject(GameResources.menuoverlap, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            //this.boSidebarMenuOverlap.XPosition = 150;
            //this.boSidebarMenuOverlap.YPosition = 0;

            //this.boWalls = new GameWalls(this.graphics);
            //this.boRope = new GameRope(this.graphics);

            //this.boEggPile = new GameEggPiles(this.graphics);

            this.boSlingshot = new GameSlingshot(this.graphics);
            //this.boDinos = new GameDinosaurs(this.graphics);

        }
        public void Draw()
        {
            //this.boSidebar.DrawBitmap(this.graphics.RenderSurface, (float)this.boSidebar.XPosition, (float)this.boSidebar.YPosition);
            if (GameLogic.screenState == ScreenState.IntroScreen)
            {
                this.boIntroScreen.DrawBitmap(this.graphics.RenderSurface, (float)this.boIntroScreen.XPosition, (float)this.boIntroScreen.YPosition);
            }
            else if (GameLogic.screenState == ScreenState.JungleScreen
                || GameLogic.screenState == ScreenState.GameOver)
            {
                this.boJungleScreen.DrawBitmap(this.graphics.RenderSurface, (float)this.boJungleScreen.XPosition, (float)this.boJungleScreen.YPosition);
                //this.boRope.Draw();
                //this.boWalls.Draw();
                //this.boDinos.Draw();
                //this.boEggPile.Draw();
                //Draw slingshot
                this.boSlingshot.Draw();
            }
            //this.boSidebarMenuOverlap.DrawBitmap(this.graphics.RenderSurface, (float)this.boSidebarMenuOverlap.XPosition, (float)this.boSidebarMenuOverlap.YPosition);

        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            //this.boSidebar.RestoreSurface();
            if (GameLogic.screenState == ScreenState.IntroScreen)
            {
                this.boIntroScreen.RestoreSurface();
            }
            else if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                this.boJungleScreen.RestoreSurface();
                //this.boRope.restore(this.graphics);
                //this.boWalls.restore(this.graphics);
                //this.boDinos.restore(this.graphics);
                //this.boEggPile.restore(this.graphics);
                this.boSlingshot.restore(this.graphics);
            }
            //this.boSidebarMenuOverlap.RestoreSurface();

        }
        public void dispose()
        {
            this.boIntroScreen.Dispose();
            this.boJungleScreen.Dispose();
            //this.boSidebar.Dispose();
            //this.boSidebarMenuOverlap.Dispose();
            //this.boWalls.dispose();
            //this.boEggPile.dispose();
            //this.boRope.dispose();
            //this.boDinos.dispose();
            this.boSlingshot.dispose();
        }
    }
}
