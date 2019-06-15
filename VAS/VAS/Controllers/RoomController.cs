using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VAS.Model;
using VAS.Service;
using VAS.Utils;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        #region properties
        private readonly IRoomService _roomService;
        private readonly UserManager<MyUser> _userManager;

        public RoomController(IRoomService roomService, UserManager<MyUser> userManager)
        {
            _roomService = roomService;
            _userManager = userManager;
        }
        #endregion

        [HttpGet]
        public ActionResult LoadRooms(string code, string name, Guid specialityId, string number, int index = 1, int pageSize = 5)
        {
            code = code == null ? "" : code;
            name = name == null ? "" : name;
            number = number == null ? "" : number;
            var rooms = _roomService.GetRooms(r => r.Name.Contains(name)
                                                    && r.HisCode.Contains(code)
                                                    && r.Number.Contains(number)
                                                    //&& r.IsAvailable==true
                                                    );
            if (specialityId != Guid.Empty)
            {
                rooms = rooms.Where(_ => _.SpecialityId == specialityId);
            }
            var result = rooms.OrderByDescending(_ => _.DateCreated)
                    .ToPageList<RoomViewModel, Room>(index, pageSize);
            return Ok(result);
        }

        /// Lấy room theo id
        [HttpGet("{id}")]
        public ActionResult LoadRoomById(Guid id)
        {
            var room = _roomService.GetRoom(id);
            if (room == null) return NotFound();
            return Ok(room.Adapt<RoomViewModel>());
        }

        [HttpGet("code/{hiscode}")]
        public ActionResult CheckHisCode(string hiscode)
        {
            var room = _roomService.GetRooms(_=>_.HisCode == hiscode).FirstOrDefault();
            if (room == null) return NotFound();
            return Ok();
        }



        [HttpPost]
        public async Task<ActionResult> CreateRoomAsync([FromBody]RoomCreateModel room)
        {
            try
            {
                _roomService.CreateRoom(room.Adapt<Room>(), (await _userManager.GetUserAsync(User)).UserName);
                _roomService.Save();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return StatusCode(201);
        }

        [HttpPost("List")]
        public async Task<ActionResult> CreateRoomsAsync([FromBody]List<RoomCreateModel> roomCMs)
        {
            try
            {
                var rooms = roomCMs.Adapt<List<Room>>();
                foreach (var room in rooms)
                {
                    room.IsAvailable = true;
                }
                _roomService.CreateRooms(rooms, (await _userManager.GetUserAsync(User)).UserName);
                _roomService.Save();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRoomAsync([FromBody]RoomUpdateModel model)
        {
            try
            {
                var room = _roomService.GetRoom(model.Id);
                if (room == null) return NotFound();
                room = model.Adapt(room);
                _roomService.Save();
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/ChangeActive")]
        public async Task<ActionResult> ChangeActive(Guid id)
        {
            try
            {
                var room = _roomService.GetRoom(id);
                if (room == null) return NotFound();
                room.IsAvailable = !room.IsAvailable;
                _roomService.Save();
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}