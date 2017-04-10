# TUF2000-M PARSER
A simple ASP.NET WebForms application that parses data received from a TUF2000-M (Ultrasonic Flow Meter) device, and presents it as comprehensible values in a user interface.

The TUF2000-M produces readings in form of a text feed that contains the reading date as well as 100 register values that correspond to the ...

## Classes

Custom classes are located in the _App_Code_ folder.

### Entry.cs

The class _Entry_ represents a range of values in the register. The range is defined by passing the starting register number and the length of the range to the constructor. In addition the value format (specified in code through the enumerator EntryFormat) is passed to the constructor to ensure proper parsing. Optionally a unit string (e.g. "m/s") and a special rule function delegate can be passed to the contructor. The special rule function is used in cases where the desired value cannot be produced through generic conversion.

The public _SetValue_ method contains the main gears of the Entry class, as this is where raw values are parsed and converted to the desired value and format, either through generic conversion or through the use of a predefined special rule.

The predefined static dictionary _\_errorMeanings_ contains error references for the special Error BIT value. 

### BinaryHelper.cs

The static class _BinaryHelper_ contains a few helper methods for quick conversion between binary strings and different numerical value types. These methods are publically accessible.

## Process

The entire process is carried out in the _PageLoad_ event of the Default page class (Default.aspx.cs).

Register entries are predefined in the Entry[] array _\_entries_ according to the register table available in the TUF2000-M manual.

The parser application uses a StreamReader to read the data from the text file. It begins by parsing the date, before moving on to reading each register and its respective value. These values are stored in the _rawEntries_ dictionary, which is indexed by register number.

Once all the raw data has been gathered, a foreach iterator goes through each Entry object in the _\_entries_ array. Corresponding values for each entry are fetched from the _rawEntries_ dictionary and applied to the object via the public _SetValue_ method. Finally the parsed value is added to the UI as the contents of a set of dynamically added web controls.

## Presentation

The UI is presented on a single page (Default.aspx).

...
