using System;
using System.Windows;

namespace dax.Gui
{
    public interface INotificationView
    {
        void SetStatus(String text);

        void ShowError(String message);

        void ShowWarning(String message);

        void ShowMessage(String message);

        MessageBoxResult ShowQuestion(String message, MessageBoxButton buttons);
    }
}
