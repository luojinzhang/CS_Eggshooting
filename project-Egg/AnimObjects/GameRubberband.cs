using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public enum BandStatus
    {
        Ready,
        Finished,
    }
    class GameRubberband : AbstractGameObjects
    {
        protected DxImageObject boRubberband;
        protected DxTimer t;
        protected double bandLength;
        protected double bandMid;
        protected static double handleY, handleX;
        protected double gocAlpha;
        protected double heSoGocLeft;
        protected double hesoGocRight;
        protected double bandX,bandY;
        protected static BandStatus bandStatus;

        public static double HandleX
        {
            get { return GameRubberband.handleX; }
        }
        public static double HandleY
        {
            get { return GameRubberband.handleY; }
        }
        public static BandStatus BandStatus
        {
          get { return GameRubberband.bandStatus; }
          set { GameRubberband.bandStatus = value; }
        }
        public GameRubberband (DxInitGraphics graphics)
            :base (graphics)
        {
            this.boRubberband = new DxImageObject(GameResources.RubberBand, BitmapType.SOLID, 0, this.graphics.DDDevice);
            t = new DxTimer();
            this.bandLength = 106.0;
            this.bandX = 355.0;
            this.bandY = 403.0;
            handleY = 460;
            this.bandMid = this.bandX + this.bandLength/2;
            bandStatus = project_Egg.BandStatus.Ready;
        }
        public override void dispose()
        {
            this.boRubberband.Dispose();
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                int y = 403;
                int x = 355;
                if (GameRubberband.bandStatus == BandStatus.Ready)
                {
                    double tempY = y;

                    for (; x < (int)handleX; x++)
                    {
                        this.boRubberband.DrawBitmap(this.graphics.RenderSurface, x, y);
                        tempY += heSoGocLeft;
                        y = (int)(tempY);
                        // y = a * x
                    }

                    //for (int k = x + 10; x < k; x++)
                    //{
                    //    this.boRubberband.DrawBitmap(this.graphics.RenderSurface, x, y);
                        
                    //}

                    for (; x < bandX + bandLength; x++)
                    {
                        this.boRubberband.DrawBitmap(this.graphics.RenderSurface, x, y);
                        tempY -= hesoGocRight;
                        y = (int)(tempY);
                    }
                }
                else if (GameRubberband.bandStatus == BandStatus.Finished)
                {
                    t.markTime();
                    if (t.msElapsed() > 200)
                    {
                        GameRubberband.BandStatus = project_Egg.BandStatus.Ready;
                        GameBackgroundEggs.EggOnRubberband.EggStatus = EggStatus.Normal;
                        GameBackgroundEggs.EggOnSteggy.rollColor();
                        t.resetTime();
                        return;
                    }
                    for (; x < bandX + bandLength; x++)
                    {
                        this.boRubberband.DrawBitmap(this.graphics.RenderSurface, x, y);
                    }

                }
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boRubberband.RestoreSurface();
        }
        public double calcucalteGocAlpha(double cursorX, double cursorY)
        {
            if (cursorX > 150)
            {
                if (Math.Abs(cursorX - this.bandMid) == 0)
                {
                    this.gocAlpha = Math.Atan(Math.Abs(cursorY - this.bandY));
                }
                else
                {
                    this.gocAlpha = Math.Atan((double)Math.Abs(cursorY - this.bandY) / Math.Abs(cursorX - bandMid));
                }
            }
            return this.gocAlpha;
        }
        public double calculateHandleX(double cursorX, double cursorY)
        {
            if (cursorX > this.bandMid)
            {
                handleX = this.bandMid - Math.Cos(this.gocAlpha) * this.bandLength / 2;

            }
            else if (cursorX < this.bandMid)
            {
                handleX = this.bandMid + Math.Cos(this.gocAlpha) * this.bandLength  / 2;
            }

            if (handleX < this.bandMid - 30)
            {
                handleX = this.bandMid - 30;
            }
            else if (handleX > this.bandMid + 30)
            {
                handleX = this.bandMid + 30;
            }
            return handleX;
        }
        public double calculateHeSoGoc_left()
        {
            // Calculate rubberband leftSide
            double rubberBand_leftSide = Math.Abs(handleX - this.bandX);
            // Calculate hesogoc left
            heSoGocLeft = (handleY - bandY) / rubberBand_leftSide;

            return this.heSoGocLeft;
        }
        public double calculateHeSoGoc_right()
        {
            //Calculate rubberband rightside
            double rubberBand_rightSide = Math.Abs(bandX + bandLength - handleX);
            hesoGocRight = (handleY - bandY) / rubberBand_rightSide;
            return this.hesoGocRight;
        }
    }
}
