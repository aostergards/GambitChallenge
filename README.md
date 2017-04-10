# TUF2000-M PARSER
A simple ASP.NET WebForms application that parses data received from a TUF2000-M (Ultrasonic Flow Meter) device, and presents it as comprehensible values in a user interface.

The TUF2000-M produces readings in form of a text feed that contains the reading date as well as 100 register values that correspond to the ......

Classes

The class "Entry" (located in App_Code) represents a range of values in the register. The range is defined by passing the starting register number and the length of the range to the constructor. In addition the value format (specified in code through the enumerator EntryFormat) is passed to the constructor to ensure proper parsing. Optionally a unit string (e.g. "m/s") and a special rule function delegate can be passed to the contructor. The special rule function is used in cases where the desired value cannot be produced through generic parsing.

The static class "BinaryHelper" (located in App_Code) contains a few helper methods for conversion between binary strings and different numerical value types.

Process

The parser application uses a StreamReader to read the data from the text file. It begins by parsing the date, before moving on to reading each register and its respective value. These values are stored in a dictionary indexed by register number.

