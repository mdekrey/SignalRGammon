using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    using ActionDispatcher = Func<CheckersAction, Task<bool>>;

    public class Rules
    {
        public static (CheckersState, bool) ApplyAction(CheckersState state, CheckersAction? action)
        {
            return (state, false);
        }

        public static async Task CheckAutomaticActions(CheckersState state, ActionDispatcher dispatch)
        {
            await Task.Yield();
        }
    }
}
