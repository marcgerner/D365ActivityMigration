using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    [DataContract]
    internal class DataTransferObject
    {
        [DataMember]
        internal string originalfieldvalue;

        [DataMember]
        internal Guid createdby;

        [DataMember]
        internal DateTime createdon;

        [DataMember]
        internal DateTime modifiedon;

        [DataMember]
        internal Guid modifiedby;

        public string ToJson()
        {
            
            var memoryStream = new MemoryStream();
            var jsonSerializer = new DataContractJsonSerializer(typeof(DataTransferObject));
            jsonSerializer.WriteObject(memoryStream, this);
            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static DataTransferObject ParseJson(string json)
        {
            var annotationDto = new DataTransferObject();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var jsonSerializer = new DataContractJsonSerializer(annotationDto.GetType());
            annotationDto = jsonSerializer.ReadObject(memoryStream) as DataTransferObject;
            memoryStream.Close();
            return annotationDto;
        }
    }
}