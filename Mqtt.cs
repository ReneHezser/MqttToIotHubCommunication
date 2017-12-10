using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using MqttToIotHub.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttToIotHub
{
    class Mqtt
    {
        private readonly Configuration.MqttSettings _settings;
        private readonly string _clientId;
        private MqttClient _client;

        public delegate void MQttMessageEventDelegate(MqttEventArgs args);
        public event MQttMessageEventDelegate OnMqttMessageEvent;

        public Mqtt(Configuration.MqttSettings settings)
        {
            _settings = settings;
            _clientId = Guid.NewGuid().ToString();
        }

        internal void Connect()
        {
            Console.Write("Connecting to MQTT broker on '" + _settings.HostName + "'... ");
            _client = new MqttClient(_settings.HostName);

            // register to message received
            _client.MqttMsgPublishReceived += OnTopicReceived;
            _client.Subscribe(_settings.SubscribedTopics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            byte code = _client.Connect(_clientId);
            Console.WriteLine("Connected to MQTT (" + code + ").");

            _client.ConnectionClosed += OnConnectionClosed;
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection to MQTT closed. Trying to reconnect...");
            Task.Delay(2000);
            byte code = _client.Connect(_clientId);
            Console.WriteLine("Connected to MQTT (" + code + ").");
        }

        void OnTopicReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var topic = e.Topic;
            var message = Encoding.Default.GetString(e.Message);
            object value = ParseObjectValue(message);

            Console.WriteLine("MQTT: Received '" + topic + "':" + value);
            if (OnMqttMessageEvent != null)
                OnMqttMessageEvent(new MqttEventArgs(topic, value));
        }

        internal void Send(string topic, string value)
        {
            Console.WriteLine("Publishing message to MQTT: '" + topic + "':'" + value + "'");
            var message = Encoding.Default.GetBytes(value);
            _client.Publish(topic, message);
        }

        private static object ParseObjectValue(string message)
        {
            object value = message;
            if (value == null) return null;

            int numberInt;
            double numberDouble;
            if (message.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                value = true;
            }
            else if (message.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                value = false;
            }
            else if (int.TryParse(message, out numberInt))
            {
                value = numberInt;
            }
            else if (double.TryParse(message, out numberDouble))
            {
                value = numberDouble;
            }

            return value;
        }
    }
}