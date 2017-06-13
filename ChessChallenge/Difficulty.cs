/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Enueration for Difficulty
 * The player can play on different difficulty levels against the AI
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    public enum Difficulty
    {
        // set to 0
        None,
        // set to 1
        Easy,
        // set to 2
        Normal,
        // set to 3
        Hard,
        // set to 4
        Grandmaster
    }
}
