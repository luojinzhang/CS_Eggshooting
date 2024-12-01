using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameMap : AbstractGameObjects
    {
        protected List<GameEggRows> listEggRows;
        protected DxTimer t;
        protected int loopForEggsScrollDown;
        public List<GameEggRows> ListEggRows
        {
            get { return listEggRows; }
        }
        public GameMap(DxInitGraphics graphics)
            : base(graphics)
        {
            this.t = new DxTimer();
            this.listEggRows = new List<GameEggRows>();
            this.listEggRows.Add(new GameEggRows(this.graphics, EggRowStatus.Even));
            this.listEggRows.Add(new GameEggRows(this.graphics, EggRowStatus.Odd));
            loopForEggsScrollDown = 3000;
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                t.markTime();

                updateYPosOfRows();
                for (int i = 0; i < listEggRows.Count; i++)
                {
                    this.listEggRows[i].Draw();
                }

                if (this.listEggRows[0].YPosition + 39 /*39 la chieu cao cua 1 frame trung*/
                                                        <= GameRope.Limit)
                {
                    if (GameDinosaurs.DinoStatus != DinosaurStatus.Normal)
                    {
                        GameDinosaurs.DinoStatus = DinosaurStatus.Normal;
                    }
                    if (t.msElapsed() >= loopForEggsScrollDown)
                    {
                        addNewRowOnTop();
                        t.resetTime();
                    }
                }
                else
                {


                   

                    if (GameDinosaurs.DinoStatus != DinosaurStatus.Panic)
                    {
                        GameDinosaurs.DinoStatus = DinosaurStatus.Panic;
                        GameLogic.soundWarning.Play();
                    }

                    if (t.msElapsed() >= 5000)
                    {
                        t.resetTime();
                        GameLogic.screenState = ScreenState.GameOver;
                        
                        // cho game over
                        // foot xuat hien

                    }
                }
            }
        }
        public override void dispose()
        {
            for (int i = 0; i < listEggRows.Count; i++)
            {
                this.listEggRows[i].dispose();
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            for (int i = 0; i < listEggRows.Count; i++)
            {
                this.listEggRows[i].restore(this.graphics);
            }
        }
        public void addNewRowOnTop()
        {
            if (this.listEggRows[listEggRows.Count - 1].EggRowStatus == EggRowStatus.Even)
            {
                this.listEggRows.Add(new GameEggRows(this.graphics, EggRowStatus.Odd));
            }
            else if (this.listEggRows[listEggRows.Count - 1].EggRowStatus == EggRowStatus.Odd)
            {
                this.listEggRows.Add(new GameEggRows(this.graphics, EggRowStatus.Even));
            }
        }
        public void addNewRowAtBottom()
        {
            if (this.listEggRows[0].EggRowStatus == EggRowStatus.Even)
            {
                this.listEggRows.Insert(0, new GameEggRows(this.graphics, EggRowStatus.Odd));

            }
            else if (this.listEggRows[0].EggRowStatus == EggRowStatus.Odd)
            {
                this.listEggRows.Insert(0, new GameEggRows(this.graphics, EggRowStatus.Even));
            }
            for (int i = 0; i < this.listEggRows[0].ListEggs.Count; i++)
            {
                this.listEggRows[0].ListEggs[i].EggStatus = EggStatus.Idle;
            }
            this.listEggRows[0].YPosition = 39 * (this.listEggRows.Count - 1);
            this.updateYPosOfRows();
        }
        public void updateYPosOfRows()
        {
            int j = 0;
            for (int i = this.listEggRows.Count - 1; i >= 0; i--)
            {
                this.listEggRows[i].YPosition = j * 39; //39 la chieu cao 1 frame trung
                this.listEggRows[i].updateYPosOfEggs();
                j++;
            }
        }

    }
}
