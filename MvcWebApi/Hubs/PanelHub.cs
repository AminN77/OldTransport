using Microsoft.AspNetCore.SignalR;
using MvcWebApi.Hubs.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWebApi.Hubs
{
    public class PanelHub : Hub, IPanelHub
    {
        public async Task UpdateProjectsListRealTime()
        {
            await Clients.All.SendAsync("UpdateProjectsList");
        }

        public async Task UpdateOffersListRealTime()
        {
            await Clients.All.SendAsync("UpdateOffersList");
        }


    }
}
