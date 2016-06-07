using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class MatSimsVehicles
{
    [XmlElement("vehicleType")]
    public List<VehicleType> vTypes;

    [XmlElement("vehicle")]
    public List<Vehicle> vehicles;

    public static MatSimsVehicles Load(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "vehicleDefinitions";
            xRoot.Namespace = "http://www.matsim.org/files/dtd";
            xRoot.IsNullable = true;

            var serializer = new XmlSerializer(typeof(MatSimsVehicles), xRoot);
            return serializer.Deserialize(stream) as MatSimsVehicles;
        }
    }

    public void Save(string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var serializer = new XmlSerializer(typeof(MatSimsVehicles));
            serializer.Serialize(stream, this);
        }
    }

    public string GetVehicleTypeString(string id)
    {
        return vTypes.Find(t => t.id == id).ToString();
    }


    public string GetVehicleString(string id)
    {
        return vehicles.Find(veh => veh.id == id).ToString();
    }
}

public class VehicleType
{
    [XmlAttribute]
    public string id;

    public override string ToString()
    {
        //return $"id: {id}";
        return string.Format("id: {0}", id);
    }
}

public class Vehicle
{
    [XmlAttribute]
    public string id;

    [XmlAttribute]
    public string type;

    public override string ToString()
    {
        //return $"id: {id}, type: {type}";
        return string.Format("id: {0}, type: {1}", id, type);

    }
}
