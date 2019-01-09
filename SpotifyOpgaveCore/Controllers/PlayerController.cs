using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyOpgaveCore.Controllers
{
    public class PlayerController : Controller
    {
        private static SpotifyWebAPI _spotify;

        // GET: Player
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]

        public async Task<string> GetDeviceAsync()
        {
            string deviceID = "";
            _spotify = new SpotifyWebAPI()
            {
                //TODO Get token from session
                AccessToken = await HttpContext.GetTokenAsync("Spotify", "access_token"),
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
        [Authorize]

        public async void Play()
        {
            string device = await GetDeviceAsync();

            ErrorResponse error = _spotify.ResumePlayback(device);
        }
        [Authorize]

        public async void Pause()
        {
            string device = await GetDeviceAsync();

            ErrorResponse error = _spotify.PausePlayback(device);

        }
        [Authorize]

        public async void SkipToNext()
        {
            string device = await GetDeviceAsync();
            ErrorResponse error = _spotify.SkipPlaybackToNext(device);
        }
        [Authorize]

        public async void SkipToPrev()
        {
            string device = await GetDeviceAsync();
            ErrorResponse error = _spotify.SkipPlaybackToPrevious(device);
        }
        [Authorize]

        public async void VolumeControl(int volume)
        {
            string device = await GetDeviceAsync();
            ErrorResponse error = _spotify.SetVolume(volume, device);
        }

    }

}
