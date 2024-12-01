using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    class GameScore : AbstractGameObjects
    {
        protected DxImageObject boScoreDigits;
        List<int> listOfInts;
        protected int currentFrame;
        protected static int score;
        public static int Score
        {
            get { return GameScore.score; }
            set { GameScore.score = value; }
        }
        public GameScore(DxInitGraphics graphics)
            : base(graphics)
        {
            this.boScoreDigits = new DxImageObject(GameResources.ScoreFont, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boScoreDigits.Width = 17;
            this.boScoreDigits.Height = GameResources.ScoreFont.Height;
            this.listOfInts = new List<int>();
        }
        public override void dispose()
        {

            this.boScoreDigits.Dispose();

        }
        public override void Draw()
        {
            if (GameLogic.screenState == ScreenState.JungleScreen)
            {
                //converseScoreToImageObjects();
                //this.boScoreDigits[0].DrawBitmap(this.graphics.RenderSurface, (float)this.boScoreDigits[0].XPosition, (float)this.boScoreDigits[0].YPosition, (int)this.boScoreDigits[0].SourceX, (int)this.boScoreDigits[0].SourceY, this.boScoreDigits[0].Width, this.boScoreDigits[0].Height);
                //for (int i = 0; i < this.boScoreDigits.Count; i++)
                //{
                //    this.boScoreDigits[i].DrawBitmap(this.graphics.RenderSurface, (float)this.boScoreDigits[i].XPosition, (float)this.boScoreDigits[i].YPosition,
                //                                        (int)this.boScoreDigits[i].SourceX, (int)this.boScoreDigits[i].SourceY,
                //                                        this.boScoreDigits[i].Width, this.boScoreDigits[i].Height);
                //}

             
                for (int i = 0; i < this.listOfInts.Count; i++)
                {

                    //Console.WriteLine(score);

             
                    this.boScoreDigits.DrawBitmap(this.graphics.RenderSurface, 30 + i * this.boScoreDigits.Width, 70,
                                                   (int)(this.listOfInts[i] * this.boScoreDigits.Width), 0,
                                                   this.boScoreDigits.Width, this.boScoreDigits.Height);
                   
                }
            }
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;

            this.boScoreDigits.RestoreSurface();

        }
        public void converseScoreToImageObjects()
        {
            int temp = GameScore.score;
            listOfInts = new List<int>();
            while (temp > 0)
            {
                listOfInts.Add(temp % 10); // lay phan du tuc la lay digit hang don vi, chuc, ngan, chuc ngan ... ra 
                temp = temp / 10; // sau khi lay duoc digit hang nho nhat, chia cho 10 de lay ra phan nguyen con lai cua so do.
            }

            //while (listOfInts.Count < 6)
            //{
            //    listOfInts.Add(0);
            //}
            listOfInts.Reverse();

            //for (int i = 0; i < listOfInts.Count; i++)
            //{
            //    this.boScoreDigits.Add(new DxImageObject(GameResources.ScoreFont, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice));
            //    this.boScoreDigits[i].Width = 17;
            //    this.boScoreDigits[i].Height = GameResources.ScoreFont.Height;
            //    this.boScoreDigits[i].XPosition = 30 + i * this.boScoreDigits[i].Width;
            //    this.boScoreDigits[i].YPosition = 70;
            //    this.boScoreDigits[i].SourceX = listOfInts[i] * this.boScoreDigits[i].Width;
            //    this.boScoreDigits[i].SourceY = 0;
            //}
        }
    }
}
