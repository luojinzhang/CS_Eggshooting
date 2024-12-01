using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX.AudioVideoPlayback;

namespace project_Egg
{
    class DxSound
    {
        //Audio Playback class
        protected Audio music;
        //String of the musicpath
        protected string musicpath = "Resources\\";
        // file name
        protected string fileName;
        //The playlist initialized later
        private string[] m_playlist = null;
        //The maxindex, store the upper bounds of the playlist
        private int maxindex = 0;
        //Current index of the music played
        private int musicindex = 0;
        public DxSound(string fileName)
        {
            this.fileName = musicpath + fileName;
            music = new Audio(this.fileName);
        }
        private string[] FillPlaylist()
        {
            // Create the playlist
            string[] playlist;
            // Fill the playlist
            playlist = Directory.GetFiles(musicpath, "*.wav");
            // Get the number of song
            maxindex = playlist.Length;
            // Return
            return playlist;
        }
        private void InitAudioPlayback()
        {
            // Create the Audio class with the first element of the playlist
            music = new Audio(m_playlist[0]);
            // Event to handle the ending of the music
            music.Ending += new EventHandler(this.OnMusicEnding);
            // Play the music
            music.Play();
        }
        private void OnMusicEnding(object sender, System.EventArgs e)
        {
            // Check if the Audio class is created
            if (music != null)
            {
                // Increment the music idnex
                musicindex++;
                // Stop the current song played
                music.Stop();
                // If this is the song, return to the first song
                if (musicindex > maxindex - 1)
                {
                    musicindex = 0;
                }

                try
                {
                    // Open the next song
                    music.Open(m_playlist[musicindex]);
                    // Play it
                    music.Play();

                }
                catch (Exception)
                {
                    // If they are any problems, exit the application
                    //this.Close();
                }
            }
        }
        public void Play()
        {
            this.music.Open(this.fileName);
            this.music.Play();
        }
    }
}
