using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Enums
{
    public enum PositionEnum
    {
        [Description("Quarter Back")]
        QB = 1,
        [Description("Running Back")]
        RB = 2,
        [Description("Wide Receiver")]
        WR = 3,
        [Description("Tight End")]
        TE = 4
    }
}
