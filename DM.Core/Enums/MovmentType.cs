using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Enums
{
    public enum MovmentType
    {
        [Description("Add quantity")]
        AddedMovemnt = 1,
        [Description("Transfering to onther shelf")]
        InternalMovment = 2,
        [Description("Moving from wearhouse to another")]
        ExternalMovment = 3,
        [Description("Move To Store")]
        MoveToStore = 4,
        [Description("Move To Damaged section")]
        MoveToDamaged = 5,
        [Description("Deleted quantity")]
        DeletedMovemnt = 6
    }
}
