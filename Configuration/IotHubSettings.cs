using System.ComponentModel;
using System.Xml.Serialization;

namespace MqttToIotHub.Configuration
{
    [XmlRoot("IotHub")]
    public class IotHubSettings
    {
        [Description("HostName=<iothub_host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>")]
        [XmlElement(ElementName = "ConnectionString")]
        public string ConnectionString { get; set; }

        [XmlElement(ElementName = "DeviceId")]
        public string DeviceId { get; set; }
    }
}