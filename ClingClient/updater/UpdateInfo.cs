using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ClingUpdater {
    [XmlRoot("update")]
    public class UpdateInfo {
        [XmlAttribute]
        public bool exists;

        [XmlElement]
        public string packageType;

        [XmlElement]
        public string distributionType {get; set;}

        [XmlElement]
        public string platform;

        [XmlElement]
        public string version;

        [XmlElement]
        public string packageFileName;

        [XmlElement]
        public string packageUrl;

        [XmlElement]
        public string packageChecksum;

        [XmlElement]
        public string releaseNotes;

        [XmlElement]
        public bool isCritical;
    }
}