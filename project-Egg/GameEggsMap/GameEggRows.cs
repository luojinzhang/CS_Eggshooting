using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public enum EggRowStatus
    {
        Odd,
        Even,
    }
    class GameEggRows : AbstractGameObjects
    {
        protected EggRowStatus eggRowStatus;
        protected List<GameEggs> listEggs;
        protected int noOfEggs;
        protected double YPos;
        protected double offSetXPos = 245;
        DxTimer t;
        public List<GameEggs> ListEggs
        {
            get { return listEggs; }
        }
        public EggRowStatus EggRowStatus
        {
            get { return eggRowStatus; }
            set { eggRowStatus = value; }
        }
        public double YPosition
        {
            get { return YPos; }
            set { YPos = value; }
        }
        public GameEggRows(DxInitGraphics graphics)
            : base(graphics)
        {

        }
        public GameEggRows(DxInitGraphics graphics, EggRowStatus status)
            : base(graphics)
        {
            t = new DxTimer();
            this.eggRowStatus = status;
            checkNoOfEggs();
            this.listEggs = new List<GameEggs>();
            for (int i = 0; i < this.noOfEggs; i++)
            {
                this.listEggs.Add(new GameEggs(this.graphics));
            }
            for (int i = 0; i < this.listEggs.Count; i++)
            {
                if (this.eggRowStatus == EggRowStatus.Odd)
                {
                    this.listEggs[i].updateEggXPos(i * this.listEggs[i].BoEgg.Width + offSetXPos);
                    this.listEggs[i].updateEggYPos(this.YPos);
                }
                else if (this.eggRowStatus == EggRowStatus.Even)
                {
                    this.listEggs[i].updateEggXPos(i * this.listEggs[i].BoEgg.Width + offSetXPos + this.listEggs[i].BoEgg.Width / 2);
                    this.listEggs[i].updateEggYPos(this.YPos);
                }
            }
        }

        public void checkNoOfEggs()
        {
            if (this.eggRowStatus == EggRowStatus.Even)
            {
                this.noOfEggs = 8;
            }
            else if (this.eggRowStatus == EggRowStatus.Odd)
            {
                this.noOfEggs = 9;
            }
        }
        public override void Draw()
        {
            for (int i = 0; i < this.listEggs.Count; i++)
            {
                if (this.YPos + this.listEggs[i].BoEgg.Height >= GameRope.Limit 
                         && this.listEggs[i].EggStatus == EggStatus.Normal) /* kiem tra neu trung vuot muc va o tinh trang normal thi ve trung chop chop*/
                {
                    t.markTime();
                    if (t.msElapsed() <= 400)
                    {
                        this.listEggs[i].Draw();
                    }
                    if (t.msElapsed() > 800)
                    {
                        this.t.resetTime();
                    }

                }
                else
                {
                    this.listEggs[i].Draw(); /* truong hop con lai ve binh thuong*/
                }
            }
        }
        public override void dispose()
        {
            for (int i = 0; i < this.listEggs.Count; i++)
            {
                this.listEggs[i].dispose();
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            for (int i = 0; i < this.listEggs.Count; i++)
            {
                this.listEggs[i].restore(this.graphics);
            }
        }
        public void updateYPosOfEggs()
        {
            for (int i = 0; i < listEggs.Count; i++)
            {
                this.listEggs[i].BoEgg.YPosition = this.YPos;
            }
        }
    }
}
