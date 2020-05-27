using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace FBS_Telemetry_Viewer
{
    class DisplayData
    {
        // Display data
        public void Display(string jsonData, HttpResponseHeaders headers, DataGridView dataGridView, Label label)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add();
            dataTable.Columns.Add();

            // Parsing the Json data and adding data to the DataTable two columns at a time.
            JToken json = JToken.Parse(jsonData);
            IEnumerable<KeyValuePair<string, JValue>> fields = new JsonFieldsCollector(json).GetAllFields();

            if (fields.Count() > 0)
            {
                foreach (var field in fields)
                {
                    dataTable.Rows.Add(field.Key, field.Value);
                }
            }
            else
            {
                dataTable.Rows.Add("No data");
            }

            // Complete the number of rows in DataGridView
            dataGridView.RowCount = dataTable.Rows.Count;

            // Index of first visible row.
            // If there is no data, the return value will be -1.
            int firstVisibleIndex = dataGridView.FirstDisplayedScrollingRowIndex;
            if (firstVisibleIndex >= 0)
            {
                // Calculate the index of last visible row.
                int maxIndex = (dataGridView.Height / dataGridView.Rows[0].Height + firstVisibleIndex) - 1;
                maxIndex = (maxIndex > dataGridView.RowCount) ? dataGridView.RowCount : maxIndex;
                // Update values only in the visible range of DataGridView.
                for (int i = firstVisibleIndex; i < maxIndex; i++)
                {
                    dataGridView[0, i].Value = dataTable.Rows[i].ItemArray[0];
                    dataGridView[1, i].Value = dataTable.Rows[i].ItemArray[1];
                }
            }

            // Output Timestamp from Http headers.
            if (headers != null)
            {
                var headerTimeStamp =
                    headers.FirstOrDefault(pair => string.Compare(pair.Key, "Engine-Timestamp") == 0).Value;
                if (headerTimeStamp != null)
                {
                    var timeStamp = double.Parse(headerTimeStamp.ElementAt(0)/*,System.Globalization.NumberStyles.Float*/);
                    var span = TimeSpan.FromSeconds(timeStamp);
                    label.Text = $"TS: {timeStamp.ToString("F3")} ({span.ToString(@"h\:mm\:ss")})";
                }
            }
        }
    }
}
