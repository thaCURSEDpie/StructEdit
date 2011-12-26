

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A struct representing a parameter in-memory
    /// </summary>
    public struct SParameter
    {
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type Type;

        /// <summary>
        /// The minimum value the parameter should have. 0xCAFED00D represents no minimum value.
        /// </summary>
        public float MinVal;

        /// <summary>
        /// The parameter's name.
        /// </summary>
        public String ParamName;

        /// <summary>
        /// The maximum value the paramter should have. 0xCAFED00D represents no maximum value.
        /// </summary>
        public float MaxVal;

        /// <summary>
        /// The offset from the main-structure start.
        /// </summary>
        public int Offset;

        /// <summary>
        /// The size of the string, if that is the type.
        /// </summary>
        public int StringSize;
    };
}
