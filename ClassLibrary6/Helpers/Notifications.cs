using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary6.Helpers
{
    public class Notifications
    {
        //one script one void
        public static void Noti(string title, string whattosay)
        {
            if (title != "") {
                HUDManager.Instance.DisplayTip(title, whattosay);
            }
            else {
                HUDManager.Instance.DisplayTip("PAxLM", whattosay);
            }
        }
    }
}
