using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameWalls
    {
        protected DxImageObject boLeftWall;
        protected DxImageObject boRigtWall;
        protected DxInitGraphics graphics;
        public GameWalls(DxInitGraphics graphics)
        {
            this.graphics = graphics;

            this.boLeftWall = new DxImageObject(GameResources.leftwall, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boLeftWall.XPosition = 190;
            this.boLeftWall.YPosition = 0;

            this.boRigtWall = new DxImageObject(GameResources.rightwall, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boRigtWall.XPosition = 540;
            this.boRigtWall.YPosition = 0;
        }
        public void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen
                || GameLogic.screenState == ScreenState.GameOver)
            {
                this.boLeftWall.DrawBitmap(this.graphics.RenderSurface, (float)this.boLeftWall.XPosition, (float)this.boLeftWall.YPosition);
                this.boRigtWall.DrawBitmap(this.graphics.RenderSurface, (float)this.boRigtWall.XPosition, (float)this.boRigtWall.YPosition);
            }
        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boLeftWall.RestoreSurface();
            this.boRigtWall.RestoreSurface();
        }
        public void dispose()
        {
            this.boLeftWall.Dispose();
            this.boRigtWall.Dispose();
        }
    }
}
