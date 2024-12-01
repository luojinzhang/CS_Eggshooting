using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{

    class GameFlyingEggs : AbstractGameObjects
    {
        protected List<GameEggs> listFlyingEggs;


        public List<GameEggs> ListFlyingEggs
        {
            get { return listFlyingEggs; }
            set { listFlyingEggs = value; }
        }
        public GameFlyingEggs(DxInitGraphics graphics)
            : base(graphics)
        {

            listFlyingEggs = new List<GameEggs>();
            addFlyingEgg();
            this.listFlyingEggs[0].VFly = -10;
        }
        public override void dispose()
        {
            for (int i = 0; i < listFlyingEggs.Count; i++)
            {
                this.listFlyingEggs[i].dispose();
            }
        }
        public override void Draw()
        {
            

            for (int i = 0; i < listFlyingEggs.Count; i++)
            {
                if (this.listFlyingEggs[i].EggStatus == EggStatus.Fly)
                {
                    this.listFlyingEggs[i].Draw();
                }
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            for (int i = 0; i < listFlyingEggs.Count; i++)
            {
                this.listFlyingEggs[i].restore(this.graphics);
            }
        }
        public void addFlyingEgg()
        {
            listFlyingEggs.Add(new GameEggs(this.graphics));
            listFlyingEggs[listFlyingEggs.Count - 1].Color = GameBackgroundEggs.EggOnRubberband.Color;
            listFlyingEggs[listFlyingEggs.Count - 1].checkColor();
            listFlyingEggs[listFlyingEggs.Count - 1].EggStatus = EggStatus.Idle;
            this.listFlyingEggs[listFlyingEggs.Count - 1].VFly = -10;

        }
        public void fly(double cursorX, double cursorY)
        {
            if (this.ListFlyingEggs.Count - 1 >= 0)
            {

                this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].BoEgg.XPosition = GameBackgroundEggs.EggOnRubberband.BoEgg.XPosition;
                this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].BoEgg.YPosition = GameBackgroundEggs.EggOnRubberband.BoEgg.YPosition;

                if (cursorX - GameBackgroundEggs.EggOnRubberband.centerX() != 0)
                {
                    double heSoGoc;
                    //update Pos of flyingEgg
                    double offSetY = GameBackgroundEggs.EggOnRubberband.centerY();
                    double offSetX = GameBackgroundEggs.EggOnRubberband.centerX();

                    heSoGoc = Math.Abs((cursorY - offSetY)) / Math.Abs((cursorX - offSetX));

                    this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VX = Math.Sqrt(Math.Pow(this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VFly, 2)
                                                                                                            / (Math.Pow(heSoGoc, 2) + 1));

                    this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VY = -1 * Math.Sqrt(Math.Pow(this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VFly, 2)
                                                                                                            / ((1 / Math.Pow(heSoGoc, 2)) + 1));

                    if (cursorX < GameBackgroundEggs.EggOnRubberband.centerX())
                    {
                        this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VX *= -1;
                    }
                }

                else if (cursorX - GameBackgroundEggs.EggOnRubberband.centerX() == 0)
                {
                    this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VX = 0;
                    this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VY = this.ListFlyingEggs[this.ListFlyingEggs.Count - 1].VFly;
                }
            }
        }
    }
}
