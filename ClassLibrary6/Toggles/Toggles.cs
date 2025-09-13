using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAxLM.Toggles
{
    public class Toggles
    {
        public static Toggles inst = new Toggles();
        //two toggles for non working mods that i am not doing again
        public bool infmon = false;
        public bool p_mon = false;
        //misc
        public bool spamhorn = false;
        //player
        public bool t_speed = false;
        public bool t_fastclimb = false;
        public bool t_fly = false;
        public bool t_infammo = false;
        public bool t_infstam = false;
        public bool t_infhealth = false;
        public bool t_infcharge = false;
        public bool t_chargeany = false;
        public bool t_nodark = false;
        public bool t_noweight = false;
        public bool t_strong = false;
        public bool t_fastheal = false;
        public bool t_infreach = false;
        public bool t_hearall = false;
        //idk
        public bool t_tvspz = false;
        public bool t_spazlights = false;
        //esps
        public bool t_esp = false;
        public bool t_tesp = false;
        public bool t_pesp = false;
        public bool t_ttesp = false;
        public bool t_desp = false;
        public bool t_lesp = false;
        //op
        public bool t_rigspam = false;
        //settings
        public bool t_time = false;
        //managers
        public bool w_moons = false;
        public bool w_explorer = false;
        public bool w_pexplorer = false;
        public bool w_items = false;
        public bool w_spawners = false;
        public bool w_players = false;
        public bool w_enemies = false;
        public bool w_ispawner = false;
        public bool w_landmine = false;
    }
}
