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
using SpotifyOpgaveCore.Extensions;

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

            ICollection<Song> songs = _context.Songs.Where(x => x.RoomId == id).ToList();
            ICollection<Vote> votes = _context.Votes.Where(x => x.Song.RoomId == id).ToList();
            var voteValue = songs.Select(x => x.Votes.Select(f => f.Value));

            _spotify = new SpotifyWebAPI()
            {
                //TODO Get token from session
                AccessToken = await HttpContext.GetTokenAsync("Spotify", "access_token"),
                TokenType = "Bearer",
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

        [HttpPost]
        public async Task<ActionResult> Search(string searchString, int? id)
        {
            var room = await _context.Rooms
                           .SingleOrDefaultAsync(m => m.RoomId == id);            //TODO SearchQuery i stedet for string "Eminem"
            SearchItem item = _spotify.SearchItems(searchString, SearchType.Track);

            if (!string.IsNullOrEmpty(searchString))
            {
                room.FullTrack = item.Tracks.Items;
                //searchResult.RoomId = id;

            }
            return PartialView("Search", room);
        }
        public async Task Play(string spotifyUri, int id)
        {
            var song = _context.Songs.FirstOrDefault(x => x.RoomId == id);
            spotifyUri = song.SpotifyUri;
            ErrorResponse error = _spotify.ResumePlayback(uris: new List<string> { spotifyUri });
            _context.Remove(song);
            await _context.SaveChangesAsync();

        }

        public async Task<ActionResult> PlayingContext(string spotifyUri, int id)
        {
            var room = await _context.Rooms
                          .SingleOrDefaultAsync(m => m.RoomId == id);
            room.PlaybackContext = _spotify.GetPlayingTrack();
            if (room.PlaybackContext.Item != null)
            {
                ViewBag.song = room.PlaybackContext.Item.Artists[0].Name + " - " + room.PlaybackContext.Item.Name;
                double dProgress = ((double)room.PlaybackContext.ProgressMs / room.PlaybackContext.Item.DurationMs) * 100.0;
                int currentProgress = Convert.ToInt32(dProgress);
                ViewBag.progress = currentProgress;
                if (room.PlaybackContext.IsPlaying == false && room.PlaybackContext.ProgressMs == 0)
                {
                    await Play(spotifyUri, id);
                }
            }

            return PartialView("playingContext", room);

        }
        public async Task<IActionResult> CreateSong(string spotifyUri, string songName, int id)
        {
            var room = await _context.Rooms
                          .SingleOrDefaultAsync(m => m.RoomId == id);
            if (ModelState.IsValid)
            {
                if (_context.Songs.Any(o => o.SpotifyUri == spotifyUri && o.RoomId == id))
                {

                }
                else
                {
                    Song song = new Song();
                    song.RoomId = id;
                    song.Name = songName;
                    song.SpotifyUri = spotifyUri;
                    _context.Add(song);
                    await _context.SaveChangesAsync();
                }
            }
            ICollection<Song> songs = _context.Songs.Where(x => x.RoomId == id).ToList();
            var vote = _context.Votes.Where(x => x.SongID == id).Select(x=>x.Value);

            return PartialView("CreateSong", room);
        }
        public async Task<ActionResult> Upvote(int songId, int id)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);
            ICollection<Song> songs = _context.Songs.Where(x => x.RoomId == id).ToList();
            ICollection<Vote> votes = _context.Votes.Where(x => x.Song.RoomId == id).ToList();
            //var lastVote = songs.OrderByDescending(x => x.Votes.Select(m => m.VoteId)).Select(f => f.Votes.Select(k => k.Value).FirstOrDefault());
            int lastVote = votes.Where(x => x.SongID == songId).OrderByDescending(f => f.VoteId).Select(m => m.Value).FirstOrDefault();

            int newVote = lastVote + 1;
            {
                if (_context.Votes.Any(o => o.SongID == songId && o.UserId == User.Identity.Name))
                {

                }
                else
                {
                    Vote vote = new Vote();
                    vote.SongID = songId;
                    vote.Value = newVote;
                    vote.UserId = User.Identity.Name;
                    _context.Add(vote);
                    await _context.SaveChangesAsync();
                }
                    return PartialView("CreateSong", room);   
            }
        }
        public async Task<ActionResult> Downvote(int songId, int id)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);
            ICollection<Song> songs = _context.Songs.Where(x => x.RoomId == id).ToList();
            ICollection<Vote> votes = _context.Votes.Where(x => x.Song.RoomId == id).ToList();
            //var lastVote = songs.OrderByDescending(x => x.Votes.Select(m => m.VoteId)).Select(f => f.Votes.Select(k => k.Value).FirstOrDefault());
            int lastVote = votes.Where(x => x.SongID == songId).OrderByDescending(f => f.VoteId).Select(m => m.Value).FirstOrDefault();

            int newVote = lastVote - 1;
            {
                if (_context.Votes.Any(o => o.SongID == songId && o.UserId == User.Identity.Name))
                {

                }
                else
                {
                    Vote vote = new Vote();
                    vote.SongID = songId;
                    vote.Value = newVote;
                    vote.UserId = User.Identity.Name;
                    _context.Add(vote);
                    await _context.SaveChangesAsync();
                }
                return PartialView("CreateSong", room);
            }
        }
    }
}