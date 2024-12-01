using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectDraw;



namespace project_Egg
{
    public class DxAnimationSF
    {
        // The instantiated TimerAPI object, will be used to control 
        // Frames per Second
        protected DxTimer t = new DxTimer();
        protected DxImageObject boAnimation;     // Image Object for Alphabets & Digits  
        protected DxInitGraphics graphics;
        double frameTimeInterval;
        int picsW, picsH;
        int aGridW, aGridH;
        int blitX, blitY, blitW, blitH;
        double vY;
        bool show = false;
        public double VY
        {
            get { return vY; }
            set { vY = value; }
        }
        public DxImageObject BoAnimation
        {
            get { return boAnimation; }
        }
        public bool Show
        {
            get { return show; }
            set { show = value; }
        }
        /// <summary>
        /// Constructor that takes frames in a single Image bitmap and Animates them.
        /// </summary>
        /// <param name="aPathImage">Name of the embedded Image in the format: [namespac].[folder].[imagename].[imageextn]></param>
        /// <param name="NoOfFramesW">No of Frames available in the Width of the Image</param>
        /// <param name="NoOfFrameRowsH">No of Rows of such Frames</param>
        /// <param name="fps">Frames Per Second animation rate</param>
        /// <param name="gHandler">Direct Draw Graphics Handler</param>
        public DxAnimationSF(string aPathImage, int NoOfFramesW, int NoOfFrameRowsH, int fps, DxInitGraphics gHandler)
        {
            // Receive the Direct Draw Graphics Handler
            graphics = gHandler;

            // Create Image Object for the Alphabets and Digit
            this.boAnimation = new DxImageObject(aPathImage,
                                                  BitmapType.TRANSPARENT, 0,
                                                  this.graphics.DDDevice);
            // Assign to instance variables
            picsW = NoOfFramesW;
            picsH = NoOfFrameRowsH;

            // Get the animation grids Width and Height so that the limits are 
            // not exceeded in loop operations
            aGridW = this.boAnimation.Width;
            aGridH = this.boAnimation.Height;

            // Calculate time interval between frames
            frameTimeInterval = 1000 / fps;

            // Set the first blit coordinates
            blitX = 0;
            blitY = 0;
            blitW = aGridW / NoOfFramesW;
            blitH = aGridH / NoOfFrameRowsH;

            //
        }

        public DxAnimationSF(System.Drawing.Bitmap image, int NoOfFramesW, int NoOfFrameRowsH,
                            int fps, double width, double height, DxInitGraphics gHandler, BitmapType Type,
                            int colKey)
        {
            // Receive the Direct Draw Graphics Handler
            graphics = gHandler;

            // Create Image Object for the Alphabets and Digit
            this.boAnimation = new DxImageObject(image,
                                                  Type, colKey,
                                                  this.graphics.DDDevice);
            //this.boAnimation.Width =(int) width;
            //this.boAnimation.Height =(int) height;
            // Assign to instance variables
            picsW = NoOfFramesW;
            picsH = NoOfFrameRowsH;

            // Get the animation grids Width and Height so that the limits are 
            // not exceeded in loop operations
            aGridW = this.boAnimation.Width;
            aGridH = this.boAnimation.Height;

            // Calculate time interval between frames
            frameTimeInterval = 1000 / fps;

            // Set the first blit coordinates
            blitX = 0;
            blitY = 0;
            blitW = aGridW / NoOfFramesW;
            blitH = aGridH / NoOfFrameRowsH;

            //
        }

        /// <summary>
        /// Helps restore a lost Animated frame surface in case of a context switch
        /// </summary>
        /// <param name="gHandler">Direct Draw Graphics Handler</param>


        /// <summary>
        /// Plays the Animation. For this this function need to be polled continously
        /// from within the Game Loop
        /// </summary>
        /// <param name="x">X Coordinate of the screen</param>
        /// <param name="y">Y Coordinate of the screen</param>
        /// <param name="type">Can be Continuous or Single Sequence</param>
        public void Play(int x, int y, Animate type)
        {
            if (!show) return;
            t.markTime();
            if (t.msElapsed() > frameTimeInterval) // Change the frame after frameTimeInterval
            {
                blitX = blitX + blitW;
                if (blitX > aGridW - 1)
                {
                    blitX = 0;
                    blitY = blitY + blitH;
                    if (blitY > aGridH - 1)
                    {
                        blitX = 0;
                        blitY = 0;
                        if (type == Animate.SingleSequence)
                        {
                            show = false;
                            return;
                        }
                    }
                }
                t.resetTime();
            }
            // Set up the Rectangle coord to cut out from the source Surface
            Rectangle rcRect = new Rectangle(blitX, blitY, blitW, blitH);

            // DrawFast Method to Blit
            if (this.boAnimation.ObjectType == BitmapType.TRANSPARENT)
                this.graphics.RenderSurface.DrawFast(x,
                                       y,
                                       boAnimation.SurfaceOfBitmap,
                                       rcRect,
                                       DrawFastFlags.SourceColorKey
                                       | DrawFastFlags.Wait);
            else
                this.graphics.RenderSurface.DrawFast(x,
                                       y,
                                       boAnimation.SurfaceOfBitmap,
                                       rcRect,
                                       DrawFastFlags.NoColorKey
                                       | DrawFastFlags.Wait);

            //graphics.RenderSurface.DrawFast(x, y, boAnimation.SurfaceOfBitmap, rcRect, DrawFastFlags.SourceColorKey | DrawFastFlags.Wait);
        }

        public void ResetPlay()
        {
            show = true;
        }

        public bool isPlaying()
        {
            return show;
        }
        public void Dispose()
        {
            this.boAnimation.Dispose();
        }
        public void Restore(DxInitGraphics graphics)
        {
            this.graphics = graphics;
            this.boAnimation.RestoreSurface();
        }
    }
}



