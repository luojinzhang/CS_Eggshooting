using System;
using System.Collections.Generic;
using System.Text;

namespace project_Egg
{
    public enum EggStatus
    {
        Normal,
        Explose,
        Idle,
        Fly,
        Fallen,
    }
    public enum EggColor
    {
        Red,
        Blue,
        Green,
    }
    class GameEggs : AbstractGameObjects
    {
        protected DxImageObject boEgg;
        protected DxAnimationSF boExplosion;
        protected EggStatus eggStatus;
        protected EggColor color;
        protected static Random rand = new Random();
        protected int currentFrame;
        protected double vFly, vX, vY;

        public double VY
        {
            get { return vY; }
            set { vY = value; }
        }

        public double VX
        {
            get { return vX; }
            set { vX = value; }
        }

        public double VFly
        {
            get { return vFly; }
            set { vFly = value; }
        }
        public DxImageObject BoEgg
        {
            get { return boEgg; }
        }
        public EggStatus EggStatus
        {
            get { return eggStatus; }
            set { eggStatus = value; }
        }
        public EggColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public GameEggs(DxInitGraphics graphics)
            : base(graphics)
        {
            this.boEgg = new DxImageObject(GameResources.EggSheet, BitmapType.TRANSPARENT, 0xFF00FF, this.graphics.DDDevice);
            this.boEgg.Width = 35;
            this.boEgg.Height = 39;
            this.rollColor();
            //this.boExplosion = new DxAnimationSF(GameResources.boomsheet, 8, 2, 60, this.graphics, BitmapType.TRANSPARENT, 0xFF00FF);

        }

        public override void Draw()
        {
            if (this.eggStatus == EggStatus.Normal
                || this.eggStatus == project_Egg.EggStatus.Fly)
            {
                if (this.boEgg.YPosition + 39.0 < 480)
                {
                    this.boEgg.DrawBitmap(this.graphics.RenderSurface, (float)this.boEgg.XPosition, (float)this.boEgg.YPosition, this.currentFrame * this.boEgg.Width, 0, this.boEgg.Width, this.boEgg.Height);
                }
            }
            else if (this.eggStatus == EggStatus.Explose)
            {
                if (this.boEgg.XPosition - 44 + 88 < 640
                    && this.boEgg.YPosition - 123.0 / 2 >= 0
                    && this.boEgg.YPosition + 123.0 < 480)
                {
                    this.boExplosion.Show = true;
                    this.boExplosion.Play((int)this.boEgg.XPosition - 44, (int)this.boEgg.YPosition, Animate.SingleSequence);
                }
                else if (this.boEgg.YPosition >= 0
                    && this.boEgg.XPosition - 44 + 88 < 640
                    && this.boEgg.YPosition + 123.0 < 480)
                {
                    this.boExplosion.Show = true;
                    this.boExplosion.Play((int)this.boEgg.XPosition, (int)this.boEgg.YPosition, Animate.SingleSequence);
                }
                else
                {
                    this.boExplosion.Show = false;
                }
                if (this.boExplosion.Show == false)
                {
                    this.eggStatus = project_Egg.EggStatus.Idle;
                    this.boExplosion.Dispose();
                    this.boExplosion.ResetPlay();
                }
            }
            else if (this.eggStatus == project_Egg.EggStatus.Fallen)
            {
                this.boEgg.DrawBitmap(this.graphics.RenderSurface, (float)this.boEgg.XPosition, (float)this.boEgg.YPosition, this.currentFrame * this.boEgg.Width, 0, this.boEgg.Width, this.boEgg.Height);

            }
        }
        public void initBoExplosion() /* can init BoExplosion truoc khi su dung, init khi vua explose */
        {
            this.boExplosion = new DxAnimationSF(GameResources.boomsheet, 8, 2, 60, (double)(GameResources.boomsheet.Width / 8), (double)(GameResources.boomsheet.Height / 2), this.graphics, BitmapType.TRANSPARENT, 0xFF00FF);

        }
        public override void dispose()
        {
            this.boEgg.Dispose();
            //this.boExplosion.Dispose();
        }
        public override void restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boEgg.RestoreSurface();
            //this.boExplosion.Restore(this.graphics);
        }
        public void rollColor()
        {
            this.currentFrame = rand.Next(0, 3);
            if (this.currentFrame == 0)
            {
                this.color = EggColor.Red;
            }
            else if (this.currentFrame == 1)
            {
                this.color = EggColor.Blue;
            }
            else if (this.currentFrame == 2)
            {
                this.color = EggColor.Green;
            }
        }
        public void updateEggXPos(double XPos)
        {
            this.boEgg.XPosition = XPos;
        }
        public void updateEggYPos(double YPos)
        {
            this.boEgg.YPosition = YPos;
        }
        public void checkColor()
        {

            if (this.color == EggColor.Red)
            {
                this.currentFrame = 0;
            }
            else if (this.color == EggColor.Blue)
            {
                this.currentFrame = 1;
            }
            else if (this.color == EggColor.Green)
            {
                this.currentFrame = 2;
            }
        }
        public double centerX()
        {
            double centerX = this.boEgg.XPosition + this.boEgg.Width / 2;
            return centerX;
        }
        public double centerY()
        {
            double centerY = this.boEgg.YPosition + this.boEgg.Height / 2;
            return centerY;
        }
    }
}

