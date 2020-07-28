using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWebApi.Hubs.Abstractions
{
    public interface IPanelHub
    {
        Task UpdateOffersListRealTime();

        Task UpdateProjectsListRealTime();
    }
}
