using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameFoot : AbstractGameObjects
    {
        protected DxImageObject boFoot;
        protected double vFoot;
        protected bool playSound = false;
        public GameFoot(DxInitGraphics graphics)
            : base(graphics)
        {
            this.boFoot = new DxImageObject(GameResources.foot, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boFoot.XPosition = 260;
            this.boFoot.YPosition = 0;
            this.boFoot.SourceX = 0;
            this.boFoot.SourceY = GameResources.foot.Height;
            this.boFoot.Width = GameResources.foot.Width;
            this.boFoot.Height = 0;
            this.vFoot = 50.0;
        }
        public override void dispose()
        {
            this.boFoot.Dispose();
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.GameOver)
            {
                //if (this.boFoot.Height == 0)
                //{
                //    GameLogic.soundFootFalls.Play();
                //}
             
                if (this.boFoot.Height < GameResources.foot.Height)
                {
                    this.boFoot.Height += (int)this.vFoot;
                    this.boFoot.SourceY = GameResources.foot.Height - this.boFoot.Height;
                    if (this.boFoot.Height >= GameResources.foot.Height
                            || this.boFoot.SourceY < 0)
                    {
                        this.boFoot.Height = GameResources.foot.Height;
                        this.boFoot.SourceY = 0;
                        playSound = true;
                    }
                }
                if (this.playSound == true)
                {
                    GameLogic.soundFootLands.Play();
                    playSound = false;  
                }
                this.boFoot.DrawBitmap(this.graphics.RenderSurface, (float)this.boFoot.XPosition, (float)this.boFoot.YPosition, (int)this.boFoot.SourceX, (int)this.boFoot.SourceY, (int)this.boFoot.Width, (int)this.boFoot.Height);
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boFoot.RestoreSurface();
        }
    }
}
