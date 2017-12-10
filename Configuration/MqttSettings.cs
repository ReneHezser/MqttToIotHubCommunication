using System.ComponentModel;
using System.Xml.Serialization;

namespace MqttToIotHub.Configuration
{
    [XmlRoot("Mqtt")]
    public class MqttSettings
    {
        [XmlElement(ElementName = "HostName")]
        [DefaultValue("127.0.0.1")]
        public string HostName { get; set; } = "127.0.0.1";

        [XmlElement(ElementName = "Port")]
        [DefaultValue(1883)]
        public int Port { get; set; } = 1883;

        [XmlElement(ElementName = "SubscribedTopics")]
        [DefaultValue("#")]
        public string[] SubscribedTopics { get; set; } = new[] { "#" };
    }
}