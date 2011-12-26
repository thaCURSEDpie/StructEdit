//=======================================================================
//
// <copyright file="GlobalParams.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           GlobalParams.cs
//          Version:        Alpha
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains static GlobalParams class,
//              which holds the global parameters for the project.
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class holds all the global parameters for the mod.
    /// </summary>
    public static class GlobalParams
    {
        /// <summary>
        /// The directory input files should be stored.
        /// </summary>
        public static String InputFilesDirectory = ".\\scripts\\structedit";

        /// <summary>
        /// The input file extension.
        /// </summary>
        public static String InputFilesExtension = ".sif";
    }
}
