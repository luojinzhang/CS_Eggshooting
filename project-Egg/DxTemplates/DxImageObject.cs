using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX.DirectDraw;

namespace project_Egg
{
    /// <summary>
    /// To Enumeration to distinguish the type of bitmap
    /// </summary>
    public enum BitmapType
    {
        SOLID = 0,
        TRANSPARENT = 1
    }
    public enum ButtonState
    {
        Hovered,
        Normal,
    }
    public class DxImageObject
    {
        #region Variables
        /// <summary>
        /// Button state if create buttons
        protected ButtonState btState;

        /// Horizontal position
        /// </summary>
        protected double dXPos = 0.0f;

        /// <summary>
        /// Vertical position
        /// </summary>
        protected double dYPos = 0.0f;

        /// <summary>
        /// Last horizontal position
        /// </summary>
        protected double dLastXPos = 0.0f;

        /// <summary>
        /// Last vertical position
        /// </summary>
        protected double dLastYPos = 0.0f;

        /// <summary>
        /// Width
        /// </summary>
        protected int iWidth = 0;

        /// <summary>
        /// Height
        /// </summary>
        protected int iHeight = 0;

        /// <summary>
        /// The dimensions of the bitmap
        /// as System.Drawing.Rectangle object
        /// </summary>
        protected Rectangle objectSizeRect = Rectangle.Empty;

        /// <summary>
        /// The position of the bitmap
        /// as rectangle (= integer values).
        /// </summary>
        protected Rectangle rectPosition = Rectangle.Empty;

        /// <summary>
        /// Defines the visibility of this object
        /// </summary>
        protected bool bVisible = true;

        /// <summary>
        /// The source bitmap
        /// </summary>
        protected Bitmap sourceBitmap;

        /// <summary>
        /// Type of the source bitmap
        /// </summary>
        protected BitmapType objectType;

        //source X, source Y
        protected double sourceX, sourceY;



        /// <summary>
        /// Target device for this object
        /// </summary>
        protected Device targetDevice;

        /// <summary>
        /// The objects surface description
        /// </summary>
        protected SurfaceDescription surfaceDesc;

        /// <summary>
        /// The objects surface
        /// </summary>
        protected Surface bitmapSurface;

        protected ColorKey tempKey;

        protected int colKey;
        #endregion

        #region Properties

        /// <summary>
        /// The current horizontal position
        /// </summary>
        public double SourceY
        {
            get { return sourceY; }
            set { sourceY = value; }
        }

        public double SourceX
        {
            get { return sourceX; }
            set { sourceX = value; }
        }

        public BitmapType ObjectType
        {
            get { return objectType; }
        }
        public double XPosition
        {
            get
            {
                return dXPos;
            }
            set
            {
                // save last position
                dLastXPos = dXPos;

                // save value
                dXPos = value;

                // set integer position
                rectPosition = new Rectangle(new Point(Convert.ToInt32(dXPos), rectPosition.Y),
                                             new Size(this.Width, this.Height));
            }
        }

        /// <summary>
        /// The current vertical position
        /// </summary>
        public double YPosition
        {
            get
            {
                return dYPos;
            }
            set
            {
                // save last position
                dLastYPos = dYPos;

                // save value
                dYPos = value;

                // set integer position
                rectPosition = new Rectangle(new Point(rectPosition.X, Convert.ToInt32(dYPos)),
                                             new Size(this.Width, this.Height));
            }
        }

        /// <summary>
        /// Current width
        /// </summary>
        public int Width
        {
            get
            {
                return iWidth;
            }
            set
            {
                iWidth = value;
                //objectSizeRect = new Rectangle(new Point(Convert.ToInt32(sourceX), Convert.ToInt32(sourceY))
                //                                , new Size(this.Width, this.Height));
            }
        }

        /// <summary>
        /// Current height
        /// </summary>
        public int Height
        {
            get
            {
                return iHeight;
            }
            set
            {
                iHeight = value;
            }
        }

