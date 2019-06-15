using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VAS.Model;
using VAS.Service;

namespace VAS.Hubs
{
    //[Authorize]
    public class CenterHub : Hub
    {
        private IHttpContextAccessor _contextAccessor;
        private readonly ISignalRConnectionService _connectionService;
        private readonly UserManager<MyUser> _userManager;

        private HttpContext _context { get { return _contextAccessor.HttpContext; } }

        public CenterHub(IHttpContextAccessor contextAccessor, ISignalRConnectionService connectionService, UserManager<MyUser> userManager)
        {
            _contextAccessor = contextAccessor;
            _connectionService = connectionService;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var _user = _userManager.GetUserAsync(_context.User).Result;
            if (_user != null)
            {
                var connectionId = Context.ConnectionId;
                _connectionService.CreateConnection(new SignalRConnection
                {
                    ConnectionId = connectionId,
                    Username = _user.UserName,
                    DateCreated = DateTime.Now
                });
                _connectionService.Save();
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = _context.User;
            _connectionService.DeleteConnection(_ => _.ConnectionId.Equals(Context.ConnectionId));
            _connectionService.Save();
            await base.OnDisconnectedAsync(exception);
        }


    }
}
