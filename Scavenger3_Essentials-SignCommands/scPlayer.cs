﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Scavenger3_Essentials_SignCommands
{
    public class scPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public Dictionary<string, DateTime> Cooldowns { get; set; }
        public bool DestroyMode { get; set; }
        public int AlertCooldownCooldown { get; set; }
        public int AlertPermissionCooldown { get; set; }
        public int AlertDestroyCooldown { get; set; }

        public scPlayer(int index)
        {
            this.Index = index;
            this.Cooldowns = new Dictionary<string, DateTime>();
            this.DestroyMode = false;
            this.AlertDestroyCooldown = 0;
            this.AlertPermissionCooldown = 0;
            this.AlertCooldownCooldown = 0;
        }
    }
}