        /// <summary>
        /// Flag for visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return bVisible;
            }
            set
            {
                bVisible = value;
            }
        }

        /// <summary>
        /// Last horizontal position. Read only.
        /// </summary>
        public double LastXPosition
        {
            get
            {
                return dLastXPos;
            }
        }

        /// <summary>
        /// Last vertical position. Read only.
        /// </summary>
        public double LastYPosition
        {
            get
            {
                return dLastYPos;
            }
        }

        /// <summary>
        /// The position of the bitmap
        /// as rectangle (= integer values).
        /// </summary>
        public Rectangle Position
        {
            get
            {
                return rectPosition;
            }
        }

        /// <summary>
        /// The objects bitmap
        /// </summary>
        public Bitmap BitmapOfObject
        {
            get
            {
                return sourceBitmap;
            }
        }

        /// <summary>
        /// The objects surface
        /// </summary>
        public Surface SurfaceOfBitmap
        {
            get
            {
                return bitmapSurface;
            }
        }
        public ButtonState BtState
        {
            get { return btState; }
            set { btState = value; }
        }
        #endregion

        /// <summary>
        /// Constructor that takes a Image bitmap
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap Image object</param>
        /// <param name="ObjectType">Desired bitmap type i.e SOLID or TRANSPARENT</param>
        /// <param name="cKey">Desired Colorkey for Image. Will be applied only if BitmapType is equal to TRANSPARENT.</param>
        /// <param name="TargetDevice">DirectDraw target device</param>
        public DxImageObject(Bitmap SourceBitmap, BitmapType ObjectType, int cKey, Device TargetDevice)
        {
            // Save references to the source bitmap,
            // to the bitmap type and the device
            sourceBitmap = SourceBitmap;
            objectType = ObjectType;
            targetDevice = TargetDevice;

            // Create a rectangle with the bitmaps dimensions
            objectSizeRect = new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height);

            // Create a rectangle with the bitmaps position
            rectPosition = new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height);

            // Save width and height seperately
            iWidth = SourceBitmap.Width;
            iHeight = SourceBitmap.Height;

            // Build surface description
            initializeSurfaceDescription();

            // Create ColorKey object
            tempKey = new ColorKey();

            // Save Colorkey value
            colKey = cKey;

            // Create surface
            initializeSurface(cKey);
        }

        /// <summary>
        /// Constructor that takes a resource string i.e. path to the image file.
        /// </summary>
        /// <param name="ObjectType">Desired bitmap type i.e SOLID or TRANSPARENT</param>
        /// <param name="TargetDevice">DirectDraw target device</param>
        /// <param name="cKey">Desired Colorkey for Image. Will be applied only if BitmapType is equal to TRANSPARENT.</param>
        /// <param name="AssemblyImageResource">Resource to images imbedded in the executables</param>
        public DxImageObject(BitmapType ObjectType, Device TargetDevice, int cKey, string AssemblyImageResource)
        {
            // Create a new bitmap from the resource
            sourceBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream(AssemblyImageResource));

            // Save bitmap type and target device
            objectType = ObjectType;
            targetDevice = TargetDevice;

            // Create a rectangle with the bitmaps dimensions 
            objectSizeRect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);

            // Create a rectangle with the bitmaps position
            rectPosition = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);

            // Save width and height seperately
            iWidth = sourceBitmap.Width;
            iHeight = sourceBitmap.Height;

            // Build surface description
            initializeSurfaceDescription();

            // Create ColorKey object
            tempKey = new ColorKey();

            // Save Colorkey value
            colKey = cKey;

            // Create surface
            initializeSurface(cKey);
        }

        /// <summary>
        /// Constructor that takes a resource string i.e. path to the image file.
        /// </summary>
        /// <param name="Resource">Resource to take the bitmap from</param>
        /// <param name="ObjectType">Desired bitmap type</param>
        /// <param name="cKey">Desired Colorkey for Image. Will be applied only id BitmapType is equal to TRANSPARENT.</param>
        /// <param name="TargetDevice">DirectDraw target device</param>
        public DxImageObject(string Resource, BitmapType ObjectType, int cKey, Device TargetDevice)
        {
            // Create a new bitmap from the resource
            sourceBitmap = new Bitmap(Resource);

            // Save bitmap type and target device
            objectType = ObjectType;
            targetDevice = TargetDevice;

            // Create a rectangle with the bitmaps dimensions 
            objectSizeRect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);

            // Create a rectangle with the bitmaps position
            rectPosition = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);

            // Save width and height seperately
            iWidth = sourceBitmap.Width;
            iHeight = sourceBitmap.Height;

            // Build surface description
            initializeSurfaceDescription();

            // Create ColorKey object
            tempKey = new ColorKey();

            // Save Colorkey value
            colKey = cKey;

            // Create surface
            initializeSurface(cKey);
        }

        /// <summary>
        /// Sets up the surface description for the bitmap object
        /// </summary>
        protected void initializeSurfaceDescription()
        {
            // Create a description
            surfaceDesc = new SurfaceDescription();

            // Define the surface as offscreenplain
            surfaceDesc.SurfaceCaps.OffScreenPlain = true;

            // Set the dimensions of the surface
            surfaceDesc.Width = objectSizeRect.Width;
            surfaceDesc.Height = objectSizeRect.Height;
        }

        /// <summary>
        /// Creates the surface for this bitmap object
        /// </summary>
        protected void initializeSurface(int colorkey)
        {
            // Create surface using the bitmap, 
            // the surface description and device
            this.bitmapSurface = new Surface(sourceBitmap, surfaceDesc, targetDevice);

            // If the desired bitmap type is transparent,
            // set a color key
            switch (this.objectType)
            {
                case BitmapType.TRANSPARENT:

                    // Set Colorkey for transparency
                    tempKey.ColorSpaceLowValue = colorkey;
                    tempKey.ColorSpaceHighValue = colorkey;

                    // Assign the color key to the surface
                    bitmapSurface.SetColorKey(ColorKeyFlags.SourceDraw, tempKey);
                    break;
            }
        }

        /// <summary>
        /// Draws the bitmap object to the given surface
        /// </summary>
        /// <param name="TargetSurface">
        /// Target surface for drawing operation</param>
        /// <param name="TargetXPos">
        /// Desired horizontal position</param>
        /// <param name="TargetYPos">
        /// Desired vertical position</param>
        public void DrawBitmap(Surface TargetSurface, float TargetXPos, float TargetYPos)
        {
            // Save new position 
            this.XPosition = TargetXPos;
            this.YPosition = TargetYPos;

            // Call overriden method
            DrawBitmap(TargetSurface);
        }

        public void DrawBitmap(Surface TargetSurface, float TargetXPos, float TargetYPos, int sourceXRec, int sourceYRec, int sourceWid, int sourceHei)
        {
            this.XPosition = TargetXPos;
            this.YPosition = TargetYPos;

            if (!this.bVisible)
                return;


            Rectangle sourceRec = new Rectangle(sourceXRec, sourceYRec, sourceWid, sourceHei);
            if (this.objectType == BitmapType.TRANSPARENT)
                TargetSurface.DrawFast(rectPosition.X,
                                       rectPosition.Y,
                                       bitmapSurface,
                                       sourceRec,
                                       DrawFastFlags.SourceColorKey
                                       | DrawFastFlags.Wait);
            else
                TargetSurface.DrawFast(rectPosition.X,
                                       rectPosition.Y,
                                       bitmapSurface,
                                       sourceRec,
                                       DrawFastFlags.NoColorKey
                                       | DrawFastFlags.Wait);
        }
        /// <summary>
        /// Draws the bitmap object to the given surface
        /// </summary>
        /// <param name="TargetSurface">    
        /// Target surface for drawing operation</param>
        public void DrawBitmap(Surface TargetSurface)
        {
            // If object is not visible -> do nothing
            if (!this.bVisible)
                return;

            // Draw solid or transparent bitmap, 
            // depeding on the type
            if (this.objectType == BitmapType.TRANSPARENT)
                TargetSurface.DrawFast(rectPosition.X,
                                       rectPosition.Y,
                                       bitmapSurface,
                                       objectSizeRect,
                                       DrawFastFlags.SourceColorKey
                                       | DrawFastFlags.Wait);
            else
                TargetSurface.DrawFast(rectPosition.X,
                                       rectPosition.Y,
                                       bitmapSurface,
                                       objectSizeRect,
                                       DrawFastFlags.NoColorKey
                                       | DrawFastFlags.Wait);
        }

        /// <summary>
        /// This method has to be called on application
        /// switches. It restores the surface for this
        /// bitmap if it is lost.
        /// </summary>
        public void RestoreSurface()
        {
            // create surface description and surface
            this.initializeSurfaceDescription();
            this.initializeSurface(colKey);
        }
        public void Dispose()
        {
            this.bitmapSurface.Dispose();
            this.sourceBitmap.Dispose();
        }
    }
}
