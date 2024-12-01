using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameHand : AbstractGameObjects
    {
        protected DxImageObject boHand;
        protected double midPos = 405;
        protected double handleY, handleX;
        protected int currentFrame;
        public GameHand(DxInitGraphics graphics)
            : base(graphics)
        {
            this.boHand = new DxImageObject(GameResources.handsheet, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boHand.Width = 81;
            this.boHand.SourceX = 0;
            this.boHand.SourceY = 0;

        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                check();
                this.boHand.DrawBitmap(this.graphics.RenderSurface, (float)this.boHand.XPosition, (float)this.boHand.YPosition, (int)this.boHand.SourceX, (int)this.boHand.SourceY, this.boHand.Width, this.boHand.Height);
            }
        }
        public override void dispose()
        {
            this.boHand.Dispose();
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boHand.RestoreSurface();
        }
        public void setHandleX(double handleX)
        {
            this.handleX = handleX;
        }
        public void setHandleY(double handleY)
        {
            this.handleY = handleY;
        }
        public void check()
        {
            if (GameRubberband.BandStatus == BandStatus.Finished)
            {
                this.boHand.YPosition = 450;
                this.boHand.Height = (int)(480.0 - this.boHand.YPosition);
            }
            else if (GameRubberband.BandStatus == BandStatus.Ready)
            {
                this.boHand.YPosition = handleY - 25.5;
                this.boHand.Height = (int)(480.0 - this.boHand.YPosition);

                //handleX <= 458 va >= 378
                if (handleX >= 378 && handleX < 378 + 7.5)
                {
                    this.currentFrame = 0;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 60, (float)handleY - 35, 0 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 && handleX < 378 + 7.5 * 2)
                {
                    this.currentFrame = 1;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 60, (float)handleY - 35, 1 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 2 && handleX < 378 + 7.5 * 3)
                {
                    this.currentFrame = 2;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 55, (float)handleY - 35, 2 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 3 && handleX < 378 + 7.5 * 4)
                {
                    this.currentFrame = 3;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 50, (float)handleY - 35, 3 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 4 && handleX < 378 + 7.5 * 5)
                {
                    this.currentFrame = 4;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 45, (float)handleY - 35, 4 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 5 && handleX < 378 + 7.5 * 6)
                {
                    this.currentFrame = 5;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 40, (float)handleY - 35, 5 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 6 && handleX < 378 + 7.5 * 7)
                {
                    this.currentFrame = 6;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 35, (float)handleY - 35, 6 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 7 && handleX < 378 + 7.5 * 8)
                {
                    this.currentFrame = 7;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 35, (float)handleY - 35, 7 * frameX, 0, 81, 118);
                }
                else if (handleX >= 378 + 7.5 * 8)
                {
                    this.currentFrame = 8;

                    //this.DrawBitmap(this.graphics.RenderSurface, (float)handleX - 35, (float)handleY - 35, 8 * frameX, 0, 81, 118);
                }
                //Console.WriteLine(this.currentFrame);
                this.boHand.XPosition = handleX - 39.5;
                this.boHand.SourceX = currentFrame * boHand.Width;
            }

        }
    }
}
