using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameRope
    {
        protected DxInitGraphics graphics;
        protected DxImageObject boRope;
        protected static double limit;

        public static double Limit
        {
            get { return GameRope.limit; }
        }

        public GameRope(DxInitGraphics graphics)
        {
            this.graphics = graphics;

            this.boRope = new DxImageObject(GameResources.rope, BitmapType.SOLID, 0, this.graphics.DDDevice);
            this.boRope.XPosition = 200;
            this.boRope.YPosition = 320;
            GameRope.limit = this.boRope.YPosition;
        }
        public void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                while (this.boRope.XPosition <= 540)
                {
                    this.boRope.DrawBitmap(this.graphics.RenderSurface, (float)this.boRope.XPosition, (float)this.boRope.YPosition);
                    this.boRope.XPosition += 8;
                }
                this.boRope.XPosition = 200;
            }
        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boRope.RestoreSurface();
        }
        public void dispose()
        {
            this.boRope.Dispose();
        }
    }
}
