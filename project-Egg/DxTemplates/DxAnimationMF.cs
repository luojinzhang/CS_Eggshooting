using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectDraw;


namespace project_Egg
{
    public enum Animate
    {
        Continuous = 0,
        SingleSequence = 1
    }

    public class DxAnimationMF
    {
        // The instantiated TimerAPI object, will be used to control 
        // Frames per Second
        protected DxTimer t = new DxTimer();
        protected DxImageObject[] boAnimation;     // Image Object for Alphabets & Digits  
        protected DxInitGraphics graphics;
        double frameTimeInterval;

        int currentFrame;
        int totalFrames;

        bool Show = true;
        
        /// <summary>
        /// Constructor that takes frames in a single Image bitmap and Animates them.
        /// </summary>
        /// <param name="frm">Name of the embedded Image in the format: [namespac].[folder].[imagename].[imageextn]></param>
        /// <param name="fps">Frames Per Second animation rate</param>
        /// <param name="gHandler">Direct Draw Graphics Handler</param>
        public DxAnimationMF(DxImageObject[] frm, int fps, DxInitGraphics gHandler)
        {
            // Receive the Direct Draw Graphics Handler
            graphics = gHandler;

            // Save the Animation image objects array
            boAnimation = frm;

            // Calculate time interval between frames
            frameTimeInterval = 1000 / fps;

            // Store the total no of frames
            totalFrames = frm.Length-1;

            // set the current frame to the first one
            currentFrame = 0;
        }

        /// <summary>
        /// Helps restore a lost Animated frame surface in case of a context switch
        /// </summary>
        /// <param name="gHandler">Direct Draw Graphics Handler</param>
        public void RestoreSurface(DxInitGraphics gHandler)
        {
            int i;
            // Receive the Direct Draw Graphics Handler
            graphics = gHandler;
            for(i=0;i<=totalFrames;i++)
                boAnimation[i].RestoreSurface();
        }

        /// <summary>
        /// Plays the Animation. For this this function need to be polled continously
        /// from within the Game Loop
        /// </summary>
        /// <param name="x">X Coordinate of the screen</param>
        /// <param name="y">Y Coordinate of the screen</param>
        /// <param name="type">Can be Continuous or Single Sequence</param>
        public void Play(int x, int y, Animate type)
        {
            if (!Show) return;
            t.markTime();
            if (t.msElapsed() > frameTimeInterval) // Change the frame after frameTimeInterval
            {
                if (currentFrame <= totalFrames)
                {
                    currentFrame++;
                    if (type == Animate.SingleSequence && currentFrame > totalFrames)
                    {
                        Show = false;
                        t.resetTime();
                        currentFrame = 0;
                        return;
                    }
                    if (currentFrame > totalFrames)
                    {
                        currentFrame = 0;
                    }
                }
                t.resetTime();
            }

            // DrawFast Method to Blit
            graphics.RenderSurface.DrawFast(x, y, boAnimation[currentFrame].SurfaceOfBitmap, DrawFastFlags.SourceColorKey | DrawFastFlags.Wait);
        }

        public void ResetPlay()
        {
            Show = true;
        }

        public bool isPlaying()
        {
            return Show;
        }
    }
}
