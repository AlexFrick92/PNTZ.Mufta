using System;
using System.Xml.Linq;


namespace DpConnect.Configuration
{
    public class DpConfiguration<T> : IDpConfiguration
        where T : IDpSourceConfiguration
    {
        public string PropertyName { get; set; }

        public string ConnectionId { get; set; }

        public T SourceConfiguration { get; set; }


        // Явная реализация интерфейса IDpConfiguration
        IDpSourceConfiguration IDpConfiguration.SourceConfiguration
        {
            get => SourceConfiguration;
            set => SourceConfiguration = (T)value;
        }
    }
}
