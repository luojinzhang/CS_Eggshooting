using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameCursor
    {
        protected DxInitGraphics graphics;
        protected DxImageObject boCursor;
        public GameCursor(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boCursor = new DxImageObject(GameResources.cusor, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
        }
        public void Draw()
        {
            this.boCursor.DrawBitmap(this.graphics.RenderSurface, (float)this.boCursor.XPosition, (float)this.boCursor.YPosition);
        }
        public void updateCursorPos(double cursorX, double cursorY)
        {
            this.boCursor.XPosition = cursorX;
            this.boCursor.YPosition = cursorY;
        }
        public void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boCursor.RestoreSurface();
        }
        public void dispose()
        {
            this.boCursor.Dispose();
        }
    }
}
