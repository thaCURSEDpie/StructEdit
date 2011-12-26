

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class CInputParser
    {
        /// <summary>
        /// Parses an integer (can be hex or decimal) from a given string. Hexadecimal strings should begin with a '0x' prefix
        /// </summary>
        /// <param name="line">the string</param>
        /// <param name="foundInt">The found int.</param>
        /// <returns>
        /// On success: true. On failure: false;
        /// </returns>
        private static bool tryParseInteger(String line, ref int foundInt)
        {
            // Hex
            if (line.Contains("0x"))
            {
                line = line.Replace("0x", "");
                if (Int32.TryParse(line,
                               System.Globalization.NumberStyles.AllowHexSpecifier,
                               System.Globalization.CultureInfo.InvariantCulture,
                               out foundInt))
                {
                    return true;
                }
            }
            else
            {
                if (Int32.TryParse(line, out foundInt))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ignoreLine(String line)
        {            
            if (line == null || line.Length == 0 || line[0] == '#' || line[0] == ' ' || line[0] == '\n' || line[0] == '\0')
            {
                return true;
            }

            return false;
        }

        public static int CreateStructure(string filePath, ref CEditableStruct structure, ref int errorLine)
        {
            String structName = "";

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

            String line = "";

            int numLines = 0;
            int numRealLines = 0;

            char[] delimiter = new char[1];
            delimiter[0] = ' ';

            while (line != null)
            {
                line = sr.ReadLine();
                numLines++;

                if (!ignoreLine(line))
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

                        if (!tryParseInteger(elements[1], ref structSize))
                        {
                            errorLine = numLines;
                            // Error code 2: error parsing parameter 2 of file header (invalid integer).
                            sr.Close();
                            return 2;
                        }

                        if (!tryParseInteger(elements[2], ref structOffset))                            
                        {
                            errorLine = numLines;
                            // Error code 3: error parsing parameter 3 of file header (invalid integer).
                            sr.Close();
                            return 3;
                        }

                        if (!tryParseInteger(elements[3], ref numElements))
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

                        if (!tryParseInteger(elements[1], ref tempParam.Offset))
                        {
                            errorLine = numLines;

                            // Error code 6: error parsing parameter 1 of a parameter (invalid integer)
                            sr.Close();
                            return 6;
                        }

                        if (elements[2].Contains("CHAR["))
                        {
                            // Cleanup
                            elements[2] = elements[2].Replace("CHAR", "");
                            elements[2] = elements[2].Replace("[", "");
                            elements[2] = elements[2].Replace("]", "");
                            // Done

                            if (!Int32.TryParse(elements[2], out tempParam.StringSize))
                            {

                                // Error code 11: invalid parameter type 'CHAR[]': invalid string length specified.
                                sr.Close();
                                return 11;
                            }

                            tempParam.Type = typeof(String);
                        }
                        else
                        {
                            switch (elements[2])
                            {
                                case "INT":
                                    tempParam.Type = typeof(Int32);
                                    break;
                                case "FLOAT":
                                    tempParam.Type = typeof(float);
                                    break;
                                case "SHORT":
                                    tempParam.Type = typeof(Int16);
                                    break;
                                case "LONG":
                                    tempParam.Type = typeof(Int64);
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
    }
}
