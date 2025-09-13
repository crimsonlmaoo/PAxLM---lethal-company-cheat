using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAxLM.Helpers
{
    public class Notifications
    {
        //one script one void
        public static void Noti(string whattosay)
        {
            HUDManager.Instance.DisplayTip("PAxLM", whattosay);
        }
    }
}
