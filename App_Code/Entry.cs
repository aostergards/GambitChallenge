﻿using System;
using System.Collections.Generic;

namespace GambitChallenge.App_Code
{
    public class Entry
    {
        #region Fields

        // References for the 16-bit error value range:
        private static Dictionary<int, string> _errorMeanings = new Dictionary<int, string>()
        {
            { 00, "no received signal" },
            { 01, "low received signal" },
            { 02, "poor received signal" },
            { 03, "pipe empty" },
            { 04, "hardware failure" },
            { 05, "receiving circuits gain in adjusting" },
            { 06, "frequency at the frequency output overflow" },
            { 07, "current at 4-20 mA overflow" },
            { 08, "RAM check-sum error" },
            { 09, "main clock or timer clock error" },
            { 10, "parameters check-sum error" },
            { 11, "ROM check-sum error" },
            { 12, "temperature circuits error" },
            { 13, "reserved" },
            { 14, "internal timer overflow" },
            { 15, "analog input over range" },
        };

        #endregion

        #region Properties

        public int RegisterID { get; private set; }
        public int Length { get; private set; }
        public string Name { get; private set; }
        public string Unit { get; private set; }
        public EntryFormat Format { get; private set; }

        public Func<int[], object> SpecialRule;

        public object Value { get; private set; }

        #endregion

        #region Constructors

        public Entry(int regID, int length, string name, EntryFormat format)
            : this(regID, length, name, null, format, null) { }

        public Entry(int regID, int length, string name, EntryFormat format, Func<int[], object> specialRule)
            : this(regID, length, name, null, format, specialRule) { }

        public Entry(int regID, int length, string name, string unit, EntryFormat format)
            : this(regID, length, name, unit, format, null) { }

        public Entry(int regID, int length, string name, string unit, EntryFormat format, Func<int[], object> specialRule)
        {
            this.RegisterID = regID;
            this.Length = length;
            this.Name = name;
            this.Unit = unit;
            this.Format = format;
            this.SpecialRule = specialRule;
        }

        #endregion

        #region Methods

        public void SetValue(params int[] encodedValues)
        {
            if (encodedValues.Length > 0)
            {
                if (SpecialRule != null)
                {
                    // If a special rule function has been given, use that to decode value:
                    this.Value = SpecialRule.Invoke(encodedValues);
                }
                else
                {
                    switch (this.Format)
                    {
                        case EntryFormat.CALENDAR_BCD:
                            {
                                // Special format for the calendar value:
                                if (encodedValues.Length == 3)
                                {
                                    // 6 values (s, m, h, d, M, y):
                                    int[] values = new int[encodedValues.Length * 2];
                                    
                                    // Get each value from the three registers:
                                    for (int i = 0; i < encodedValues.Length; i++)
                                    {
                                        string binary = BinaryHelper.IntTo16BitBinaryString(encodedValues[i]);

                                        string high = binary.Substring(0, 8);
                                        string low = binary.Substring(8, 8);

                                        int first = BinaryHelper.BinaryBCDStringToInt(low);
                                        int second = BinaryHelper.BinaryBCDStringToInt(high);

                                        values[0 + i * 2] = first;
                                        values[1 + i * 2] = second;
                                    }

                                    // Make sure day and month values are not zero:
                                    if (values[3] != 0 && values[4] != 0)
                                    {
                                        // Store datetime value:
                                        this.Value = new DateTime(2000 + values[5], values[4], values[3], values[2], values[1], values[0]);
                                    }
                                }
                                break;
                            }
                        case EntryFormat.ERROR_BIT:
                            {
                                // Special format for the error bit value:
                                if (encodedValues.Length == 1)
                                {
                                    int errorBit = encodedValues[0];

                                    if (_errorMeanings.ContainsKey(errorBit))
                                    {
                                        this.Value = _errorMeanings[encodedValues[0]];
                                    }
                                    else
                                    {
                                        this.Value = "Unknown error";
                                    }
                                }
                                break;
                            }
                        case EntryFormat.INTEGER:
                            {
                                // Store integer as is:
                                if (encodedValues.Length == 1)
                                {
                                    this.Value = encodedValues[0];
                                }
                                break;
                            }
                        case EntryFormat.LONG:
                            {
                                // Two values combine for a 32-bit binary number (low first):
                                if (encodedValues.Length == 2)
                                {
                                    string low = BinaryHelper.IntTo16BitBinaryString(encodedValues[0]);
                                    string high = BinaryHelper.IntTo16BitBinaryString(encodedValues[1]);

                                    string binary = high + low;

                                    this.Value = Convert.ToInt32(binary, 2);
                                }
                                break;
                            }
                        case EntryFormat.REAL4:
                            {
                                // Two values combine for a 32-bit binary number (low first):
                                if (encodedValues.Length == 2)
                                {
                                    string low = BinaryHelper.IntTo16BitBinaryString(encodedValues[0]);
                                    string high = BinaryHelper.IntTo16BitBinaryString(encodedValues[1]);

                                    string binary = high + low;

                                    this.Value = BinaryHelper.BinaryStringToSingle(binary);
                                }
                                break;
                            }
                    }
                }
            }
        }

        public override string ToString()
        {
            // Return "n/a" if no value exists:
            if (this.Value != null)
            {
                // Convert value to string and add unit (if available):
                string s = this.Value.ToString();
                if (!String.IsNullOrEmpty(this.Unit))
                {
                    s += " " + this.Unit;
                }
                return s;
            }
            return "n/a";
        }

        #endregion
    }

    public enum EntryFormat
    {
        CALENDAR_BCD,
        ERROR_BIT,
        INTEGER,
        LONG,
        REAL4,
    }
}