using System;
using System.IO;
using System.Xml.Serialization;
using MqttToIotHub.Configuration;

namespace MqttToIotHub.Configuration
{
    [XmlRoot("RH.MqttToIotHub.Settings")]
    public class Settings
    {
        internal string FileName;

        [XmlElement(ElementName = "Mqtt")]
        public MqttSettings Mqtt { get; set; }

        [XmlElement(ElementName = "IotHub")]
        public IotHubSettings IotHub { get; set; }

        public Settings()
        {
            FileName = "config.xml";
            Mqtt = new MqttSettings();
            IotHub = new IotHubSettings();
        }

		public static Settings ReadConfiguration()
        {
            return ReadConfiguration(string.Empty);
        }

        public static Settings ReadConfiguration(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Reading config file from '" + args[0] + "'");
                return ReadConfiguration(args[0]);
            }

            return ReadConfiguration(string.Empty);
        }

        /// <summary>
		///
		/// </summary>
		/// <param name="fileName">filename of the XML configuration file</param>
		/// <returns></returns>
		public static Settings ReadConfiguration(string fileName)
        {
            var settings = new Settings();
            if (!string.IsNullOrEmpty(fileName))
                settings.FileName = fileName;

            Console.WriteLine("Deserializing settings from '" + settings.FileName + "'");
            var serializer = new XmlSerializer(settings.GetType());
            using (var fileStream = new FileStream(settings.FileName, FileMode.Open, FileAccess.Read))
            {
                settings = (Settings)serializer.Deserialize(fileStream);
            }


            return settings;
        }

        internal static void SafeConfiguration(Settings settings)
        {
            var serializer = new XmlSerializer(settings.GetType());
            using (var writer = new FileStream(settings.FileName, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}