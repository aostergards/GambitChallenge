# TUF2000-M PARSER
A simple ASP.NET WebForms application that parses data received from a TUF2000-M (Ultrasonic Flow Meter) device, and presents it as comprehensible values in a user interface.

The TUF2000-M produces readings in form of a text feed that contains the reading date as well as 100 register values that correspond to registers found in the device's manual. The application takes and parses this data according to each register's value format.

## Classes

Custom classes are located in the _App_Code_ folder.

### Entry.cs

The class _Entry_ represents a range of values in the register. The range is defined by passing the starting register number and the length of the range to the constructor. In addition the value format (specified in code through the enumerator EntryFormat) is passed to the constructor to ensure proper parsing. Optionally a unit string (e.g. "m/s") and a special rule function delegate can be passed to the contructor. The special rule function is used in cases where the desired value cannot be produced through generic conversion.

The public _SetValue_ method contains the main gears of the Entry class, as this is where raw values are parsed and converted to the desired value and format, either through generic conversion or through the use of a predefined special rule.

```c#
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
```

Note: The predefined static dictionary _\_errorMeanings_ contains error references for the special Error BIT value.

### BinaryHelper.cs

The static class _BinaryHelper_ contains a few helper methods for quick conversion between binary strings and different numerical value types. These methods are publically accessible.

```c#
public static class BinaryHelper
{
    public static string IntTo16BitBinaryString(int value)
    {
        return Convert.ToString(value, 2).PadLeft(16, '0');
    }

    public static float BinaryStringToSingle(string bstring)
    {
        int i = Convert.ToInt32(bstring, 2);
        byte[] bytes = BitConverter.GetBytes(i);
        return BitConverter.ToSingle(bytes, 0);
    }

    public static int BinaryBCDStringToInt(string bstring)
    {
        string high = bstring.Substring(0, 4);
        string low = bstring.Substring(4, 4);

        int tens = Convert.ToInt32(high, 2);
        int ones = Convert.ToInt32(low, 2);

        return tens * 10 + ones;
    }
}
```

## Process

The entire process is carried out in the _PageLoad_ event of the Default page class (Default.aspx.cs).

Register entries are predefined in the Entry[] array _\_entries_ according to the register table available in the TUF2000-M manual.

The parser application uses a StreamReader to read the data from the text file. It begins by parsing the date, before moving on to reading each register and its respective value. These values are stored in the _rawEntries_ dictionary, which is indexed by register number.

Once all the raw data has been gathered, a foreach iterator goes through each Entry object in the _\_entries_ array. Corresponding values for each entry are fetched from the _rawEntries_ dictionary and applied to the object via the public _SetValue_ method. Finally the parsed value is added to the UI as the contents of a set of dynamically added web controls.

## Presentation

The UI is presented on a single page (Default.aspx).

The data is presented in a very simple but readable way. Headed by the reading date, the range of select entries are presented. Some CSS (Content/Site.css) was used to customize the containers and input controls for a nicer look.
