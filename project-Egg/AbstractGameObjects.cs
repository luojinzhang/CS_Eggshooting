using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public abstract class AbstractGameObjects
    {
        protected DxInitGraphics graphics;
        public AbstractGameObjects(DxInitGraphics graphics)
        {
            this.graphics = graphics;
        }
        public abstract void Draw();
        public abstract void dispose();
        public abstract void restore(DxInitGraphics graphics);
    }
}
