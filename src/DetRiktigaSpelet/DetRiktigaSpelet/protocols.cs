using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DetRiktigaSpelet
{
    public enum Protocol
    {
        Disconnected = 0,
        Connected = 1,
        PlayerMoved = 2,
        PlayerRotated = 3,
        BulletCreated = 4,
        GetID = 5,
        Sekundarcreated = 6,
        PlayerChange = 7,
        StateChange = 8,
        Animeradframe = 9,
        Animeradframediagonal = 10,
        Killstowinchanged = 11,
        Timeforroundchanged = 12,
        PlayerReady = 13,
        PlayerStoped = 14
    }
}
