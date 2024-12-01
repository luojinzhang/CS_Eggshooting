using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameSlingshot : AbstractGameObjects
    {
        DxImageObject boSlingshot;

        public GameSlingshot(DxInitGraphics graphics)
            : base(graphics)
        {
            boSlingshot = new DxImageObject(GameResources.Slingshot, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boSlingshot.XPosition = 350;
            this.boSlingshot.YPosition = 380;
            this.boSlingshot.SourceX = 0;
            this.boSlingshot.SourceY = 0;
            this.boSlingshot.Width = GameResources.Slingshot.Width;
            this.boSlingshot.Height = (int)(480.0 - this.boSlingshot.YPosition);
        }
        public override void dispose()
        {
            this.boSlingshot.Dispose();
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                this.boSlingshot.DrawBitmap(this.graphics.RenderSurface, (float)this.boSlingshot.XPosition, (float)this.boSlingshot.YPosition,
                                            (int)this.boSlingshot.SourceX, (int)this.boSlingshot.SourceY,
                                                                    (int)this.boSlingshot.Width, this.boSlingshot.Height);
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boSlingshot.RestoreSurface();
        }
    }
}
