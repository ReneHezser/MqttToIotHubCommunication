using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using MqttToIotHub.Configuration;

namespace MqttToIotHub
{
    class IotHub
    {
        private readonly IotHubSettings _settings;
        private DeviceClient _client;

        public IotHub(IotHubSettings settings)
        {
            _settings = settings;
        }

        internal void Connect()
        {
            try
            {
                Console.Write("Connecting to Iot Hub with device: " + _settings.DeviceId + "... ");
                _client = DeviceClient.CreateFromConnectionString(_settings.ConnectionString, TransportType.Mqtt);

                _client.OpenAsync().Wait();
                Console.WriteLine("Connected to Iot Hub.");

                _client.SetConnectionStatusChangesHandler(OnConnectionStatusChanges);
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        private void OnConnectionStatusChanges(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            Console.WriteLine("Connection to IoT Hub closed. Trying to reconnect...");
            Task.Delay(2000);
            _client.OpenAsync().Wait();
            Console.WriteLine("Connected to IoT Hub.");
        }

        internal async Task<Message> ReceiveAsync()
        {
            return await _client.ReceiveAsync();
        }

        internal async Task SendMessage(MqttEventArgs args)
        {
            Console.WriteLine("Device sending {0} messages to IoTHub...\n", args.Message);
            string dataBuffer = string.Format("{{\"deviceId\":\"{0}\",\"message\":\"{1}\",\"value\":\"{2}\"}}", _settings.DeviceId, args.Message, args.Value);
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));

            await _client.SendEventAsync(eventMessage);
        }
    }
}