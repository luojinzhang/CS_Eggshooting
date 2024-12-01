using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;         // Added to take care of the Control class
using Microsoft.DirectX;
using Microsoft.DirectX.DirectDraw;

namespace project_Egg
{
    public class DxInitGraphics
    {
        protected Control target = null;
        protected Device graphicsDevice = null;
        protected Clipper graphicsClipper = null;
        protected Surface surfacePrimary = null;
        protected Surface surfaceSecondary = null;

        /// <summary>
        /// The DirectDraw Device
        /// </summary>
        public Device DDDevice
        {
            get
            {
                return graphicsDevice;
            }
        }

        /// <summary>
        /// This surface can be accessed to render to.
        /// </summary>
        public Surface RenderSurface
        {
            get
            {
                return surfaceSecondary;
            }
        }

        /// <summary>
        /// Constructor. Initializes DirectDraw Device and surfaces.
        /// </summary>
        /// <param name="RenderControl">
        /// The control the device is connected to.
        /// </param>
        public DxInitGraphics(Control RenderControl)
        {
            // Save reference to target control
            this.target = RenderControl;

            // Create DirectDraw device
            graphicsDevice = new Device();

#if DEBUG
            // In debug mode, use windowed level
            graphicsDevice.SetCooperativeLevel(this.target, CooperativeLevelFlags.Normal);
#else
            

			// In release mode, use fullscreen...
			graphicsDevice.SetCooperativeLevel(this.target, CooperativeLevelFlags.FullscreenExclusive);
			// ...and 640x480x16 resolution with 60Hz frequency
			graphicsDevice.SetDisplayMode(640, 480, 32, 60, true);
#endif

            // create surfaces
            this.createSurfaces();
        }

        /// <summary>
        /// This method creates the primary and secondary surfaces
        /// </summary>
        protected void createSurfaces()
        {
            // Every surface needs a description
            // This is where you set the parameters for the surface
            SurfaceDescription desc = new SurfaceDescription();

            // First we want to create a primary surface
            desc.SurfaceCaps.PrimarySurface = true;

#if !DEBUG
			// In release mode, we enable flipping, set the complex
			// flag and tell the surface that we will use one back
			// buffer
			desc.SurfaceCaps.Flip = true;
			desc.SurfaceCaps.Complex = true;
			desc.BackBufferCount = 1;
#endif

            // Create the surface
            surfacePrimary = new Surface(desc, graphicsDevice);

            // To build the secondary surface, we need 
            // a new description -> clear all values
            desc.Clear();

#if DEBUG
            // In debug mode, we simply copy the primary surfaces
            // dimensions and create a offscreenplain secondary
            // surface
            desc.Width = surfacePrimary.SurfaceDescription.Width;
            desc.Height = surfacePrimary.SurfaceDescription.Height;
            desc.SurfaceCaps.OffScreenPlain = true;
            surfaceSecondary = new Surface(desc, this.graphicsDevice);
#else
			// In release mode, we set the backbuffer flag to true
			// and retrieve a backbuffer surface from the primary
			// surface
			desc.SurfaceCaps.BackBuffer = true;
			surfaceSecondary = surfacePrimary.GetAttachedSurface(desc.SurfaceCaps);
#endif

            // This is the clipper for the secondary surface
            // -> connect it to the target control
            graphicsClipper = new Clipper(graphicsDevice);
            graphicsClipper.Window = target;

            // Attach clipper to the surface
            surfacePrimary.Clipper = this.graphicsClipper;
        }

        /// <summary>
        /// This method flips the secondary surface to the
        /// primary one, thus drawing its content to the screen.
        /// </summary>
        public void Flip()
        {
            // First we check if the target control
            // is created yet.
            if (!this.target.Created)
            {
                return;
            }

            // Now check if both surfaces are there.
            if (surfacePrimary == null || surfaceSecondary == null)
            {
                return;
            }

            // Try to Draw or Flip, depending on compile mode
            try
            {
#if DEBUG
                surfacePrimary.Draw(surfaceSecondary, DrawFlags.Wait);
#else
				surfacePrimary.Flip(surfaceSecondary, FlipFlags.Wait);
#endif
            }
            catch (SurfaceLostException)
            {
                // On activation of power saving mode 
                // and in other situations we may lose the surfaces
                // and have to recreate them
                this.createSurfaces();
            }
        }
    }
}
