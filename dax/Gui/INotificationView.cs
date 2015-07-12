using System;

namespace dax.Gui
{
    public interface INotificationView
    {
        void SetStatus(String text);

        void ShowError(String message);
    }
}
