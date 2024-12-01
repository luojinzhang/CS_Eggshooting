using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameEggPiles
    {
        protected DxInitGraphics graphics;
        protected DxImageObject boLeftEggPile;
        protected DxImageObject boRightEggPile;
        public GameEggPiles(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boLeftEggPile = new DxImageObject(GameResources.eggpilesheet, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boLeftEggPile.XPosition = 140;
            this.boLeftEggPile.YPosition = 385;
            this.boLeftEggPile.SourceX = 217;
            this.boLeftEggPile.SourceY = 0;
            this.boLeftEggPile.Width = 217;
            this.boLeftEggPile.Height = 95;

            this.boRightEggPile = new DxImageObject(GameResources.eggpilesheet, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boRightEggPile.XPosition = 480;
            this.boRightEggPile.YPosition = 385;
            this.boRightEggPile.SourceX = 0;
            this.boRightEggPile.SourceY = 0;
            this.boRightEggPile.Width = (int)(640.0 - this.boRightEggPile.XPosition);
            this.boRightEggPile.Height = 95;
        }
        public void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {

                this.boLeftEggPile.DrawBitmap(this.graphics.RenderSurface, (float)this.boLeftEggPile.XPosition, (float)this.boLeftEggPile.YPosition,
                                                            (int)this.boLeftEggPile.SourceX, (int)this.boLeftEggPile.SourceY,
                                                            this.boLeftEggPile.Width, this.boLeftEggPile.Height);
                this.boRightEggPile.DrawBitmap(this.graphics.RenderSurface, (float)this.boRightEggPile.XPosition, (float)this.boRightEggPile.YPosition,
                                                            (int)this.boRightEggPile.SourceX, (int)this.boRightEggPile.SourceY,
                                                            this.boRightEggPile.Width, this.boRightEggPile.Height);
            }
        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boLeftEggPile.RestoreSurface();
            this.boRightEggPile.RestoreSurface();
        }
        public void dispose()
        {
            this.boLeftEggPile.Dispose();
            this.boRightEggPile.Dispose();
        }
    }
}
