using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.ArenaSystem
{
    [PropertyObject]
    public class ArenaTeam
    {
        public Dictionary<PlayerMobile, PlayerStatsEntry> Players { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Count { get { return Players == null ? 0 : Players.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Unoccupied { get { return Count == 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile PlayerZero { get; set; }

        public ArenaTeam()
        {
            Players = new Dictionary<PlayerMobile, PlayerStatsEntry>();
        }

        public ArenaTeam(PlayerMobile pm)
        {
            Players = new Dictionary<PlayerMobile, PlayerStatsEntry>();
            AddParticipant(pm);
        }

        public void AddParticipant(PlayerMobile pm)
        {
            if (Players.Count == 0)
                PlayerZero = pm;

            Players[pm] = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(pm);
        }

        public bool RemoveParticipant(PlayerMobile pm)
        {
            if (Players == null)
                return false;

            if (Players.ContainsKey(pm))
            {
                Players.Remove(pm);
                return true;
            }

            return false;
        }

        public bool Contains(PlayerMobile pm)
        {
            return Players.ContainsKey(pm);
        }
    }
}