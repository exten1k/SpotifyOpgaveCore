using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedopgaveSpotify.Controllers
{
    public class PlayerController : Controller
    {
        private static SpotifyWebAPI _spotify;

        // GET: Player
        public ActionResult Index()
        {
            return View();
        }
        public string GetDevice(string access_token)
        {
            string deviceID = "";
            _spotify = new SpotifyWebAPI()
            {
                //TODO Get token from session
                AccessToken = access_token,
                TokenType = "Bearer"
            };
            if (_spotify.AccessToken != null)
            {
                PlaybackContext context = _spotify.GetPlayback();
                if (context.Device != null)
                {
                    deviceID = context.Device.Id;

                }
            }
            return deviceID;
        }
        //public void Play(string access_token) {
        //    string device = GetDevice(access_token);

        //    ErrorResponse error = _spotify.ResumePlayback(device);
        //}
        public void Pause(string access_token)
        {
            string device = GetDevice(access_token);

            ErrorResponse error = _spotify.PausePlayback(device);

        }
        public void SkipToNext(string access_token)
        {
            string device = GetDevice(access_token);
            ErrorResponse error = _spotify.SkipPlaybackToNext(device);
        }
        public void SkipToPrev(string access_token)
        {
            string device = GetDevice(access_token);
            ErrorResponse error = _spotify.SkipPlaybackToPrevious(device);
        }
        public void VolumeControl(string access_token,int volume)
        {
            string device = GetDevice(access_token);
            ErrorResponse error = _spotify.SetVolume(volume,device);
        }

    }

}
