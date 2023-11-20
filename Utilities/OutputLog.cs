using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Multi_Tool.Utilities
{
    internal class OutputLog
    {
        ListView outputList;

        public OutputLog(ListView list = null)
        {
            outputList = list;
        }

        [STAThread]
        public void WriteToOutput(object entry)
        {
            if (outputList != null)
            {
                if (!(entry is ListViewItem))
                {
#if !RELEASE
                    Debug.WriteLine("Writing to Output Log - " + entry);
#endif
                    ListViewItem item = new ListViewItem();
                    item.Content = entry;
                    outputList.Items.Add(item);
                }
                else
                {
#if !RELEASE
                    Debug.WriteLine("Writing to Output Log - " + (entry as ListViewItem).Content.ToString());
#endif
                    outputList.Items.Add(entry);
                }
            }
        }

        [STAThread]
        public void WriteToOutput(params object[] entry)
        {
            if (outputList != null)
            {
                foreach (object param in entry)
                {
                    if (!(param is ListViewItem))
                    {
#if !RELEASE
                        Debug.WriteLine("Writing to Output Log - " + param);
#endif
                        ListViewItem item = new ListViewItem();
                        item.Content = param;
                        outputList.Items.Add(item);
                    }
                    else
                    {
#if !RELEASE
                        Debug.WriteLine("Writing to Output Log - " + (param as ListViewItem).Content.ToString());
#endif
                        outputList.Items.Add(param);
                    }
                }
            }
        }

        public void ClearLog()
        {
            if (outputList != null)
            {
                outputList.Items.Clear();
            }
        }

        public void RemoveItem(int item = 0)
        {
            if(outputList != null)
            {
                outputList.Items.RemoveAt(item);
            }
        }

        public void RemoveItem(object? item = null)
        {
            if(outputList != null)
            {
                if (item != null && outputList.Items.Contains(item))
                {
                    outputList.Items.Remove(item);
                }
                else
                {
                    Debug.WriteLine("No item specified or item doesn't exist in the output list.");
                }
            }
        }

        public void SetLog(ListView param1)
        {
            outputList = param1;
        }

        public ListView GetLog
        {
            get { return outputList; }
        }
    }
}
