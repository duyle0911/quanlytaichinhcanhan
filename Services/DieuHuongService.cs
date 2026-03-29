using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Controls;

namespace WpfApp3.Services
{
    public sealed class DieuHuongService : IDieuHuongService
    {
        private readonly IServiceProvider _provider;

        public DieuHuongService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Page LayTrang<T>() where T : Page
        {
            return _provider.GetRequiredService<T>();
        }
    }
}