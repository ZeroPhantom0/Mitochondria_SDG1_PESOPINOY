using System;
using System.Windows.Forms;

namespace PesoPinoy.UI.Helpers
{
    public static class UIHelper
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => action()));
            }
            else
            {
                action();
            }
        }

        public static void SafeSetText(this Control control, string text)
        {
            control.InvokeIfRequired(() => control.Text = text);
        }

        public static void SafeAddRow(this DataGridView grid, params object[] values)
        {
            grid.InvokeIfRequired(() => grid.Rows.Add(values));
        }

        public static void SafeClearRows(this DataGridView grid)
        {
            grid.InvokeIfRequired(() => grid.Rows.Clear());
        }
    }
}