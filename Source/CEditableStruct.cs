//=======================================================================
//
// <copyright file="CEditableStruct.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           CEditableStruct.cs
//          Version:        Alpha
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains the CEditableStruct class,
//              which provides the value editing / viewing functionality.
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A class which handles the editing and viewing of in-memory structures.
    /// </summary>
    public unsafe class CEditableStruct
    {
        /// <summary>
        /// The base address of the application.
        /// </summary>
        private int _appBaseAddr;

        /// <summary>
        /// The array's offset from the base address.
        /// </summary>
        private int _arrayOffset;

        /// <summary>
        /// The absolute start address of the array.
        /// </summary>
        private int _arrayStartAddress;

        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        private int _numElements;

        /// <summary>
        /// The size of this structure in-memory. (in bytes)
        /// </summary>
        private int _size;

        /// <summary>
        /// The number of parameters this structures has in-memory.
        /// </summary>
        private int _numParams;

        /// <summary>
        /// This structure's name.
        /// </summary>
        private string _name;

        /// <summary>
        /// A list containing the structure's parameters.
        /// </summary>
        private List<SParameter> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CEditableStruct"/> class.
        /// </summary>
        /// <param name="name">The structure name.</param>
        /// <param name="appBaseAddr">The application base address.</param>
        /// <param name="arrayOffset">The offset from the baseaddress.</param>
        /// <param name="size">The structure size in-memory.</param>
        /// <param name="numElements">The number elements in the array.</param>
        /// <param name="parameters">A list containing the structure's parameters.</param>
        public CEditableStruct(string name, int appBaseAddr, int arrayOffset, int size, int numElements, List<SParameter> parameters)
        {
            this._name = name;
            this._appBaseAddr = appBaseAddr;            
            this._arrayOffset = arrayOffset;
            this._size = size;
            this._arrayStartAddress = appBaseAddr + arrayOffset;

            this._numElements = numElements;
            this._numParams = parameters.Count;
            this._parameters = parameters;
        }

        /// <summary>
        /// Gets the start address of this structure's array.
        /// </summary>
        public int ArrayStartAddress
        {
            get
            {
                return this._arrayStartAddress;
            }
        }

        /// <summary>
        /// Gets the array offset.
        /// </summary>
        public int ArrayOffset
        {
            get
            {
                return this._arrayOffset;
            }
        }

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public int NumParams
        {
            get
            {
                return this._numParams;
            }
        }

        /// <summary>
        /// Gets the number of elements in the array.
        /// </summary>
        public int NumElements
        {
            get
            {
                return this._numElements;
            }
        }

        /// <summary>
        /// Gets the structure name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// Gets the size of this structure in-memory.
        /// </summary>
        public int Size
        {
            get
            {
                return this._size;
            }
        }

        /// <summary>
        /// Gets the index of a generic parameter within the structure
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramIndex">Index of the parameter.</param>
        /// <returns>On success: 0. On failure: 1. Note: on failure paramIndex is set to -1.</returns>
        public int GetGenericParamIndex(string paramName, out int paramIndex)
        {
            for (int i = 0; i < this._parameters.Count; i++)
            {
                if (this._parameters[i].ParamName == paramName)
                {
                    paramIndex = i;
                    return 0;
                }
            }

            paramIndex = -1;
            return 1;
        }

        /// <summary>
        /// Gets a generic parameter by it's index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="parameter">The found parameter.</param>
        /// <returns>On success: 0. On no parameter found: 1.</returns>
        public int GetGenericParamByIndex(int index, out SParameter parameter)
        {
            if (index < 0 || index >= this._numParams)
            {
                parameter = new SParameter();
                return 1;
            }

            parameter = this._parameters[index];
            return 0;
        }

        /// <summary>
        /// Finds a parameter by it's name.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="foundParam">The found parameter.</param>
        /// <returns>On success: 0. On no found parameter: 1.</returns>
        public int GetGenericParamByName(string paramName, ref SParameter foundParam)
        {
            for (int i = 0; i < this._parameters.Count; i++)
            {
                if (this._parameters[i].ParamName == paramName)
                {
                    foundParam = this._parameters[i];
                    return 0;
                }
            }

            return 1;
        }

        /// <summary>
        /// Gets the value of a specified parameter.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="index">The index of the element the requested parameter is in.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>On success: 0. On invalid index number: -1. On unsupported parameter type: -2.</returns>
        public int GetParamValue<T>(int index, string paramName, ref T paramValue)
        {
            if (index >= this._numElements || index < 0)
            {
                // Error code -1: invalid index number.
                return -1;
            }

            SParameter tempParam = new SParameter();
            int retVal = this.GetGenericParamByName(paramName, ref tempParam);

            if (retVal == 1)
            {
                return retVal;
            }

            int paramAddress = this.GetElementAddress(index) + tempParam.Offset;

            if (typeof(T) == typeof(int))
            {
                paramValue = (T)(object)*(int*)(paramAddress);
            }
            else if (typeof(T) == typeof(string))
            {
                string paramString = string.Empty;
                byte[] charArray = new byte[tempParam.StringSize];
                
                // We load in the characters one by one
                for (int i = 0; i < tempParam.StringSize; i++)
                {
                    charArray[i] = *(byte*)paramAddress;
                }

                paramString = Encoding.ASCII.GetString(charArray);
                paramValue = (T)(object)paramString;
            }
            else if (typeof(T) == typeof(char))
            {
                paramValue = (T)(object)*(char*)(paramAddress);
            }
            else if (typeof(T) == typeof(short))
            {
                paramValue = (T)(object)*(short*)(paramAddress);
            }
            else if (typeof(T) == typeof(float))
            {
                paramValue = (T)(object)*(float*)(paramAddress);
            }
            else if (typeof(T) == typeof(double))
            {
                paramValue = (T)(object)*(double*)(paramAddress);
            }
            else if (typeof(T) == typeof(long))
            {
                paramValue = (T)(object)*(long*)(paramAddress);
            }
            else if (typeof(T) == typeof(char[]))
            {
                paramValue = (T)(object)*(char*)(paramAddress);
            }
            else
            {
                // Error code -2: unsupported parameter type
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// Gets the value of a parameter.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="index">The paramter index.</param>
        /// <param name="paramIndex">Index of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>On success: 0. On invalid index number: -1. On unsupported param type: -2. On invalid paramter index number: -3.</returns>
        public int GetParamValue<T>(int index, int paramIndex, ref T paramValue)
        {
            if (index >= this._numElements || index < 0)
            {
                // Error code -1: invalid index number.
                return -1;
            }

            SParameter tempParam = new SParameter();

            if (paramIndex < 0 || paramIndex >= this._numParams)
            {
                // Error code -3: invalid param index number
                return -3;
            }

            tempParam = this._parameters[paramIndex];

            int paramAddress = this.GetElementAddress(index) + tempParam.Offset;

            if (typeof(T) == typeof(int))
            {
                paramValue = (T)(object)*(int*)(paramAddress);
            }
            else if (typeof(T) == typeof(string))
            {
                string paramString = string.Empty;
                byte[] charArray = new byte[tempParam.StringSize];

                // We load in the characters one by one
                for (int i = 0; i < tempParam.StringSize; i++)
                {
                    charArray[i] = *(byte*)paramAddress;
                }

                paramString = Encoding.ASCII.GetString(charArray);
                paramValue = (T)(object)paramString;
            }
            else if (typeof(T) == typeof(char))
            {
                paramValue = (T)(object)*(char*)(paramAddress);
            }
            else if (typeof(T) == typeof(short))
            {
                paramValue = (T)(object)*(short*)(paramAddress);
            }
            else if (typeof(T) == typeof(float))
            {
                paramValue = (T)(object)*(float*)(paramAddress);
            }
            else if (typeof(T) == typeof(double))
            {
                paramValue = (T)(object)*(double*)(paramAddress);
            }
            else if (typeof(T) == typeof(long))
            {
                paramValue = (T)(object)*(long*)(paramAddress);
            }
            else if (typeof(T) == typeof(char[]))
            {
                paramValue = (T)(object)*(char*)(paramAddress);
            }
            else
            {
                // Error code -2: unsupported parameter type
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// Sets the param value.
        /// </summary>
        /// <typeparam name="T">The new value's type.</typeparam>
        /// <param name="index">The index.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="newVal">The new val.</param>
        /// <returns>On success: 0. On invalid index number: -1. On unsupported param type: -2. On input string too long: -3.</returns>
        public int SetParamValue<T>(int index, string paramName, T newVal)
        {
            if (index >= this._numElements || index < 0)
            {
                // Error code -1: invalid index number.
                return -1;
            }

            SParameter tempParam = new SParameter();
            int retVal = this.GetGenericParamByName(paramName, ref tempParam);

            if (retVal == 1)
            {
                return retVal;
            }

            int paramAddress = this.GetElementAddress(index) + tempParam.Offset;

            if (typeof(T) == typeof(int))
            {
                int* tempPtr = (int*)paramAddress;
                *tempPtr = (int)(object)newVal;
            }
            else if (typeof(T) == typeof(string))
            {
                string newString = (string)(object)newVal;

                if (newString.Length > tempParam.StringSize)
                {
                    // Error code -3: string length too long
                    return -3;
                }

                // Paste in the characters one by one
                for (int i = 0; i < tempParam.StringSize; i++)
                {
                    char* tempPtr = (char*)(paramAddress + i * sizeof(char));
                    *tempPtr = newString[i];
                }
            }
            else if (typeof(T) == typeof(char))
            {
                char* tempPtr = (char*)paramAddress;
                *tempPtr = (char)(object)newVal;
            }
            else if (typeof(T) == typeof(short))
            {
                short* tempPtr = (short*)paramAddress;
                *tempPtr = (short)(object)newVal;
            }
            else if (typeof(T) == typeof(float))
            {
                float* tempPtr = (float*)paramAddress;
                *tempPtr = (float)(object)newVal;
            }
            else if (typeof(T) == typeof(double))
            {
                double* tempPtr = (double*)paramAddress;
                *tempPtr = (double)(object)newVal;
            }
            else if (typeof(T) == typeof(long))
            {
                long* tempPtr = (long*)paramAddress;
                *tempPtr = (long)(object)newVal;
            }
            else if (typeof(T) == typeof(char[]))
            {
                char* tempPtr = (char*)paramAddress;
                *tempPtr = (char)(object)newVal;
            }
            else
            {
                // Error code -2: unsupported parameter type
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// Changes a parameter's value by it's index.
        /// </summary>
        /// <typeparam name="T">The new value's type.</typeparam>
        /// <param name="index">The element index.</param>
        /// <param name="paramIndex">The parameter index.</param>
        /// <param name="newVal">The new value.</param>
        /// <returns>On success: 0. On invalid index number: -1. On unsupported parameter type: -2. On string length too long: -3. On invalid param index number: -4.</returns>
        public int SetParamValue<T>(int index, int paramIndex, T newVal)
        {
            if (index >= this._numElements || index < 0)
            {
                // Error code -1: invalid index number.
                return -1;
            }

            SParameter tempParam = new SParameter();

            if (paramIndex >= this.NumParams || index < 0)
            {
                // Error code -4: invalid param index number
                return -4;
            }

            tempParam = this._parameters[paramIndex];

            int paramAddress = this.GetElementAddress(index) + tempParam.Offset;

            if (typeof(T) == typeof(int))
            {
                int* tempPtr = (int*)paramAddress;
                *tempPtr = (int)(object)newVal;
            }
            else if (typeof(T) == typeof(string))
            {
                string newString = (string)(object)newVal;

                if (newString.Length > tempParam.StringSize)
                {
                    // Error code -3: string length too long
                    return -3;
                }

                // Paste in the characters one by one
                for (int i = 0; i < tempParam.StringSize; i++)
                {
                    char* tempPtr = (char*)(paramAddress + i * sizeof(char));
                    *tempPtr = newString[i];
                }
            }
            else if (typeof(T) == typeof(char))
            {
                char* tempPtr = (char*)paramAddress;
                *tempPtr = (char)(object)newVal;
            }
            else if (typeof(T) == typeof(short))
            {
                short* tempPtr = (short*)paramAddress;
                *tempPtr = (short)(object)newVal;
            }
            else if (typeof(T) == typeof(float))
            {
                float* tempPtr = (float*)paramAddress;
                *tempPtr = (float)(object)newVal;
            }
            else if (typeof(T) == typeof(double))
            {
                double* tempPtr = (double*)paramAddress;
                *tempPtr = (double)(object)newVal;
            }
            else if (typeof(T) == typeof(long))
            {
                long* tempPtr = (long*)paramAddress;
                *tempPtr = (long)(object)newVal;
            }
            else if (typeof(T) == typeof(char[]))
            {
                char* tempPtr = (char*)paramAddress;
                *tempPtr = (char)(object)newVal;
            }
            else
            {
                // Error code -2: unsupported parameter type
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// Gets the address of an element in the array with the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>On success: the address of the element. On invalid index number: -1</returns>
        private int GetElementAddress(int index)
        {
            if (index >= this._numElements || index < 0)
            {
                // Error code -1: invalid index number
                return -1;
            }

            return (this._arrayStartAddress + this._size * index);
        }
    }
}
