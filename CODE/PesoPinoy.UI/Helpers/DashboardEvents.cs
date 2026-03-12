using System;

namespace PesoPinoy.UI.Helpers
{
    public static class DashboardEvents
    {
        public static event EventHandler DataChanged;

        public static void NotifyDataChanged()
        {
            DataChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}