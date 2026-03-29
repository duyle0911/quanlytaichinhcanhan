using System.Windows.Controls;

namespace WpfApp3.Services
{
    public interface IDieuHuongService
    {
        Page LayTrang<T>() where T : Page, new();
    }
}