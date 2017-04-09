using GambitChallenge.App_Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GambitChallenge
{
    public partial class _Default : Page
    {
        private const string FeedURL = "http://tuftuf.gambitlabs.fi/feed.txt";
        private const string FeedFilename = "feed.txt";

        private DateTime _logDate;
        
        private Entry[] _entries = new Entry[]
            {
                new Entry(01, 2, "Flow Rate", "m³/h", EntryFormat.REAL4),
                new Entry(03, 2, "Energy Flow Rate", "GJ/h", EntryFormat.REAL4),
                new Entry(05, 2, "Velocity", "m/s", EntryFormat.REAL4),
                new Entry(07, 2, "Fluid sound speed", "m/s", EntryFormat.REAL4),
                new Entry(17, 2, "Positive energy acc.", EntryFormat.LONG),
                new Entry(21, 2, "Negative energy acc.", EntryFormat.LONG),
                new Entry(33, 2, "Temperature #1/inlet", "C°", EntryFormat.REAL4),
                new Entry(35, 2, "Temperature #2/outlet", "C°", EntryFormat.REAL4),
                new Entry(53, 3, "Calendar", EntryFormat.CALENDAR_BCD),
                new Entry(72, 1, "Error Code", EntryFormat.ERROR_BIT),
                new Entry(93, 1, "Upstream strength", EntryFormat.INTEGER),
                new Entry(94, 1, "Downstream strength", EntryFormat.INTEGER),
                new Entry(96, 1, "UI Language", EntryFormat.INTEGER, delegate(int[] values){ return values[0] == 0 ? "English" : "Chinese"; }),
                new Entry(99, 2, "Reynold's number", EntryFormat.REAL4)
            };

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<int, int> rawEntries = new Dictionary<int, int>();

            // Read the feed file:
            /*
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(FeedURL);
            httpRequest.Timeout = 10000;

            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
            */

            using (StreamReader reader = new StreamReader(Server.MapPath("~/App_Data/" + FeedFilename))) //webResponse.GetResponseStream()))
            {
                bool isDateRead = false;

                while (!reader.EndOfStream)
                {
                    // Read the line into a variable:
                    string text = reader.ReadLine();

                    if (!isDateRead)
                    {
                        // The date/time is on the first line, take that first:
                        _logDate = DateTime.Parse(text);
                        isDateRead = true;
                    }
                    else
                    {
                        // Store data in dictionary:
                        string[] data = text.Split(':');
                        rawEntries.Add(int.Parse(data[0]), int.Parse(data[1]));
                    }
                }
            }

            // Iterate over available entries and fetch corresponding data from dictionary:
            foreach (Entry entry in _entries)
            {
                int regID = entry.RegisterID;
                int slots = entry.Slots;

                int[] values = new int[slots];

                // Collect all values that correspond to this entry:
                for (int i = 0; i < slots; i++)
                {
                    values[i] = rawEntries[regID + i];
                }

                // Send value to entry object for parsing and decoding:
                entry.SetValue(values);

                // Present value in UI:
                AddToUI(entry);
            }

            // Present other decoded values in interface:
            LogDateTimeTextBox.Text = _logDate.ToString("dd.MM.yyyy - HH:mm:ss");
        }

        private void AddToUI(Entry entry)
        {
            Panel wrapper = new Panel() { CssClass = "label-wrapper" };
            {
                if (entry.Format == EntryFormat.ERROR_BIT)
                    wrapper.CssClass += " red";

                wrapper.Controls.Add(new Literal() { Text = "<div>" + entry.Name + "</div>" });
                wrapper.Controls.Add(new TextBox() { Text = entry.ToString(), Enabled = false });
            }
            ValuesPanel.Controls.Add(wrapper);
        }
    }
}