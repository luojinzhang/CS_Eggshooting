using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace project_Egg
{
    public class DxTimer
    {
        #region Imported functions from Kernel32.dll
        /*
         The functions we need to access are Query PerformanceFrequency 
         QueryPerformanceCounter which are located in "kernel32.dll". They are not directly
         available to .NET, so we have to import them. To do this, we'll use the so called 
         InteropServices. Add the following using statement at the beginning of the new 
         class file: 
        */

        /// <summary>
        /// This function retrieves the system's performance 
        /// counter frequency.
        /// </summary>
        [DllImport("kernel32")]
        private static extern bool QueryPerformanceFrequency(ref long Frequency);

        /// <summary>
        /// This function retrieves the current number of
        /// ticks for this system.
        /// </summary>
        [DllImport("kernel32")]
        private static extern bool QueryPerformanceCounter(ref long Count);

        #endregion

        #region Will be used for the Instantiated Timer Object
        private long LastTime = 0;       // The last number of ticks.
        private long CurrentTime = 0;    // The current number of ticks.
        private bool bTimerStart = true; // Used in the markTime() method
        #endregion

        #region Will be used for the Un-Instantiated Timer Object
        private static long lLastTime = 0;       // The last number of ticks.
        private static long lCurrentTime = 0;    // The current number of ticks.
        #endregion

        private static long lTicksPerSecond = 0; // The number of ticks per second for this system.
        private static bool bInitialized = false;// Indicates if the Timer is initialized

        // The elapsed seconds since the last GetElapsedSeconds() call.
        private static double dElapsedSeconds = 0.0;

        // The elapsed milliseconds since the last GetElapsedMilliseconds() call.
        private static double dElapsedMilliseconds = 0.0;

        /// <summary>
        /// Property to query the ticks per second
        /// for this system (Timer has to be initialized).
        /// </summary>
        public static long TicksPerSecond
        {
            get
            {
                return lTicksPerSecond;
            }
        }

        /// <summary>
        /// The initialization of the timer. Tries to query
        /// performance frequency and determines if performance
        /// counters are supported by this system.
        /// </summary>
        public static void Init()
        {
            // Try to read frequency. If this fails, performance
            // counters are not supported.
            if (!QueryPerformanceFrequency(ref lTicksPerSecond))
            {
                throw new Exception("Performance Counter not supported on this system!");
            }

            // Initialization successful
            bInitialized = true;
        }

        /// <summary>
        /// Starts the Timer. This set the initial time value.
        /// Timer has to be initialized for this.
        /// </summary>
        public static void Start()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            // Initialize time value
            QueryPerformanceCounter(ref lLastTime);
        }

        /// <summary>
        /// Gets the elapsed milliseconds since the last
        /// call to this function. Timer has to be initialized
        /// for this.
        /// </summary>
        /// <returns>The number of milliseconds.</returns>
        public static double GetElapsedMilliseconds()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            // Get current number of ticks
            QueryPerformanceCounter(ref lCurrentTime);

            // Calculate number of milliseconds since last call
            dElapsedMilliseconds = ((double)(lCurrentTime - lLastTime) / (double)lTicksPerSecond) * 1000.0;

            // Store current number of ticks for next call
            lLastTime = lCurrentTime;

            // Return milliseconds
            return dElapsedMilliseconds;
        }


        /// <summary>
        /// Gets the elapsed seconds since the last call
        /// to this function. Timer has to be initialized for this.
        /// </summary>
        /// <returns>The number of seconds.</returns>
        public static double GetElapsedSeconds()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            // Get current number of ticks
            QueryPerformanceCounter(ref lCurrentTime);

            // Calculate elapsed seconds
            dElapsedSeconds = (double)(lCurrentTime - lLastTime) / (double)lTicksPerSecond;

            // Store current number of ticks for next call
            lLastTime = lCurrentTime;

            // Return number of seconds
            return dElapsedSeconds;
        }

        /// <summary>
        /// Saves the entry point ticks for time calculation later on.
        /// Timer has to be initialized for this.
        /// </summary>
        public void markTime()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            if (bTimerStart)
            {
                // Initialize time value
                QueryPerformanceCounter(ref LastTime);
                bTimerStart = false;
            }
        }

        /// <summary>
        /// Returns the number of milli seconds elapsed since
        /// markTime() method was called.
        /// </summary>

        public double msElapsed()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            // Get current number of ticks
            QueryPerformanceCounter(ref CurrentTime);

            // Calculate number of milliseconds since last call
            dElapsedMilliseconds = ((double)(CurrentTime - LastTime) / (double)lTicksPerSecond) * 1000.0;

            // Return milliseconds
            return dElapsedMilliseconds;
        }

        /// <summary>
        /// Returns the number of seconds elapsed since markTime()
        /// method was called.
        /// </summary>
        public double secElapsed()
        {
            // Check if initialized
            if (!bInitialized)
            {
                throw new Exception("Timer not initialized!");
            }

            // Get current number of ticks
            QueryPerformanceCounter(ref CurrentTime);

            // Calculate elapsed seconds
            dElapsedSeconds = (double)(CurrentTime - LastTime) / (double)lTicksPerSecond;

            // Return number of seconds
            return dElapsedSeconds;
        }


        /// <summary>
        /// Resets the timer so that the markTime() method can remark a new time
        /// </summary>
        public void resetTime()
        {
            bTimerStart = true;
        }

    }
}
