using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameBackgroundEggs : AbstractGameObjects
    {
        protected static GameEggs eggOnSteggy;

        public static GameEggs EggOnSteggy
        {
            get { return GameBackgroundEggs.eggOnSteggy; }
            set { GameBackgroundEggs.eggOnSteggy = value; }
        }
        protected static GameEggs eggOnRubberband;
        public static GameEggs EggOnRubberband
        {
            get { return GameBackgroundEggs.eggOnRubberband; }
            set { GameBackgroundEggs.eggOnRubberband = value; }
        }
        public GameBackgroundEggs(DxInitGraphics graphics)
            : base(graphics)
        {
            eggOnSteggy = new GameEggs(this.graphics);
            setPosForEggOnSteggy();

            eggOnRubberband = new GameEggs(this.graphics);
            setPosForEggOnRubberband();
        }
        
        public void setPosForEggOnRubberband()
        {
            // vi tri trung tren day chay tu 372 ~ 412
            // vi tri cua handleX tu 378 ~ 438
            double heSoDiChuyen = 40.0 / 60.0; // he so giua eggOnBand va HandleX
            int offSetX = 123;
            double temp = GameRubberband.HandleX - 6;
            eggOnRubberband.BoEgg.XPosition = (GameRubberband.HandleX - 6) * heSoDiChuyen + offSetX;
            eggOnRubberband.updateEggYPos(GameRubberband.HandleY - 30);
        }

        private void setPosForEggOnSteggy()
        {
            eggOnSteggy.updateEggXPos(237 + 65); //tinh tren toa do x cua steggy
            eggOnSteggy.updateEggYPos(380 + 30); //tinh tren toa do y cua steggy
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                setPosForEggOnRubberband();
                eggOnSteggy.Draw();
                eggOnRubberband.Draw();
            }
        }
        public override void dispose()
        {
            eggOnRubberband.dispose();
            eggOnSteggy.dispose();
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            eggOnSteggy.restore(this.graphics);
            eggOnRubberband.restore(this.graphics);
        }

        public void checkCurrentFrameByColor()
        {
            eggOnRubberband.checkColor();
            eggOnSteggy.checkColor();
        }
    }
}
