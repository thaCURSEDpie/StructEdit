//=======================================================================
//
// <copyright file="GlobalVars.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           GlobalVars.cs
//          Version:        Alpha
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains the static GlobalVars class, which
//              contains all the global variables.
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class hold all the global variables for the mod.
    /// </summary>
    public static class GlobalVars
    {
        /// <summary>
        /// A list holding all the CEditableStructs
        /// </summary>
        public static List<CEditableStruct> Structures;

        /// <summary>
        /// An int representing the game's base memory-address.
        /// </summary>
        public static int BaseAddress;
    }
}
