//=======================================================================
//
// <copyright file="CInputParser.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           CInputParser.cs
//          Version:        Beta
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains the CInutParser file, which parses
//              input files and creates CEditableStructs from them.
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class parses the input files (*.sif) and creates CEditableStructs from them.
    /// </summary>
    public static class CInputParser
    {
        /// <summary>
        /// Creates a new CEditableStruct from an input file.
        /// </summary>
        /// <param name="filePath">The file path to the input file.</param>
        /// <param name="structure">The structure to create.</param>
        /// <param name="errorLine">The error line. When an error has occured this value will be adjusted accordingly.</param>
        /// <returns>0: success. 
        /// 1: invalid number of elements in file header. 
        /// 2: error parsing element 2 of file header (invalid integer). 
        /// 3: error parsing element 3 of file header (invalid integer).
        /// 4: error parsing element 4 of file header (invalid integer).
        /// 5: invalid number of elements in parameter-describing line.
        /// 6: error parsing element 1 of parameter-line (invalid integer).
        /// 7: invalid parameter type
        /// 8: invalid float value for parameter.
        /// 9: invalid 4th element (invalid float)
        /// 10: failed to open input file
        /// 11: invalid parameter type 'char[]': invalid string length specified.</returns>
        public static int CreateStructure(string filePath, ref CEditableStruct structure, ref int errorLine)
        {
            string structName = string.Empty;

            int structSize = 0, structOffset = 0, numElements = 0;
            List<SParameter> parameters = new List<SParameter>();

            System.IO.StreamReader sr;
            try
            {
                sr = new StreamReader(filePath);
            }
            catch (SystemException se)
            {
                // Failed to open the file! :O Error code 10
                return 10;
            }

            string line = string.Empty;

            int numLines = 0;
            int numRealLines = 0;

            char[] delimiter = new char[1];
            delimiter[0] = ' ';

            while (line != null)
            {
                line = sr.ReadLine();
                numLines++;

                if (!IgnoreLine(line))
                {
                    numRealLines++;

                    // We have the header, hopefully :)
                    if (numRealLines == 1)
                    {
                        string[] elements = line.Split(delimiter, StringSplitOptions.None);

                        if (elements.Length != 4)
                        {
                            // We are expecting 4 elements
                            errorLine = numLines;

                            // Error code 1: invalid number of elements in file header.
                            sr.Close();
                            return 1;
                        }

                        structName = elements[0];

                        // If integer parsing failed...

                        if (!TryParseInteger(elements[1], ref structSize))
                        {
                            errorLine = numLines;
                            // Error code 2: error parsing parameter 2 of file header (invalid integer).
                            sr.Close();
                            return 2;
                        }

                        if (!TryParseInteger(elements[2], ref structOffset))
                        {
                            errorLine = numLines;
                            // Error code 3: error parsing parameter 3 of file header (invalid integer).
                            sr.Close();
                            return 3;
                        }

                        if (!TryParseInteger(elements[3], ref numElements))
                        {
                            errorLine = numLines;
                            // Error code 4: error parsing parameter 4 of file header (invalid integer).
                            sr.Close();
                            return 4;
                        }
                    }
                    else
                    {
                        // We are loading in parameters :)
                        SParameter tempParam = new SParameter();

                        string[] elements = line.Split(delimiter, StringSplitOptions.None);

                        if (elements.Length != 5)
                        {
                            // We are expecting 5 elements
                            errorLine = numLines;

                            // Error code 5: invalid number of elements in parameter line.
                            sr.Close();
                            return 5;
                        }

                        tempParam.ParamName = elements[0];

                        if (!TryParseInteger(elements[1], ref tempParam.Offset))
                        {
                            errorLine = numLines;

                            // Error code 6: error parsing parameter 1 of a parameter (invalid integer)
                            sr.Close();
                            return 6;
                        }

                        if (elements[2].Contains("CHAR["))
                        {
                            // Cleanup
                            elements[2] = elements[2].Replace("CHAR", string.Empty);
                            elements[2] = elements[2].Replace("[", string.Empty);
                            elements[2] = elements[2].Replace("]", string.Empty);
                            // Done

                            if (!int.TryParse(elements[2], out tempParam.StringSize))
                            {

                                // Error code 11: invalid parameter type 'CHAR[]': invalid string length specified.
                                sr.Close();
                                return 11;
                            }

                            tempParam.Type = typeof(string);
                        }
                        else
                        {
                            switch (elements[2])
                            {
                                case "INT":
                                    tempParam.Type = typeof(int);
                                    break;
                                case "UINT":
                                    tempParam.Type = typeof(uint);
                                    break;
                                case "FLOAT":
                                    tempParam.Type = typeof(float);
                                    break;
                                case "SHORT":
                                    tempParam.Type = typeof(short);
                                    break;
                                case "LONG":
                                    tempParam.Type = typeof(long);
                                    break;
                                case "DOUBLE":
                                    tempParam.Type = typeof(double);
                                    break;
                                case "CHAR":
                                    tempParam.Type = typeof(char);
                                    tempParam.StringSize = 1;
                                    break;
                                default:
                                    errorLine = numLines;

                                    // Error code 7: invalid parameter type
                                    sr.Close();
                                    return 7;
                            }

                            tempParam.StringSize = 0;
                        }

                        if (elements[3] == "NONE")
                        {
                            tempParam.MinVal = 0xCAFED00D;
                        }
                        else
                        {
                            if (!float.TryParse(elements[3], out tempParam.MinVal))
                            {
                                errorLine = numLines;

                                // Error code 8: invalid parameter 3 (invalid float)
                                sr.Close();
                                return 8;
                            }
                        }

                        if (elements[4] == "NONE")
                        {
                            tempParam.MaxVal = 0xCAFED00D;
                        }
                        else
                        {
                            if (!float.TryParse(elements[3], out tempParam.MaxVal))
                            {
                                errorLine = numLines;

                                // Error code 8: invalid parameter 4 (invalid float)
                                sr.Close();
                                return 9;
                            }
                        }

                        parameters.Add(tempParam);
                    }
                }
            }

            structure = new CEditableStruct(structName, GlobalVars.BaseAddress, structOffset, structSize, numElements, parameters);
            sr.Close();
            return 0;
        }

        /// <summary>
        /// Parses an integer (can be hex or decimal) from a given string. Hexadecimal strings should begin with a '0x' prefix
        /// </summary>
        /// <param name="line">the string</param>
        /// <param name="foundInt">The found int.</param>
        /// <returns>
        /// On success: true. On failure: false;
        /// </returns>
        private static bool TryParseInteger(string line, ref int foundInt)
        {
            // Hex
            if (line.Contains("0x"))
            {
                line = line.Replace("0x", string.Empty);
                if (int.TryParse(
                                   line,
                                   System.Globalization.NumberStyles.AllowHexSpecifier,
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   out foundInt))
                {
                    return true;
                }
            }
            else
            {
                if (int.TryParse(line, out foundInt))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Decides whether a line should be ignored or not.
        /// </summary>
        /// <param name="line">The line to be analyzed.</param>
        /// <returns>A value indicating whether the line should be ignored or not.</returns>
        private static bool IgnoreLine(string line)
        {            
            if (line == null || line.Length == 0 || line[0] == '#' || line[0] == ' ' || line[0] == '\n' || line[0] == '\0')
            {
                return true;
            }

            return false;
        }
    }
}
