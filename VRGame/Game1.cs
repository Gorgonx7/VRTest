using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRGame
{
    class Game1
    {
        static void Main()
        {
            using (Game game = new Game()) {
                game.Run();
            }
        }
    }
}
