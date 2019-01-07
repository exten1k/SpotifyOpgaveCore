using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SpotifyOpgaveCore.Models;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Enums;

namespace SpotifyOpgaveCore.Controllers
{
    public class RoomsController : Controller
    {
        private static SpotifyWebAPI _spotify;
        private readonly RoomContext _context;

        public RoomsController(RoomContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .SingleOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            _spotify = new SpotifyWebAPI()
            {
                //TODO Get token from session
                AccessToken = await HttpContext.GetTokenAsync("Spotify", "access_token"),
                TokenType = "Bearer"
            };
            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,Owner,Name")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,Owner,Name")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .SingleOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }

        [HttpGet]
        public ActionResult Search(Room searchResult, string searchString, int id)
        {
            //TODO SearchQuery i stedet for string "Eminem"
            SearchItem item = _spotify.SearchItems(searchString, SearchType.Track);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult.FullTrack = item.Tracks.Items;
                //searchResult.RoomId = id;

            }
            return PartialView("searchResult", searchResult);
        }
        public void Play(string spotifyUri)
        {
            //spotifyUri = "60SdxE8apGAxMiRrpbmLY0";
            ErrorResponse error = _spotify.ResumePlayback(uris: new List<string> { spotifyUri });
        }

        public ActionResult PlayingContext(Room room, string spotifyUri, int id = 5)
        {
            room.PlaybackContext = _spotify.GetPlayingTrack();
            //room.RoomId = id;
            if (room.PlaybackContext.Item != null)
            {
                ViewBag.song = room.PlaybackContext.Item.Artists[0].Name + " - " + room.PlaybackContext.Item.Name;
                double dProgress = ((double)room.PlaybackContext.ProgressMs / room.PlaybackContext.Item.DurationMs) * 100.0;
                int currentProgress = Convert.ToInt32(dProgress);
                ViewBag.progress = currentProgress;
                if (room.PlaybackContext.IsPlaying == false && room.PlaybackContext.ProgressMs == 0)
                {
                    spotifyUri = _context.Songs.FirstOrDefault(x => x.RoomId == id).SongID;
                    Play(spotifyUri);
                }
                //var songs = _context.Songs.FirstOrDefault(x => x.RoomId == id).SongID;


            }
            return PartialView("playingContext", room);

        }
        public async Task CreateSong(Room room, string spotifyUri, string songName, int roomID)
        {
            if (ModelState.IsValid)
            {
                Song song = new Song();
                song.RoomId = roomID;
                song.Name = songName;
                song.SongID = spotifyUri;
                _context.Add(song);
                await _context.SaveChangesAsync();


            }
        }
    }
}
