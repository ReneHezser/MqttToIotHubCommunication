using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using MqttToIotHub.Configuration;

namespace MqttToIotHub
{
    /// <summary>
    /// Connect to an MQTT Broker and an IoT Hub.
    /// Relay all messages from MQTT to Iot Hub, that match the subscribed topics.
    /// </summary>
    class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args">Pass a filename to load the configuration from (optional)</param>
        static void Main(string[] args)
        {
            Settings settings = Settings.ReadConfiguration(args);
            Console.WriteLine("Starting...");

            var mqtt = new Mqtt(settings.Mqtt);
            mqtt.Connect();

            var iotHub = new IotHub(settings.IotHub);
            iotHub.Connect();
            mqtt.OnMqttMessageEvent += delegate (MqttEventArgs eventArgs)
            {
                // no need to wait for the result
                iotHub.SendMessage(eventArgs);
            };

            while (true)
            {
                // wait for incoming messages from IoT Hub
                Task<Message> eventData = iotHub.ReceiveAsync();
                eventData.Wait();
                RelayMessage(eventData, mqtt);
            }
        }

        /// <summary>
        /// Messages from the IoT Hub will be published to the local MQTT, if it contains two properties:
        /// "path" is the SmartHome path
        /// "value" could be True or a value
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="mqtt"></param>
        private static void RelayMessage(Task<Message> eventData, Mqtt mqtt)
        {
            if (eventData == null || eventData.Result == null)
                return;

            var properties = eventData.Result.Properties;

            Console.Write("Received message from IoT Hub: ");
            foreach (var property in properties) Console.Write(property.Key + ":" + property.Value + ",");
            Console.WriteLine();

            if (properties.Keys.Count == 2 && properties.ContainsKey("path") && properties.ContainsKey("value"))
            {
                mqtt.Send(properties["path"], properties["value"]);
            }
            else
            {
                Console.WriteLine("Ignoring message '" + eventData.ToString());
            }
        }
    }
}