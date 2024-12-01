using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;    // For Keyboard Device


namespace project_Egg
{
    class DxKeyboard
    {
        /// <summary>
		/// A device for the keyboard.
		/// </summary>
		protected Device keyboard = null;

		/// <summary>
		/// Constructor. Initializes a new device and sets necessary parameters.
		/// </summary>
		/// <param name="Owner">
		/// The control for which the input state shall be captured.</param>
		public DxKeyboard(Control Owner)
		{
			// Create a new Device with the keyboard guid
			keyboard = new Device(SystemGuid.Keyboard);

			// Set data format to keyboard data
			keyboard.SetDataFormat(DeviceDataFormat.Keyboard);

			// Set the cooperative level to foreground non-exclusive
			// and deactivate windows key
			keyboard.SetCooperativeLevel(Owner, 
										 CooperativeLevelFlags.Foreground | 
										 CooperativeLevelFlags.NonExclusive |
										 CooperativeLevelFlags.NoWindowsKey);

			// Try to access keyboard
			keyboard.Acquire();
		}

        /// <summary>
        /// Reads the keyboard state and reacquires access to
        /// the keyboard if it is lost.
        /// </summary>
        /// <returns>The current KeyboardState.</returns>
        public KeyboardState GetKeyboardState()
        {
	        // This will hold the current keyboard state
	        KeyboardState state = null;

            do
            {
                // Try to get the current state
                try 
                {
                    state = this.keyboard.GetCurrentKeyboardState();

                    // if fetching the state is successful -> exit loop
                    break;
                }
                catch (InputException)
                {          
                    // let the application handle Windows messages
                    Application.DoEvents();

                    // Try to get reacquire the keyboard 
                    // and don't care about exceptions
                    try
                    { 
                        keyboard.Acquire(); 
                    }
                    catch (InputLostException)
                    { 
                        continue; 
                    }
                    catch(OtherApplicationHasPriorityException)
                    { 
                        continue; 
                    }
                }
            }
            while(true); // Do this until it's successful
      
	    // return the retrieved keyboard state
	    return state;
    }

    }
}
