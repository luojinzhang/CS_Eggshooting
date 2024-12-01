using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public enum DinosaurStatus
    {
        Normal,
        Panic,
    }
    class GameDinosaurs : AbstractGameObjects
    {
        protected static DinosaurStatus dinoStatus;
        protected DxImageObject boSteggy;
        protected DxImageObject boTank;
        protected DxImageObject boAlert;
        protected DxImageObject boTankSweet;
        protected DxTimer t;
        public static DinosaurStatus DinoStatus
        {
            get { return GameDinosaurs.dinoStatus; }
            set { GameDinosaurs.dinoStatus = value; }
        }
        public GameDinosaurs(DxInitGraphics graphics)
            : base(graphics)
        {
            GameDinosaurs.dinoStatus = DinosaurStatus.Normal;
            initDinosaurs();
            initPanicSymbol();
            this.t = new DxTimer();
        }

        private void initPanicSymbol()
        {
            this.boAlert = new DxImageObject(GameResources._alert, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boAlert.XPosition = this.boSteggy.XPosition + GameResources.SteggyTerror.Width;
            this.boAlert.YPosition = this.boSteggy.YPosition - GameResources._alert.Height;

            this.boTankSweet = new DxImageObject(GameResources._TankSweat, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boTankSweet.XPosition = 497;
            this.boTankSweet.YPosition = this.boTank.YPosition - GameResources._TankSweat.Height;
        }

        private void initDinosaurs()
        {
            this.boSteggy = new DxImageObject(GameResources.steggy, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boSteggy.XPosition = 237;
            this.boSteggy.YPosition = 380;
            this.boSteggy.Width = GameResources.steggy.Width;
            this.boSteggy.Height = (int)(480.0 - this.boSteggy.YPosition);

            this.boTank = new DxImageObject(GameResources.tanksheet, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boTank.XPosition = 477;
            this.boTank.YPosition = 358;
            this.boTank.Width = GameResources.tanksheet.Width / 3;
            this.boTank.Height = (int)(480.0 - this.boTank.YPosition);
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                if (GameDinosaurs.dinoStatus == DinosaurStatus.Normal)
                {
                    if (this.boSteggy.BitmapOfObject != GameResources.steggy ||
                        this.boTank.BitmapOfObject != GameResources.tanksheet)
                    {
                        this.boSteggy.Dispose();
                        this.boTank.Dispose();
                        this.initDinosaurs();
                    }
                }
                else if (GameDinosaurs.dinoStatus == DinosaurStatus.Panic)
                {
                    if (this.boSteggy.BitmapOfObject != GameResources.SteggyTerror ||
                        this.boTank.BitmapOfObject != GameResources.TankTerror)
                    {
                        this.boSteggy.Dispose();
                        this.boTank.Dispose();
                        initPanicDinosaurs();
                    }
                }

                this.boSteggy.DrawBitmap(this.graphics.RenderSurface,
                                            (float)this.boSteggy.XPosition, (float)this.boSteggy.YPosition, 0, 0,
                                            (int)this.boSteggy.Width, (int)this.boSteggy.Height);

                this.boTank.DrawBitmap(this.graphics.RenderSurface, (float)this.boTank.XPosition,
                                        (float)this.boTank.YPosition, 0, 0,
                                        (int)this.boTank.Width, (int)this.boTank.Height);

                if (GameDinosaurs.dinoStatus == DinosaurStatus.Panic)
                {
                    t.markTime();
                    if (t.msElapsed() < 200)
                    {
                        this.boAlert.DrawBitmap(this.graphics.RenderSurface, (float)this.boAlert.XPosition, (float)this.boAlert.YPosition);
                        this.boTankSweet.DrawBitmap(this.graphics.RenderSurface, (float)this.boTankSweet.XPosition, (float)this.boTankSweet.YPosition);
                    }
                    else if (t.msElapsed() >= 400)
                    {
                        t.resetTime();
                    }

                }
            }
        }

        private void initPanicDinosaurs()
        {
            this.boSteggy = new DxImageObject(GameResources.SteggyTerror, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boSteggy.XPosition = 237;
            this.boSteggy.YPosition = 380;
            this.boSteggy.Width = GameResources.steggy.Width;
            this.boSteggy.Height = (int)(480.0 - this.boSteggy.YPosition);

            this.boTank = new DxImageObject(GameResources.TankTerror, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boTank.XPosition = 477;
            this.boTank.YPosition = 358;
            this.boTank.Width = 108;
            this.boTank.Height = (int)(480.0 - this.boTank.YPosition);
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boTank.RestoreSurface();
            this.boSteggy.RestoreSurface();
            if (GameDinosaurs.dinoStatus == DinosaurStatus.Panic)
            {
                this.boAlert.RestoreSurface();
                this.boTankSweet.RestoreSurface();
            }
        }
        public override void dispose()
        {
            this.boSteggy.Dispose();
            this.boTank.Dispose();
            this.boAlert.Dispose();
            this.boTankSweet.Dispose();
        }
    }
}
