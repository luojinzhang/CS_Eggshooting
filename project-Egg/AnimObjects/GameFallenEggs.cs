using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameFallenEggs : AbstractGameObjects
    {
        protected List<GameEggs> listFallenEggs;
        protected List<DxAnimationSF> listBrokenEggs;
        public GameFallenEggs(DxInitGraphics graphics)
            : base(graphics)
        {
            this.listFallenEggs = new List<GameEggs>();
            this.listBrokenEggs = new List<DxAnimationSF>();
        }
        public void duplicateEggsFromListFallen(List<GameEggs> listFallenEggs)
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                for (int i = 0; i < listFallenEggs.Count; i++)
                {
                    this.listFallenEggs.Add(new GameEggs(this.graphics));
                    this.listFallenEggs[i].Color = listFallenEggs[i].Color;
                    this.listFallenEggs[i].checkColor();
                    this.listFallenEggs[i].BoEgg.XPosition = listFallenEggs[i].BoEgg.XPosition;
                    this.listFallenEggs[i].BoEgg.YPosition = listFallenEggs[i].BoEgg.YPosition;
                    this.listFallenEggs[i].VY = 9;
                    this.listFallenEggs[i].BoEgg.Width = listFallenEggs[i].BoEgg.Width;
                    this.listFallenEggs[i].BoEgg.Height = listFallenEggs[i].BoEgg.Height;
                    this.listFallenEggs[i].EggStatus = EggStatus.Fallen;
                }
            }
        }
        public void duplicateEggsFromListSameColor(List<GameEggs> listSameColor)
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                for (int i = 0; i < listSameColor.Count; i++)
                {
                    this.listBrokenEggs.Add(new DxAnimationSF(GameResources.yolksheet, 2, 1, 60, (double)(GameResources.yolksheet.Width / 2), (double)GameResources.yolksheet.Height, this.graphics, BitmapType.TRANSPARENT, 0xFF00FF));

                    this.listBrokenEggs[i].BoAnimation.YPosition = listSameColor[i].BoEgg.YPosition;
                    this.listBrokenEggs[i].BoAnimation.XPosition = listSameColor[i].BoEgg.XPosition;
                    this.listBrokenEggs[i].VY = 9;
                }
            }
        }

        public override void dispose()
        {
            for (int i = 0; i < this.listFallenEggs.Count; i++)
            {
                this.listFallenEggs[i].dispose();
            }
            for (int i = 0; i < this.listFallenEggs.Count; i++)
            {
                this.listBrokenEggs[i].Dispose();
            }
        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                for (int i = 0; i < this.listFallenEggs.Count; i++)
                {
                    if (this.listFallenEggs[i].BoEgg.YPosition + this.listFallenEggs[i].BoEgg.Height > 480)
                    {

                        this.listFallenEggs[i].dispose();
                        this.listFallenEggs.RemoveAt(i);
                        continue;
                    }
                    this.listFallenEggs[i].Draw();
                    this.listFallenEggs[i].BoEgg.YPosition += this.listFallenEggs[i].VY;

                    
                }
                for (int i = 0; i < this.listBrokenEggs.Count; i++)
                {
                    if (this.listBrokenEggs[i].BoAnimation.YPosition + this.listBrokenEggs[i].BoAnimation.Height > 480)
                    {

                        this.listBrokenEggs[i].Dispose();
                        this.listBrokenEggs.RemoveAt(i);
                        continue;
                    }
                    this.listBrokenEggs[i].ResetPlay();
                    this.listBrokenEggs[i].Play((int)this.listBrokenEggs[i].BoAnimation.XPosition, (int)this.listBrokenEggs[i].BoAnimation.YPosition, Animate.Continuous);
                    this.listBrokenEggs[i].BoAnimation.YPosition += this.listBrokenEggs[i].VY;

                    
                }
            }

        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            for (int i = 0; i < this.listFallenEggs.Count; i++)
            {
                this.listFallenEggs[i].BoEgg.RestoreSurface();
            }
            for (int i = 0; i < this.listBrokenEggs.Count; i++)
            {
                this.listBrokenEggs[i].Restore(this.graphics);
            }
        }
    }
}
