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
        [Description("Add quantity to product")]
        AddedMovemnt = 1,
        [Description("Internal Movment")]
        InternalMovment = 2,
        [Description("External Movment")]
        ExternalMovment = 3,
        [Description("Move To Store")]
        MoveToStore = 4,
        [Description("Move To Damaged")]
        MoveToDamaged = 5
    }
}
